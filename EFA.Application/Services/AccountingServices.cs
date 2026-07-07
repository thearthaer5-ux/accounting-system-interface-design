using AutoMapper;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;
using EFA.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EFA.Application.Services
{
    public interface IChartOfAccountService
    {
        Task<ChartOfAccountDto?> GetByIdAsync(int id);
        Task<ChartOfAccountDto?> GetByAccountNumberAsync(string accountNumber);
        Task<IEnumerable<ChartOfAccountDto>> GetAllAsync();
        Task<IEnumerable<ChartOfAccountHierarchyDto>> GetHierarchyAsync(int? parentId = null);
        Task<IEnumerable<ChartOfAccountDto>> GetByTypeAsync(string accountType);
        Task<int> CreateAsync(ChartOfAccountCreateUpdateDto dto);
        Task UpdateAsync(int id, ChartOfAccountCreateUpdateDto dto);
        Task DeleteAsync(int id);
        Task<bool> ValidateAccountAsync(int accountId);
    }

    public class ChartOfAccountService : IChartOfAccountService
    {
        private readonly IChartOfAccountRepository _repository;
        private readonly IMapper _mapper;

        public ChartOfAccountService(IChartOfAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ChartOfAccountDto?> GetByIdAsync(int id)
        {
            var account = await _repository.GetByIdAsync(id);
            return account == null ? null : _mapper.Map<ChartOfAccountDto>(account);
        }

        public async Task<ChartOfAccountDto?> GetByAccountNumberAsync(string accountNumber)
        {
            var account = await _repository.GetByAccountNumberAsync(accountNumber);
            return account == null ? null : _mapper.Map<ChartOfAccountDto>(account);
        }

        public async Task<IEnumerable<ChartOfAccountDto>> GetAllAsync()
        {
            var accounts = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ChartOfAccountDto>>(accounts);
        }

        public async Task<IEnumerable<ChartOfAccountHierarchyDto>> GetHierarchyAsync(int? parentId = null)
        {
            var accounts = await _repository.GetHierarchyAsync(parentId);
            var hierarchyList = new List<ChartOfAccountHierarchyDto>();

            foreach (var account in accounts)
            {
                var hierarchyDto = new ChartOfAccountHierarchyDto
                {
                    AccountId = account.AccountId,
                    AccountNumber = account.AccountNumber,
                    AccountNameAr = account.AccountNameAr,
                    AccountType = account.AccountType,
                    SubAccounts = (await GetHierarchyAsync(account.AccountId)).ToList()
                };
                hierarchyList.Add(hierarchyDto);
            }

            return hierarchyList;
        }

        public async Task<IEnumerable<ChartOfAccountDto>> GetByTypeAsync(string accountType)
        {
            var accounts = await _repository.GetAccountsByTypeAsync(accountType);
            return _mapper.Map<IEnumerable<ChartOfAccountDto>>(accounts);
        }

        public async Task<int> CreateAsync(ChartOfAccountCreateUpdateDto dto)
        {
            // Check for duplicate account number
            var existing = await _repository.GetByAccountNumberAsync(dto.AccountNumber);
            if (existing != null)
                throw new InvalidOperationException($"Account number {dto.AccountNumber} already exists.");

            var account = _mapper.Map<ChartOfAccount>(dto);
            account.CreatedDate = DateTime.UtcNow;
            
            await _repository.AddAsync(account);
            return account.AccountId;
        }

        public async Task UpdateAsync(int id, ChartOfAccountCreateUpdateDto dto)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {id} not found.");

            // Check for duplicate account number (excluding current account)
            var existing = await _repository.GetByAccountNumberAsync(dto.AccountNumber);
            if (existing != null && existing.AccountId != id)
                throw new InvalidOperationException($"Account number {dto.AccountNumber} already exists.");

            _mapper.Map(dto, account);
            account.ModifiedDate = DateTime.UtcNow;
            
            await _repository.UpdateAsync(account);
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null)
                throw new KeyNotFoundException($"Account with ID {id} not found.");

            // Check if account has sub-accounts
            if (await _repository.HasSubAccountsAsync(id))
                throw new InvalidOperationException("Cannot delete account with sub-accounts.");

            await _repository.DeleteAsync(account);
        }

        public async Task<bool> ValidateAccountAsync(int accountId)
        {
            var account = await _repository.GetByIdAsync(accountId);
            return account != null && account.IsActive;
        }
    }

    public interface IJournalService
    {
        Task<JournalDto?> GetByIdAsync(int id);
        Task<JournalDto?> GetByNumberAsync(string journalNumber);
        Task<IEnumerable<JournalDto>> GetByPeriodAsync(int fiscalPeriodId);
        Task<IEnumerable<JournalDto>> GetByStatusAsync(string status);
        Task<int> CreateAsync(JournalCreateUpdateDto dto, int userId);
        Task PostJournalAsync(int journalId, int userId);
        Task ReverseJournalAsync(int journalId, int userId);
    }

    public class JournalService : IJournalService
    {
        private readonly IJournalRepository _repository;
        private readonly IJournalEntryRepository _entryRepository;
        private readonly IMapper _mapper;

        public JournalService(IJournalRepository repository, IJournalEntryRepository entryRepository, IMapper mapper)
        {
            _repository = repository;
            _entryRepository = entryRepository;
            _mapper = mapper;
        }

        public async Task<JournalDto?> GetByIdAsync(int id)
        {
            var journal = await _repository.GetByIdAsync(id);
            if (journal == null) return null;

            var dto = _mapper.Map<JournalDto>(journal);
            dto.EntryCount = journal.JournalEntries.Count;
            return dto;
        }

        public async Task<JournalDto?> GetByNumberAsync(string journalNumber)
        {
            var journal = await _repository.GetByNumberAsync(journalNumber);
            if (journal == null) return null;

            var dto = _mapper.Map<JournalDto>(journal);
            dto.EntryCount = journal.JournalEntries.Count;
            return dto;
        }

        public async Task<IEnumerable<JournalDto>> GetByPeriodAsync(int fiscalPeriodId)
        {
            var journals = await _repository.GetByPeriodAsync(fiscalPeriodId);
            return _mapper.Map<IEnumerable<JournalDto>>(journals);
        }

        public async Task<IEnumerable<JournalDto>> GetByStatusAsync(string status)
        {
            var journals = await _repository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<JournalDto>>(journals);
        }

        public async Task<int> CreateAsync(JournalCreateUpdateDto dto, int userId)
        {
            // Validate entries have balanced debits and credits
            var totalDebit = dto.Entries.Sum(e => e.DebitAmount);
            var totalCredit = dto.Entries.Sum(e => e.CreditAmount);

            if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                throw new InvalidOperationException("Journal entries are not balanced.");

            var journal = _mapper.Map<Journal>(dto);
            journal.CreatedBy = userId;
            journal.CreatedDate = DateTime.UtcNow;
            journal.TotalDebit = totalDebit;
            journal.TotalCredit = totalCredit;

            await _repository.AddAsync(journal);
            return journal.JournalId;
        }

        public async Task PostJournalAsync(int journalId, int userId)
        {
            var journal = await _repository.GetByIdAsync(journalId);
            if (journal == null)
                throw new KeyNotFoundException($"Journal {journalId} not found.");

            if (journal.JournalStatus != "Draft")
                throw new InvalidOperationException("Only draft journals can be posted.");

            journal.JournalStatus = "Posted";
            journal.PostingDate = DateTime.UtcNow;
            journal.ModifiedBy = userId;
            journal.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(journal);
        }

        public async Task ReverseJournalAsync(int journalId, int userId)
        {
            var journal = await _repository.GetByIdAsync(journalId);
            if (journal == null)
                throw new KeyNotFoundException($"Journal {journalId} not found.");

            if (journal.JournalStatus != "Posted")
                throw new InvalidOperationException("Only posted journals can be reversed.");

            journal.JournalStatus = "Reversed";
            journal.ModifiedBy = userId;
            journal.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(journal);
        }
    }

    public interface IFiscalPeriodService
    {
        Task<FiscalPeriodDto?> GetCurrentPeriodAsync();
        Task<FiscalPeriodDto?> GetByIdAsync(int id);
        Task<IEnumerable<FiscalPeriodDto>> GetByYearAsync(int year);
        Task<int> CreateAsync(FiscalPeriodCreateUpdateDto dto);
        Task ClosePeriodAsync(int periodId);
        Task OpenPeriodAsync(int periodId);
    }

    public class FiscalPeriodService : IFiscalPeriodService
    {
        private readonly IFiscalPeriodRepository _repository;
        private readonly IMapper _mapper;

        public FiscalPeriodService(IFiscalPeriodRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FiscalPeriodDto?> GetCurrentPeriodAsync()
        {
            var period = await _repository.GetCurrentPeriodAsync();
            return period == null ? null : _mapper.Map<FiscalPeriodDto>(period);
        }

        public async Task<FiscalPeriodDto?> GetByIdAsync(int id)
        {
            var period = await _repository.GetByIdAsync(id);
            return period == null ? null : _mapper.Map<FiscalPeriodDto>(period);
        }

        public async Task<IEnumerable<FiscalPeriodDto>> GetByYearAsync(int year)
        {
            var periods = await _repository.GetByYearAsync(year);
            return _mapper.Map<IEnumerable<FiscalPeriodDto>>(periods);
        }

        public async Task<int> CreateAsync(FiscalPeriodCreateUpdateDto dto)
        {
            var period = _mapper.Map<FiscalPeriod>(dto);
            period.CreatedDate = DateTime.UtcNow;

            await _repository.AddAsync(period);
            return period.FiscalPeriodId;
        }

        public async Task ClosePeriodAsync(int periodId)
        {
            var period = await _repository.GetByIdAsync(periodId);
            if (period == null)
                throw new KeyNotFoundException($"Period {periodId} not found.");

            period.PeriodStatus = "Closed";
            await _repository.UpdateAsync(period);
        }

        public async Task OpenPeriodAsync(int periodId)
        {
            var period = await _repository.GetByIdAsync(periodId);
            if (period == null)
                throw new KeyNotFoundException($"Period {periodId} not found.");

            period.PeriodStatus = "Open";
            await _repository.UpdateAsync(period);
        }
    }

    public interface IAccountBalanceService
    {
        Task<decimal> GetAccountBalanceAsync(int accountId, int? fiscalPeriodId = null);
        Task<TrialBalanceReportDto> GenerateTrialBalanceAsync(int fiscalPeriodId);
    }

    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IAccountBalanceRepository _repository;
        private readonly IJournalEntryRepository _entryRepository;
        private readonly IChartOfAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountBalanceService(IAccountBalanceRepository repository, IJournalEntryRepository entryRepository, 
            IChartOfAccountRepository accountRepository, IMapper mapper)
        {
            _repository = repository;
            _entryRepository = entryRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<decimal> GetAccountBalanceAsync(int accountId, int? fiscalPeriodId = null)
        {
            return await _repository.GetNetBalanceAsync(accountId, fiscalPeriodId);
        }

        public async Task<TrialBalanceReportDto> GenerateTrialBalanceAsync(int fiscalPeriodId)
        {
            var balances = await _repository.GetByPeriodAsync(fiscalPeriodId);
            
            var reportDto = new TrialBalanceReportDto
            {
                FiscalPeriodId = fiscalPeriodId,
                GeneratedDate = DateTime.UtcNow,
                Balances = new List<TrialBalanceDto>()
            };

            foreach (var balance in balances)
            {
                if (balance.Account == null) continue;

                reportDto.Balances.Add(new TrialBalanceDto
                {
                    AccountId = balance.AccountId,
                    AccountNumber = balance.Account.AccountNumber,
                    AccountName = balance.Account.AccountNameAr,
                    AccountType = balance.Account.AccountType,
                    DebitAmount = balance.DebitBalance,
                    CreditAmount = balance.CreditBalance
                });

                reportDto.TotalDebit += balance.DebitBalance;
                reportDto.TotalCredit += balance.CreditBalance;
            }

            return reportDto;
        }
    }

    public interface IOpeningBalanceService
    {
        Task<IEnumerable<OpeningBalanceDto>> GetByPeriodAsync(int fiscalPeriodId);
        Task<int> CreateAsync(OpeningBalanceCreateUpdateDto dto);
        Task PostOpeningBalancesAsync(int fiscalPeriodId, int userId);
    }

    public class OpeningBalanceService : IOpeningBalanceService
    {
        private readonly IOpeningBalanceRepository _repository;
        private readonly IMapper _mapper;

        public OpeningBalanceService(IOpeningBalanceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OpeningBalanceDto>> GetByPeriodAsync(int fiscalPeriodId)
        {
            var balances = await _repository.GetByPeriodAsync(fiscalPeriodId);
            return _mapper.Map<IEnumerable<OpeningBalanceDto>>(balances);
        }

        public async Task<int> CreateAsync(OpeningBalanceCreateUpdateDto dto)
        {
            var existing = await _repository.GetByAccountAndPeriodAsync(dto.AccountId, dto.FiscalPeriodId);
            if (existing != null)
                throw new InvalidOperationException("Opening balance for this account and period already exists.");

            var balance = _mapper.Map<OpeningBalance>(dto);
            await _repository.AddAsync(balance);
            return balance.OpeningBalanceId;
        }

        public async Task PostOpeningBalancesAsync(int fiscalPeriodId, int userId)
        {
            var balances = await _repository.GetDraftBalancesAsync(fiscalPeriodId);

            foreach (var balance in balances)
            {
                balance.Status = "Posted";
            }

            foreach (var balance in balances)
                await _repository.UpdateAsync(balance);
        }
    }
}
