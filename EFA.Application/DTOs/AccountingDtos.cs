using System;
using System.Collections.Generic;

namespace EFA.Application.DTOs
{
    // ChartOfAccount DTOs
    public class ChartOfAccountDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string AccountNameEn { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountLevel { get; set; } = string.Empty;
        public int? ParentAccountId { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public decimal OpeningBalance { get; set; }
        public int? BranchId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ChartOfAccountCreateUpdateDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string? AccountNameEn { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string AccountLevel { get; set; } = "Detail";
        public int? ParentAccountId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        public decimal OpeningBalance { get; set; } = 0;
        public int? BranchId { get; set; }
    }

    public class ChartOfAccountHierarchyDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public List<ChartOfAccountHierarchyDto> SubAccounts { get; set; } = new();
    }

    // Journal DTOs
    public class JournalDto
    {
        public int JournalId { get; set; }
        public string JournalNumber { get; set; } = string.Empty;
        public int JournalTypeId { get; set; }
        public string JournalTypeName { get; set; } = string.Empty;
        public DateTime JournalDate { get; set; }
        public DateTime? PostingDate { get; set; }
        public string JournalStatus { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public int EntryCount { get; set; }
    }

    public class JournalCreateUpdateDto
    {
        public string JournalNumber { get; set; } = string.Empty;
        public int JournalTypeId { get; set; }
        public DateTime JournalDate { get; set; }
        public int? FiscalPeriodId { get; set; }
        public int? BranchId { get; set; }
        public string? Description { get; set; }
        public List<JournalEntryCreateDto> Entries { get; set; } = new();
    }

    // JournalEntry DTOs
    public class JournalEntryDto
    {
        public int JournalEntryId { get; set; }
        public string VoucherNumber { get; set; } = string.Empty;
        public int JournalId { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string? Description { get; set; }
    }

    public class JournalEntryCreateDto
    {
        public int AccountId { get; set; }
        public int? CostCenterId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string? Description { get; set; }
    }

    // FiscalPeriod DTOs
    public class FiscalPeriodDto
    {
        public int FiscalPeriodId { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public int FiscalYear { get; set; }
        public int PeriodNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PeriodStatus { get; set; } = string.Empty;
    }

    public class FiscalPeriodCreateUpdateDto
    {
        public string PeriodName { get; set; } = string.Empty;
        public int FiscalYear { get; set; }
        public int PeriodNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? BranchId { get; set; }
    }

    // OpeningBalance DTOs
    public class OpeningBalanceDto
    {
        public int OpeningBalanceId { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int FiscalPeriodId { get; set; }
        public decimal DebitBalance { get; set; }
        public decimal CreditBalance { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class OpeningBalanceCreateUpdateDto
    {
        public int AccountId { get; set; }
        public int FiscalPeriodId { get; set; }
        public decimal DebitBalance { get; set; }
        public decimal CreditBalance { get; set; }
        public int? BranchId { get; set; }
    }

    // AccountBalance DTOs
    public class AccountBalanceDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal DebitBalance { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal NetBalance => DebitBalance - CreditBalance;
        public DateTime LastUpdated { get; set; }
    }

    // Trial Balance Report
    public class TrialBalanceDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }

    public class TrialBalanceReportDto
    {
        public int FiscalPeriodId { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public List<TrialBalanceDto> Balances { get; set; } = new();
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public bool IsBalanced => Math.Abs(TotalDebit - TotalCredit) < 0.01m;
    }

    // Financial Statement DTOs
    public class FinancialStatementDto
    {
        public int FiscalPeriodId { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public List<FinancialStatementLineDto> Lines { get; set; } = new();
    }

    public class FinancialStatementLineDto
    {
        public string LineNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int IndentLevel { get; set; }
        public bool IsTotalLine { get; set; }
    }

    // Ledger Report
    public class LedgerReportDto
    {
        public int LedgerReportId { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string VoucherNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
    }

    // CostCenter DTOs
    public class CostCenterDto
    {
        public int CostCenterId { get; set; }
        public string CostCenterCode { get; set; } = string.Empty;
        public string CostCenterNameAr { get; set; } = string.Empty;
        public string CostCenterNameEn { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CostCenterCreateUpdateDto
    {
        public string CostCenterCode { get; set; } = string.Empty;
        public string CostCenterNameAr { get; set; } = string.Empty;
        public string? CostCenterNameEn { get; set; }
        public string? Description { get; set; }
        public int? BranchId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
