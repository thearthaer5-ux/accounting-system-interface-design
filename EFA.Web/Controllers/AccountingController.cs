using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFA.Web.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        private readonly IChartOfAccountService _chartService;
        private readonly IJournalService _journalService;
        private readonly IFiscalPeriodService _periodService;
        private readonly IAccountBalanceService _balanceService;
        private readonly ILogger<AccountingController> _logger;

        public AccountingController(IChartOfAccountService chartService, IJournalService journalService,
            IFiscalPeriodService periodService, IAccountBalanceService balanceService,
            ILogger<AccountingController> logger)
        {
            _chartService = chartService;
            _journalService = journalService;
            _periodService = periodService;
            _balanceService = balanceService;
            _logger = logger;
        }

        // Chart of Accounts
        public async Task<IActionResult> ChartOfAccounts()
        {
            try
            {
                var accounts = await _chartService.GetAllAsync();
                return View(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading chart of accounts: {ex.Message}");
                return BadRequest("خطأ في تحميل شجرة الحسابات");
            }
        }

        public async Task<IActionResult> AccountDetails(int id)
        {
            try
            {
                var account = await _chartService.GetByIdAsync(id);
                if (account == null)
                    return NotFound();

                return View(account);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading account details: {ex.Message}");
                return BadRequest("خطأ في تحميل بيانات الحساب");
            }
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(ChartOfAccountCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _chartService.CreateAsync(dto);
                
                return RedirectToAction(nameof(ChartOfAccounts));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating account: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        // Journals
        public async Task<IActionResult> Journals()
        {
            try
            {
                var journals = await _journalService.GetByStatusAsync("Draft");
                return View(journals);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading journals: {ex.Message}");
                return BadRequest("خطأ في تحميل اليوميات");
            }
        }

        public async Task<IActionResult> JournalDetails(int id)
        {
            try
            {
                var journal = await _journalService.GetByIdAsync(id);
                if (journal == null)
                    return NotFound();

                return View(journal);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading journal details: {ex.Message}");
                return BadRequest("خطأ في تحميل بيانات اليومية");
            }
        }

        [HttpGet]
        public IActionResult CreateJournal()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJournal(JournalCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _journalService.CreateAsync(dto, userId);
                
                return RedirectToAction(nameof(Journals));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating journal: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostJournal(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _journalService.PostJournalAsync(id, userId);
                
                return RedirectToAction(nameof(JournalDetails), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error posting journal: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // Trial Balance Report
        public async Task<IActionResult> TrialBalance(int? periodId)
        {
            try
            {
                if (!periodId.HasValue)
                {
                    var period = await _periodService.GetCurrentPeriodAsync();
                    if (period != null)
                        periodId = period.FiscalPeriodId;
                }

                if (!periodId.HasValue)
                    return BadRequest("لا توجد فترة مالية مفتوحة");

                var report = await _balanceService.GenerateTrialBalanceAsync(periodId.Value);
                return View(report);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating trial balance: {ex.Message}");
                return BadRequest("خطأ في توليد ميزان المراجعة");
            }
        }

        // Fiscal Periods
        public async Task<IActionResult> FiscalPeriods()
        {
            try
            {
                var periods = await _periodService.GetByYearAsync(DateTime.Now.Year);
                return View(periods);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading fiscal periods: {ex.Message}");
                return BadRequest("خطأ في تحميل الفترات المالية");
            }
        }
    }
}
