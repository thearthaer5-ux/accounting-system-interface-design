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
    /// خدمة تحديث الأرصدة الفورية
    /// توفر آليات لتحديث أرصدة الحسابات والمخزون بشكل فوري
    /// </summary>
    public interface IBalanceUpdateService
    {
        /// <summary>
        /// تحديث أرصدة الحسابات عند ترحيل اليومية
        /// </summary>
        Task UpdateAccountBalancesOnPostingAsync(Journal journal, int fiscalPeriodId);

        /// <summary>
        /// تحديث أرصدة المخزون عند تسجيل حركة
        /// </summary>
        Task UpdateItemBalancesAsync(int itemId, int warehouseId, decimal quantityChange);

        /// <summary>
        /// حساب رصيد الحساب في فترة زمنية محددة
        /// </summary>
        Task<(decimal debitBalance, decimal creditBalance)> CalculateAccountBalanceAsync(
            int accountId, int fiscalPeriodId);

        /// <summary>
        /// حساب رصيد المادة في مستودع محدد
        /// </summary>
        Task<decimal> CalculateItemBalanceAsync(int itemId, int warehouseId);

        /// <summary>
        /// التحقق من توازن المدين والدائن
        /// </summary>
        Task<bool> ValidateJournalBalanceAsync(Journal journal);

        /// <summary>
        /// إعادة حساب الأرصدة (مفيد للتصحيح)
        /// </summary>
        Task RecalculateBalancesAsync(int fiscalPeriodId);
    }

    public class BalanceUpdateService : IBalanceUpdateService
    {
        private readonly EFADbContext _dbContext;
        private readonly IAccountBalanceRepository _accountBalanceRepository;
        private readonly IItemBalanceRepository _itemBalanceRepository;
        private readonly ILogger<BalanceUpdateService> _logger;

        public BalanceUpdateService(
            EFADbContext dbContext,
            IAccountBalanceRepository accountBalanceRepository,
            IItemBalanceRepository itemBalanceRepository,
            ILogger<BalanceUpdateService> logger)
        {
            _dbContext = dbContext;
            _accountBalanceRepository = accountBalanceRepository;
            _itemBalanceRepository = itemBalanceRepository;
            _logger = logger;
        }

        /// <summary>
        /// تحديث أرصدة الحسابات الفورية عند ترحيل اليومية
        /// </summary>
        public async Task UpdateAccountBalancesOnPostingAsync(Journal journal, int fiscalPeriodId)
        {
            try
            {
                _logger.LogInformation($"بدء تحديث أرصدة الحسابات لليومية {journal.JournalNumber}");

                // الحصول على جميع قيود اليومية
                var entries = await _dbContext.JournalEntries
                    .Where(j => j.JournalId == journal.JournalId)
                    .ToListAsync();

                foreach (var entry in entries)
                {
                    // البحث عن رصيد الحساب الموجود
                    var existingBalance = await _accountBalanceRepository
                        .GetByAccountAndPeriodAsync(entry.AccountId, fiscalPeriodId);

                    if (existingBalance != null)
                    {
                        // تحديث الرصيد الموجود
                        existingBalance.DebitBalance += entry.DebitAmount;
                        existingBalance.CreditBalance += entry.CreditAmount;
                        existingBalance.LastUpdateDate = DateTime.UtcNow;
                        
                        await _accountBalanceRepository.UpdateAsync(existingBalance);
                    }
                    else
                    {
                        // إنشاء رصيد جديد
                        var newBalance = new AccountBalance
                        {
                            AccountId = entry.AccountId,
                            FiscalPeriodId = fiscalPeriodId,
                            DebitBalance = entry.DebitAmount,
                            CreditBalance = entry.CreditAmount,
                            CreatedDate = DateTime.UtcNow
                        };

                        await _accountBalanceRepository.AddAsync(newBalance);
                    }
                }

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"تم تحديث أرصدة الحسابات بنجاح لليومية {journal.JournalNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء تحديث أرصدة الحسابات: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// تحديث أرصدة المخزون الفورية
        /// </summary>
        public async Task UpdateItemBalancesAsync(int itemId, int warehouseId, decimal quantityChange)
        {
            try
            {
                var itemBalance = await _itemBalanceRepository
                    .GetByItemAndWarehouseAsync(itemId, warehouseId);

                if (itemBalance != null)
                {
                    itemBalance.BalanceQuantity += quantityChange;
                    itemBalance.LastMovementDate = DateTime.UtcNow;
                    itemBalance.ModifiedDate = DateTime.UtcNow;

                    await _itemBalanceRepository.UpdateAsync(itemBalance);
                    _logger.LogInformation(
                        $"تم تحديث رصيد المادة {itemId} في المستودع {warehouseId} بمقدار {quantityChange}");
                }
                else
                {
                    // إنشاء رصيد جديد
                    var newBalance = new ItemBalance
                    {
                        ItemId = itemId,
                        WarehouseId = warehouseId,
                        BalanceQuantity = quantityChange,
                        LastMovementDate = DateTime.UtcNow
                    };

                    await _itemBalanceRepository.AddAsync(newBalance);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء تحديث رصيد المخزون: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// حساب رصيد الحساب في فترة محددة
        /// </summary>
        public async Task<(decimal debitBalance, decimal creditBalance)> CalculateAccountBalanceAsync(
            int accountId, int fiscalPeriodId)
        {
            try
            {
                // البحث عن الأرصدة الافتتاحية
                var openingBalance = await _dbContext.OpeningBalances
                    .Where(o => o.AccountId == accountId && o.FiscalPeriodId == fiscalPeriodId)
                    .FirstOrDefaultAsync();

                decimal openingDebit = 0;
                decimal openingCredit = 0;

                if (openingBalance != null)
                {
                    // تصنيف الرصيد الافتتاحي حسب نوع الحساب
                    var account = await _dbContext.ChartOfAccounts.FindAsync(accountId);
                    if (account != null)
                    {
                        if (openingBalance.OpeningAmount >= 0)
                            openingDebit = openingBalance.OpeningAmount;
                        else
                            openingCredit = Math.Abs(openingBalance.OpeningAmount);
                    }
                }

                // حساب المبالغ من اليوميات
                var journalData = await _dbContext.JournalEntries
                    .Where(j => j.AccountId == accountId &&
                               j.Journal.FiscalPeriodId == fiscalPeriodId &&
                               j.Journal.JournalStatus == "Posted")
                    .GroupBy(j => 1)
                    .Select(g => new
                    {
                        TotalDebit = g.Sum(x => x.DebitAmount),
                        TotalCredit = g.Sum(x => x.CreditAmount)
                    })
                    .FirstOrDefaultAsync();

                var totalDebit = openingDebit + (journalData?.TotalDebit ?? 0);
                var totalCredit = openingCredit + (journalData?.TotalCredit ?? 0);

                _logger.LogInformation(
                    $"حساب رصيد الحساب {accountId} في الفترة {fiscalPeriodId}: " +
                    $"مدين={totalDebit}, دائن={totalCredit}");

                return (totalDebit, totalCredit);
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء حساب رصيد الحساب: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// حساب رصيد المادة في المستودع
        /// </summary>
        public async Task<decimal> CalculateItemBalanceAsync(int itemId, int warehouseId)
        {
            try
            {
                var movements = await _dbContext.ItemMovements
                    .Where(m => m.ItemId == itemId && m.WarehouseId == warehouseId &&
                               m.MovementStatus == "Completed")
                    .ToListAsync();

                decimal balance = 0;

                foreach (var movement in movements)
                {
                    if (movement.MovementType == "Inbound")
                        balance += movement.MovementQuantity;
                    else if (movement.MovementType == "Outbound")
                        balance -= movement.MovementQuantity;
                }

                return balance;
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء حساب رصيد المخزون: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// التحقق من توازن اليومية (مدين = دائن)
        /// </summary>
        public async Task<bool> ValidateJournalBalanceAsync(Journal journal)
        {
            var totalDebit = journal.JournalEntries?.Sum(e => e.DebitAmount) ?? 0;
            var totalCredit = journal.JournalEntries?.Sum(e => e.CreditAmount) ?? 0;

            var isBalanced = Math.Abs(totalDebit - totalCredit) < 0.01m; // تسامح 0.01 لأخطاء الكسور

            _logger.LogInformation(
                $"التحقق من توازن اليومية {journal.JournalNumber}: " +
                $"مدين={totalDebit}, دائن={totalCredit}, متوازنة={isBalanced}");

            return isBalanced;
        }

        /// <summary>
        /// إعادة حساب جميع الأرصدة في فترة محددة
        /// </summary>
        public async Task RecalculateBalancesAsync(int fiscalPeriodId)
        {
            try
            {
                _logger.LogInformation($"بدء إعادة حساب الأرصدة للفترة {fiscalPeriodId}");

                // حذف الأرصدة القديمة
                var oldBalances = await _dbContext.AccountBalances
                    .Where(b => b.FiscalPeriodId == fiscalPeriodId)
                    .ToListAsync();

                foreach (var balance in oldBalances)
                    _dbContext.AccountBalances.Remove(balance);

                // إعادة حساب الأرصدة لكل حساب
                var accounts = await _dbContext.ChartOfAccounts.ToListAsync();

                foreach (var account in accounts)
                {
                    var (debitBalance, creditBalance) = 
                        await CalculateAccountBalanceAsync(account.AccountId, fiscalPeriodId);

                    if (debitBalance > 0 || creditBalance > 0)
                    {
                        var newBalance = new AccountBalance
                        {
                            AccountId = account.AccountId,
                            FiscalPeriodId = fiscalPeriodId,
                            DebitBalance = debitBalance,
                            CreditBalance = creditBalance,
                            CreatedDate = DateTime.UtcNow
                        };

                        _dbContext.AccountBalances.Add(newBalance);
                    }
                }

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"تم إعادة حساب الأرصدة للفترة {fiscalPeriodId} بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء إعادة حساب الأرصدة: {ex.Message}");
                throw;
            }
        }
    }
}
