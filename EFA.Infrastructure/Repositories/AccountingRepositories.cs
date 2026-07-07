using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFA.Infrastructure.Repositories
{
    /// <summary>
    /// Repository لإدارة شجرة الحسابات
    /// </summary>
    public interface IChartOfAccountRepository : IGenericRepository<ChartOfAccount>
    {
        Task<ChartOfAccount?> GetByAccountNumberAsync(string accountNumber);
        Task<IEnumerable<ChartOfAccount>> GetHierarchyAsync(int? parentId = null);
        Task<IEnumerable<ChartOfAccount>> SearchAccountsAsync(string searchTerm);
        Task<IEnumerable<ChartOfAccount>> GetAccountsByTypeAsync(string accountType);
        Task<bool> HasSubAccountsAsync(int accountId);
    }

    public class ChartOfAccountRepository : GenericRepository<ChartOfAccount>, IChartOfAccountRepository
    {
        public ChartOfAccountRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<ChartOfAccount?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _context.ChartOfAccounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<IEnumerable<ChartOfAccount>> GetHierarchyAsync(int? parentId = null)
        {
            var query = _context.ChartOfAccounts.AsQueryable();
            if (parentId.HasValue)
                query = query.Where(a => a.ParentAccountId == parentId);
            else
                query = query.Where(a => a.ParentAccountId == null);

            return await query.OrderBy(a => a.AccountNumber).ToListAsync();
        }

        public async Task<IEnumerable<ChartOfAccount>> SearchAccountsAsync(string searchTerm)
        {
            return await _context.ChartOfAccounts
                .Where(a => a.AccountNumber.Contains(searchTerm) ||
                           a.AccountNameAr.Contains(searchTerm) ||
                           a.AccountNameEn.Contains(searchTerm))
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChartOfAccount>> GetAccountsByTypeAsync(string accountType)
        {
            return await _context.ChartOfAccounts
                .Where(a => a.AccountType == accountType && a.IsActive)
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task<bool> HasSubAccountsAsync(int accountId)
        {
            return await _context.ChartOfAccounts
                .AnyAsync(a => a.ParentAccountId == accountId);
        }
    }

    /// <summary>
    /// Repository لإدارة أنواع اليوميات
    /// </summary>
    public interface IJournalTypeRepository : IGenericRepository<JournalType>
    {
        Task<JournalType?> GetByCodeAsync(string code);
    }

    public class JournalTypeRepository : GenericRepository<JournalType>, IJournalTypeRepository
    {
        public JournalTypeRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<JournalType?> GetByCodeAsync(string code)
        {
            return await _context.JournalTypes
                .FirstOrDefaultAsync(jt => jt.JournalTypeCode == code);
        }
    }

    /// <summary>
    /// Repository لإدارة اليوميات المحاسبية
    /// </summary>
    public interface IJournalRepository : IGenericRepository<Journal>
    {
        Task<Journal?> GetByNumberAsync(string journalNumber);
        Task<IEnumerable<Journal>> GetByPeriodAsync(int fiscalPeriodId);
        Task<IEnumerable<Journal>> GetByStatusAsync(string status);
        Task<IEnumerable<Journal>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalDebitAsync(int journalId);
        Task<decimal> GetTotalCreditAsync(int journalId);
    }

    public class JournalRepository : GenericRepository<Journal>, IJournalRepository
    {
        public JournalRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<Journal?> GetByNumberAsync(string journalNumber)
        {
            return await _context.Journals
                .Include(j => j.JournalType)
                .Include(j => j.JournalEntries)
                .FirstOrDefaultAsync(j => j.JournalNumber == journalNumber);
        }

        public async Task<IEnumerable<Journal>> GetByPeriodAsync(int fiscalPeriodId)
        {
            return await _context.Journals
                .Where(j => j.FiscalPeriodId == fiscalPeriodId)
                .Include(j => j.JournalType)
                .OrderByDescending(j => j.JournalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Journal>> GetByStatusAsync(string status)
        {
            return await _context.Journals
                .Where(j => j.JournalStatus == status)
                .OrderByDescending(j => j.JournalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Journal>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Journals
                .Where(j => j.JournalDate >= startDate && j.JournalDate <= endDate)
                .OrderBy(j => j.JournalDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalDebitAsync(int journalId)
        {
            return await _context.JournalEntries
                .Where(je => je.JournalId == journalId)
                .SumAsync(je => je.DebitAmount);
        }

        public async Task<decimal> GetTotalCreditAsync(int journalId)
        {
            return await _context.JournalEntries
                .Where(je => je.JournalId == journalId)
                .SumAsync(je => je.CreditAmount);
        }
    }

    /// <summary>
    /// Repository لإدارة سندات القيود
    /// </summary>
    public interface IJournalEntryRepository : IGenericRepository<JournalEntry>
    {
        Task<IEnumerable<JournalEntry>> GetByJournalAsync(int journalId);
        Task<IEnumerable<JournalEntry>> GetByAccountAsync(int accountId);
        Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetAccountBalanceAsync(int accountId, DateTime? asOfDate = null);
    }

    public class JournalEntryRepository : GenericRepository<JournalEntry>, IJournalEntryRepository
    {
        public JournalEntryRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<JournalEntry>> GetByJournalAsync(int journalId)
        {
            return await _context.JournalEntries
                .Where(je => je.JournalId == journalId)
                .Include(je => je.Account)
                .Include(je => je.CostCenter)
                .OrderBy(je => je.LineNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntry>> GetByAccountAsync(int accountId)
        {
            return await _context.JournalEntries
                .Where(je => je.AccountId == accountId)
                .Include(je => je.Journal)
                .OrderByDescending(je => je.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.JournalEntries
                .Where(je => je.CreatedDate >= startDate && je.CreatedDate <= endDate)
                .Include(je => je.Journal)
                .Include(je => je.Account)
                .ToListAsync();
        }

        public async Task<decimal> GetAccountBalanceAsync(int accountId, DateTime? asOfDate = null)
        {
            var query = _context.JournalEntries
                .Where(je => je.AccountId == accountId);

            if (asOfDate.HasValue)
                query = query.Where(je => je.CreatedDate <= asOfDate.Value);

            var debit = await query.SumAsync(je => je.DebitAmount);
            var credit = await query.SumAsync(je => je.CreditAmount);

            return debit - credit;
        }
    }

    /// <summary>
    /// Repository لإدارة الأرصدة الافتتاحية
    /// </summary>
    public interface IOpeningBalanceRepository : IGenericRepository<OpeningBalance>
    {
        Task<OpeningBalance?> GetByAccountAndPeriodAsync(int accountId, int fiscalPeriodId);
        Task<IEnumerable<OpeningBalance>> GetByPeriodAsync(int fiscalPeriodId);
        Task<IEnumerable<OpeningBalance>> GetDraftBalancesAsync(int fiscalPeriodId);
    }

    public class OpeningBalanceRepository : GenericRepository<OpeningBalance>, IOpeningBalanceRepository
    {
        public OpeningBalanceRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<OpeningBalance?> GetByAccountAndPeriodAsync(int accountId, int fiscalPeriodId)
        {
            return await _context.OpeningBalances
                .FirstOrDefaultAsync(ob => ob.AccountId == accountId && ob.FiscalPeriodId == fiscalPeriodId);
        }

        public async Task<IEnumerable<OpeningBalance>> GetByPeriodAsync(int fiscalPeriodId)
        {
            return await _context.OpeningBalances
                .Where(ob => ob.FiscalPeriodId == fiscalPeriodId)
                .Include(ob => ob.Account)
                .ToListAsync();
        }

        public async Task<IEnumerable<OpeningBalance>> GetDraftBalancesAsync(int fiscalPeriodId)
        {
            return await _context.OpeningBalances
                .Where(ob => ob.FiscalPeriodId == fiscalPeriodId && ob.Status == "Draft")
                .ToListAsync();
        }
    }

    /// <summary>
    /// Repository لإدارة الفترات المحاسبية
    /// </summary>
    public interface IFiscalPeriodRepository : IGenericRepository<FiscalPeriod>
    {
        Task<FiscalPeriod?> GetCurrentPeriodAsync();
        Task<FiscalPeriod?> GetByYearAndNumberAsync(int year, int periodNumber);
        Task<IEnumerable<FiscalPeriod>> GetByYearAsync(int year);
        Task<FiscalPeriod?> GetPeriodByDateAsync(DateTime date);
    }

    public class FiscalPeriodRepository : GenericRepository<FiscalPeriod>, IFiscalPeriodRepository
    {
        public FiscalPeriodRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<FiscalPeriod?> GetCurrentPeriodAsync()
        {
            return await _context.FiscalPeriods
                .Where(fp => fp.PeriodStatus == "Open")
                .OrderByDescending(fp => fp.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<FiscalPeriod?> GetByYearAndNumberAsync(int year, int periodNumber)
        {
            return await _context.FiscalPeriods
                .FirstOrDefaultAsync(fp => fp.FiscalYear == year && fp.PeriodNumber == periodNumber);
        }

        public async Task<IEnumerable<FiscalPeriod>> GetByYearAsync(int year)
        {
            return await _context.FiscalPeriods
                .Where(fp => fp.FiscalYear == year)
                .OrderBy(fp => fp.PeriodNumber)
                .ToListAsync();
        }

        public async Task<FiscalPeriod?> GetPeriodByDateAsync(DateTime date)
        {
            return await _context.FiscalPeriods
                .FirstOrDefaultAsync(fp => fp.StartDate <= date && fp.EndDate >= date);
        }
    }

    /// <summary>
    /// Repository لإدارة أرصدة الحسابات
    /// </summary>
    public interface IAccountBalanceRepository : IGenericRepository<AccountBalance>
    {
        Task<AccountBalance?> GetByAccountAndPeriodAsync(int accountId, int? fiscalPeriodId);
        Task<IEnumerable<AccountBalance>> GetByPeriodAsync(int? fiscalPeriodId);
        Task<decimal> GetNetBalanceAsync(int accountId, int? fiscalPeriodId);
    }

    public class AccountBalanceRepository : GenericRepository<AccountBalance>, IAccountBalanceRepository
    {
        public AccountBalanceRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<AccountBalance?> GetByAccountAndPeriodAsync(int accountId, int? fiscalPeriodId)
        {
            return await _context.AccountBalances
                .FirstOrDefaultAsync(ab => ab.AccountId == accountId && ab.FiscalPeriodId == fiscalPeriodId);
        }

        public async Task<IEnumerable<AccountBalance>> GetByPeriodAsync(int? fiscalPeriodId)
        {
            return await _context.AccountBalances
                .Where(ab => ab.FiscalPeriodId == fiscalPeriodId)
                .Include(ab => ab.Account)
                .ToListAsync();
        }

        public async Task<decimal> GetNetBalanceAsync(int accountId, int? fiscalPeriodId)
        {
            var balance = await _context.AccountBalances
                .FirstOrDefaultAsync(ab => ab.AccountId == accountId && ab.FiscalPeriodId == fiscalPeriodId);

            if (balance == null) return 0;

            return balance.DebitBalance - balance.CreditBalance;
        }
    }

    /// <summary>
    /// Repository لتقارير الأستاذ
    /// </summary>
    public interface ILedgerReportRepository : IGenericRepository<LedgerReport>
    {
        Task<IEnumerable<LedgerReport>> GetAccountLedgerAsync(int accountId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<LedgerReport>> GetByPeriodAsync(int fiscalPeriodId);
    }

    public class LedgerReportRepository : GenericRepository<LedgerReport>, ILedgerReportRepository
    {
        public LedgerReportRepository(EFADbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LedgerReport>> GetAccountLedgerAsync(int accountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.LedgerReports.Where(lr => lr.AccountId == accountId);

            if (startDate.HasValue)
                query = query.Where(lr => lr.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(lr => lr.TransactionDate <= endDate.Value);

            return await query.OrderBy(lr => lr.TransactionDate).ToListAsync();
        }

        public async Task<IEnumerable<LedgerReport>> GetByPeriodAsync(int fiscalPeriodId)
        {
            return await _context.LedgerReports
                .Where(lr => lr.FiscalPeriodId == fiscalPeriodId)
                .OrderBy(lr => lr.TransactionDate)
                .ToListAsync();
        }
    }
}
