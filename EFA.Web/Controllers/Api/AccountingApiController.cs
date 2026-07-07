using EFA.Application.DTOs;
using EFA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EFA.Web.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountingApiController : ControllerBase
    {
        private readonly IChartOfAccountService _chartService;
        private readonly IJournalService _journalService;
        private readonly IFiscalPeriodService _periodService;
        private readonly IAccountBalanceService _balanceService;
        private readonly IOpeningBalanceService _openingBalanceService;

        public AccountingApiController(IChartOfAccountService chartService, IJournalService journalService,
            IFiscalPeriodService periodService, IAccountBalanceService balanceService,
            IOpeningBalanceService openingBalanceService)
        {
            _chartService = chartService;
            _journalService = journalService;
            _periodService = periodService;
            _balanceService = balanceService;
            _openingBalanceService = openingBalanceService;
        }

        // Chart of Accounts endpoints
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<ChartOfAccountDto>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _chartService.GetAllAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("accounts/{id}")]
        public async Task<ActionResult<ChartOfAccountDto>> GetAccount(int id)
        {
            try
            {
                var account = await _chartService.GetByIdAsync(id);
                if (account == null)
                    return NotFound();

                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("accounts/number/{accountNumber}")]
        public async Task<ActionResult<ChartOfAccountDto>> GetAccountByNumber(string accountNumber)
        {
            try
            {
                var account = await _chartService.GetByAccountNumberAsync(accountNumber);
                if (account == null)
                    return NotFound();

                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("accounts/type/{type}")]
        public async Task<ActionResult<IEnumerable<ChartOfAccountDto>>> GetAccountsByType(string type)
        {
            try
            {
                var accounts = await _chartService.GetByTypeAsync(type);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("accounts/hierarchy")]
        public async Task<ActionResult<IEnumerable<ChartOfAccountHierarchyDto>>> GetAccountHierarchy()
        {
            try
            {
                var hierarchy = await _chartService.GetHierarchyAsync();
                return Ok(hierarchy);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("accounts")]
        public async Task<ActionResult<int>> CreateAccount([FromBody] ChartOfAccountCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = await _chartService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAccount), new { id }, id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("accounts/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] ChartOfAccountCreateUpdateDto dto)
        {
            try
            {
                await _chartService.UpdateAsync(id, dto);
                return Ok(new { message = "تم تحديث الحساب بنجاح" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Journals endpoints
        [HttpGet("journals")]
        public async Task<ActionResult<IEnumerable<JournalDto>>> GetJournals(string? status = null)
        {
            try
            {
                IEnumerable<JournalDto> journals;
                if (!string.IsNullOrEmpty(status))
                    journals = await _journalService.GetByStatusAsync(status);
                else
                    journals = await _journalService.GetByStatusAsync("Draft");

                return Ok(journals);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("journals/{id}")]
        public async Task<ActionResult<JournalDto>> GetJournal(int id)
        {
            try
            {
                var journal = await _journalService.GetByIdAsync(id);
                if (journal == null)
                    return NotFound();

                return Ok(journal);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("journals/by-period/{periodId}")]
        public async Task<ActionResult<IEnumerable<JournalDto>>> GetJournalsByPeriod(int periodId)
        {
            try
            {
                var journals = await _journalService.GetByPeriodAsync(periodId);
                return Ok(journals);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("journals")]
        public async Task<ActionResult<int>> CreateJournal([FromBody] JournalCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var id = await _journalService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetJournal), new { id }, id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("journals/{id}/post")]
        public async Task<IActionResult> PostJournal(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _journalService.PostJournalAsync(id, userId);
                return Ok(new { message = "تم ترحيل اليومية بنجاح" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("journals/{id}/reverse")]
        public async Task<IActionResult> ReverseJournal(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _journalService.ReverseJournalAsync(id, userId);
                return Ok(new { message = "تم عكس اليومية بنجاح" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Trial Balance endpoint
        [HttpGet("reports/trial-balance")]
        public async Task<ActionResult<TrialBalanceReportDto>> GetTrialBalance(int? periodId = null)
        {
            try
            {
                if (!periodId.HasValue)
                {
                    var period = await _periodService.GetCurrentPeriodAsync();
                    if (period != null)
                        periodId = period.FiscalPeriodId;
                    else
                        return BadRequest("لا توجد فترة مالية مفتوحة");
                }

                var report = await _balanceService.GenerateTrialBalanceAsync(periodId.Value);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Fiscal Periods endpoints
        [HttpGet("periods/current")]
        public async Task<ActionResult<FiscalPeriodDto>> GetCurrentPeriod()
        {
            try
            {
                var period = await _periodService.GetCurrentPeriodAsync();
                if (period == null)
                    return NotFound();

                return Ok(period);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("periods/by-year/{year}")]
        public async Task<ActionResult<IEnumerable<FiscalPeriodDto>>> GetPeriodsByYear(int year)
        {
            try
            {
                var periods = await _periodService.GetByYearAsync(year);
                return Ok(periods);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("periods")]
        public async Task<ActionResult<int>> CreatePeriod([FromBody] FiscalPeriodCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = await _periodService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetCurrentPeriod), new { id }, id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("periods/{id}/close")]
        public async Task<IActionResult> ClosePeriod(int id)
        {
            try
            {
                await _periodService.ClosePeriodAsync(id);
                return Ok(new { message = "تم إغلاق الفترة المالية بنجاح" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Opening Balances endpoints
        [HttpGet("opening-balances/by-period/{periodId}")]
        public async Task<ActionResult<IEnumerable<OpeningBalanceDto>>> GetOpeningBalancesByPeriod(int periodId)
        {
            try
            {
                var balances = await _openingBalanceService.GetByPeriodAsync(periodId);
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("opening-balances")]
        public async Task<ActionResult<int>> CreateOpeningBalance([FromBody] OpeningBalanceCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = await _openingBalanceService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetOpeningBalancesByPeriod), new { periodId = dto.FiscalPeriodId }, id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("opening-balances/{periodId}/post")]
        public async Task<IActionResult> PostOpeningBalances(int periodId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _openingBalanceService.PostOpeningBalancesAsync(periodId, userId);
                return Ok(new { message = "تم ترحيل الأرصدة الافتتاحية بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
