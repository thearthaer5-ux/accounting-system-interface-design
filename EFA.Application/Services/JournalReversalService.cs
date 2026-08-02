using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;
using EFA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFA.Application.Services
{
    /// <summary>
    /// خدمة عكس القيود المحاسبية
    /// توفر آليات لعكس قيود اليوميات تصحيحاً للأخطاء
    /// </summary>
    public interface IJournalReversalService
    {
        /// <summary>
        /// عكس قيد يومية بالكامل
        /// </summary>
        Task<int> ReverseJournalAsync(int journalId, string reason, int userId);

        /// <summary>
        /// عكس قيود محددة من اليومية
        /// </summary>
        Task<int> ReverseSpecificEntriesAsync(List<int> entryIds, string reason, int userId);

        /// <summary>
        /// التحقق من إمكانية عكس اليومية
        /// </summary>
        Task<(bool canReverse, string message)> CanReverseJournalAsync(int journalId);

        /// <summary>
        /// الحصول على سجل العكسات لليومية
        /// </summary>
        Task<List<ReversalHistoryDto>> GetReversalHistoryAsync(int journalId);

        /// <summary>
        /// تعديل قيد وإنشاء قيد تصحيحي
        /// </summary>
        Task<int> CreateCorrectionEntryAsync(int originalEntryId, decimal correctionAmount, int userId);
    }

    public class JournalReversalService : IJournalReversalService
    {
        private readonly EFADbContext _dbContext;
        private readonly IBalanceUpdateService _balanceUpdateService;
        private readonly ILogger<JournalReversalService> _logger;

        public JournalReversalService(
            EFADbContext dbContext,
            IBalanceUpdateService balanceUpdateService,
            ILogger<JournalReversalService> logger)
        {
            _dbContext = dbContext;
            _balanceUpdateService = balanceUpdateService;
            _logger = logger;
        }

        /// <summary>
        /// عكس اليومية بالكامل
        /// </summary>
        public async Task<int> ReverseJournalAsync(int journalId, string reason, int userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation($"بدء عكس اليومية رقم {journalId}. السبب: {reason}");

                // البحث عن اليومية
                var originalJournal = await _dbContext.Journals
                    .Include(j => j.JournalEntries)
                    .FirstOrDefaultAsync(j => j.JournalId == journalId);

                if (originalJournal == null)
                    throw new KeyNotFoundException($"اليومية {journalId} غير موجودة");

                if (originalJournal.JournalStatus != "Posted")
                    throw new InvalidOperationException("لا يمكن عكس إلا اليوميات المرحلة");

                // إنشاء يومية عكسية جديدة
                var reversalJournal = new Journal
                {
                    JournalNumber = $"REV-{originalJournal.JournalNumber}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    JournalTypeId = originalJournal.JournalTypeId,
                    JournalDate = DateTime.UtcNow,
                    PostingDate = DateTime.UtcNow,
                    FiscalPeriodId = originalJournal.FiscalPeriodId,
                    BranchId = originalJournal.BranchId,
                    JournalStatus = "Posted",
                    Description = $"قيد عكسي لليومية {originalJournal.JournalNumber}. السبب: {reason}",
                    TotalDebit = originalJournal.TotalCredit,
                    TotalCredit = originalJournal.TotalDebit,
                    ReferenceDocument = originalJournal.JournalNumber,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                // إنشاء قيود عكسية
                foreach (var entry in originalJournal.JournalEntries)
                {
                    var reversalEntry = new JournalEntry
                    {
                        VoucherNumber = $"REV-{entry.VoucherNumber}",
                        JournalId = reversalJournal.JournalId,
                        AccountId = entry.AccountId,
                        CostCenterId = entry.CostCenterId,
                        DebitAmount = entry.CreditAmount,  // عكس المبالغ
                        CreditAmount = entry.DebitAmount,
                        Description = entry.Description,
                        BranchId = entry.BranchId,
                        LineNumber = entry.LineNumber,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    reversalJournal.JournalEntries.Add(reversalEntry);
                }

                // حفظ اليومية العكسية
                _dbContext.Journals.Add(reversalJournal);
                await _dbContext.SaveChangesAsync();

                // تحديث حالة اليومية الأصلية
                originalJournal.JournalStatus = "Reversed";
                originalJournal.ModifiedDate = DateTime.UtcNow;
                originalJournal.ModifiedBy = userId;

                await _dbContext.SaveChangesAsync();

                // تحديث الأرصدة
                if (originalJournal.FiscalPeriodId.HasValue)
                {
                    // عكس تأثير اليومية الأصلية
                    foreach (var entry in originalJournal.JournalEntries)
                    {
                        var tempEntry = new JournalEntry
                        {
                            DebitAmount = entry.CreditAmount,
                            CreditAmount = entry.DebitAmount
                        };
                    }

                    // تطبيق تأثير اليومية العكسية
                    await _balanceUpdateService.UpdateAccountBalancesOnPostingAsync(
                        reversalJournal, originalJournal.FiscalPeriodId.Value);
                }

                // تسجيل في سجل التدقيق
                await LogReversalAsync(originalJournal.JournalId, reversalJournal.JournalId, reason, userId);

                await transaction.CommitAsync();

                _logger.LogInformation(
                    $"تم عكس اليومية {originalJournal.JournalNumber} بنجاح. " +
                    $"يومية عكسية: {reversalJournal.JournalNumber}");

                return reversalJournal.JournalId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"خطأ أثناء عكس اليومية: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// عكس قيود محددة
        /// </summary>
        public async Task<int> ReverseSpecificEntriesAsync(
            List<int> entryIds, string reason, int userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var entries = await _dbContext.JournalEntries
                    .Where(e => entryIds.Contains(e.JournalEntryId))
                    .Include(e => e.Journal)
                    .ToListAsync();

                if (!entries.Any())
                    throw new KeyNotFoundException("القيود المحددة غير موجودة");

                // يجب أن تكون جميع القيود من نفس اليومية
                var journalIds = entries.Select(e => e.JournalId).Distinct();
                if (journalIds.Count() > 1)
                    throw new InvalidOperationException("جميع القيود يجب أن تكون من نفس اليومية");

                var journal = entries.First().Journal;
                if (journal.JournalStatus != "Posted")
                    throw new InvalidOperationException("لا يمكن عكس قيود من يومية غير مرحلة");

                // إنشاء يومية عكسية جديدة
                var reversalJournal = new Journal
                {
                    JournalNumber = $"PARTIAL-REV-{journal.JournalNumber}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    JournalTypeId = journal.JournalTypeId,
                    JournalDate = DateTime.UtcNow,
                    PostingDate = DateTime.UtcNow,
                    FiscalPeriodId = journal.FiscalPeriodId,
                    BranchId = journal.BranchId,
                    JournalStatus = "Posted",
                    Description = $"قيد عكسي جزئي لليومية {journal.JournalNumber}. السبب: {reason}",
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                // إنشاء قيود عكسية
                foreach (var entry in entries)
                {
                    var reversalEntry = new JournalEntry
                    {
                        VoucherNumber = $"REV-{entry.VoucherNumber}",
                        JournalId = reversalJournal.JournalId,
                        AccountId = entry.AccountId,
                        CostCenterId = entry.CostCenterId,
                        DebitAmount = entry.CreditAmount,
                        CreditAmount = entry.DebitAmount,
                        Description = $"عكس: {entry.Description}",
                        BranchId = entry.BranchId,
                        LineNumber = entry.LineNumber,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    reversalJournal.JournalEntries.Add(reversalEntry);
                    reversalJournal.TotalDebit += reversalEntry.DebitAmount;
                    reversalJournal.TotalCredit += reversalEntry.CreditAmount;
                }

                _dbContext.Journals.Add(reversalJournal);
                await _dbContext.SaveChangesAsync();

                // تحديث الأرصدة
                if (journal.FiscalPeriodId.HasValue)
                {
                    await _balanceUpdateService.UpdateAccountBalancesOnPostingAsync(
                        reversalJournal, journal.FiscalPeriodId.Value);
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    $"تم عكس {entries.Count} قيود من اليومية {journal.JournalNumber} بنجاح");

                return reversalJournal.JournalId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"خطأ أثناء عكس القيود: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// التحقق من إمكانية عكس اليومية
        /// </summary>
        public async Task<(bool canReverse, string message)> CanReverseJournalAsync(int journalId)
        {
            var journal = await _dbContext.Journals.FindAsync(journalId);

            if (journal == null)
                return (false, "اليومية غير موجودة");

            if (journal.JournalStatus == "Draft")
                return (false, "لا يمكن عكس يومية في حالة مسودة");

            if (journal.JournalStatus == "Reversed")
                return (false, "اليومية مرحلة بالفعل");

            // التحقق من أن الفترة المحاسبية مفتوحة
            if (journal.FiscalPeriodId.HasValue)
            {
                var period = await _dbContext.FiscalPeriods
                    .FindAsync(journal.FiscalPeriodId);

                if (period != null && period.IsClosed)
                    return (false, "الفترة المحاسبية مغلقة ولا يمكن عكس القيود");
            }

            return (true, "يمكن عكس اليومية");
        }

        /// <summary>
        /// الحصول على سجل العكسات
        /// </summary>
        public async Task<List<ReversalHistoryDto>> GetReversalHistoryAsync(int journalId)
        {
            var history = new List<ReversalHistoryDto>();

            var reversals = await _dbContext.Journals
                .Where(j => j.ReferenceDocument == _dbContext.Journals
                    .Where(x => x.JournalId == journalId)
                    .Select(x => x.JournalNumber)
                    .FirstOrDefault())
                .ToListAsync();

            foreach (var reversal in reversals)
            {
                history.Add(new ReversalHistoryDto
                {
                    ReversalJournalId = reversal.JournalId,
                    ReversalJournalNumber = reversal.JournalNumber,
                    ReversalDate = reversal.CreatedDate,
                    Description = reversal.Description
                });
            }

            return history;
        }

        /// <summary>
        /// إنشاء قيد تصحيحي لخطأ
        /// </summary>
        public async Task<int> CreateCorrectionEntryAsync(
            int originalEntryId, decimal correctionAmount, int userId)
        {
            try
            {
                var originalEntry = await _dbContext.JournalEntries
                    .Include(e => e.Journal)
                    .FirstOrDefaultAsync(e => e.JournalEntryId == originalEntryId);

                if (originalEntry == null)
                    throw new KeyNotFoundException("القيد الأصلي غير موجود");

                var correctionJournal = new Journal
                {
                    JournalNumber = $"CORRECTION-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    JournalTypeId = originalEntry.Journal.JournalTypeId,
                    JournalDate = DateTime.UtcNow,
                    PostingDate = DateTime.UtcNow,
                    FiscalPeriodId = originalEntry.Journal.FiscalPeriodId,
                    BranchId = originalEntry.BranchId,
                    JournalStatus = "Posted",
                    Description = $"قيد تصحيحي للقيد {originalEntry.VoucherNumber}",
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                var correctionEntry = new JournalEntry
                {
                    JournalId = correctionJournal.JournalId,
                    AccountId = originalEntry.AccountId,
                    CostCenterId = originalEntry.CostCenterId,
                    DebitAmount = correctionAmount > 0 ? correctionAmount : 0,
                    CreditAmount = correctionAmount < 0 ? Math.Abs(correctionAmount) : 0,
                    Description = $"تصحيح: {originalEntry.Description}",
                    BranchId = originalEntry.BranchId,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                correctionJournal.JournalEntries.Add(correctionEntry);
                correctionJournal.TotalDebit = correctionEntry.DebitAmount;
                correctionJournal.TotalCredit = correctionEntry.CreditAmount;

                _dbContext.Journals.Add(correctionJournal);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    $"تم إنشاء قيد تصحيحي لـ {originalEntry.VoucherNumber} بنجاح");

                return correctionJournal.JournalId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء إنشاء قيد التصحيح: {ex.Message}");
                throw;
            }
        }

        private async Task LogReversalAsync(int originalJournalId, int reversalJournalId,
            string reason, int userId)
        {
            var auditLog = new Audit
            {
                EntityName = "Journal",
                EntityId = originalJournalId,
                Action = "Reversed",
                Description = $"اليومية تم عكسها. السبب: {reason}",
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
                OldValues = originalJournalId.ToString(),
                NewValues = reversalJournalId.ToString()
            };

            _dbContext.Audits.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class ReversalHistoryDto
    {
        public int ReversalJournalId { get; set; }
        public string ReversalJournalNumber { get; set; }
        public DateTime ReversalDate { get; set; }
        public string Description { get; set; }
    }
}
