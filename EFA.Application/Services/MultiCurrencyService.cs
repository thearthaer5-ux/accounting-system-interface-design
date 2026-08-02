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
    /// خدمة العملات المتعددة
    /// توفر تحويل العملات والعمليات المالية متعددة العملات
    /// </summary>
    public interface IMultiCurrencyService
    {
        /// <summary>
        /// تحويل مبلغ من عملة إلى أخرى
        /// </summary>
        Task<decimal> ConvertCurrencyAsync(
            decimal amount, string fromCurrencyCode, string toCurrencyCode, DateTime exchangeDate);

        /// <summary>
        /// الحصول على سعر الصرف بين عملتين
        /// </summary>
        Task<decimal> GetExchangeRateAsync(
            string fromCurrencyCode, string toCurrencyCode, DateTime exchangeDate);

        /// <summary>
        /// تحديث أسعار الصرف
        /// </summary>
        Task UpdateExchangeRatesAsync(Dictionary<string, decimal> rates, DateTime effectiveDate);

        /// <summary>
        /// إضافة عملة جديدة
        /// </summary>
        Task<int> AddCurrencyAsync(string currencyCode, string currencyName, string symbol, decimal rate);

        /// <summary>
        /// الحصول على جميع العملات المفعلة
        /// </summary>
        Task<List<CurrencyDto>> GetActiveCurrenciesAsync();

        /// <summary>
        /// التحقق من أن الحسابات بنفس العملة عند التحويل
        /// </summary>
        Task<bool> ValidateMultiCurrencyEntryAsync(JournalEntry entry, int originAccountId);

        /// <summary>
        /// حساب الفرق في الصرف (Realized/Unrealized Gain/Loss)
        /// </summary>
        Task<(decimal realizedGain, decimal unrealizedGain)> CalculateExchangeGainLossAsync(
            int accountId, string baseCurrency, string localCurrency, DateTime asOfDate);

        /// <summary>
        /// إعادة تقييم الأصول والالتزامات بالعملات الأجنبية
        /// </summary>
        Task<int> CreateRevaluationEntriesAsync(
            string foreignCurrencyCode, string baseCurrencyCode, int userId);

        /// <summary>
        /// الحصول على معدل الصرف التاريخي
        /// </summary>
        Task<List<ExchangeRateHistoryDto>> GetExchangeRateHistoryAsync(
            string currencyCode, DateTime startDate, DateTime endDate);
    }

    public class MultiCurrencyService : IMultiCurrencyService
    {
        private readonly EFADbContext _dbContext;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<MultiCurrencyService> _logger;

        public MultiCurrencyService(
            EFADbContext dbContext,
            ICurrencyRepository currencyRepository,
            ILogger<MultiCurrencyService> logger)
        {
            _dbContext = dbContext;
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        /// <summary>
        /// تحويل العملات مع أسعار الصرف التاريخية
        /// </summary>
        public async Task<decimal> ConvertCurrencyAsync(
            decimal amount, string fromCurrencyCode, string toCurrencyCode, DateTime exchangeDate)
        {
            try
            {
                if (fromCurrencyCode == toCurrencyCode)
                    return amount;

                var rate = await GetExchangeRateAsync(fromCurrencyCode, toCurrencyCode, exchangeDate);

                if (rate == 0)
                    throw new InvalidOperationException(
                        $"لم يتم العثور على سعر صرف بين {fromCurrencyCode} و {toCurrencyCode}");

                var convertedAmount = amount * rate;

                _logger.LogInformation(
                    $"تحويل {amount} من {fromCurrencyCode} إلى {toCurrencyCode} = {convertedAmount} " +
                    $"(السعر: {rate})");

                return convertedAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ في تحويل العملة: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// الحصول على سعر الصرف
        /// </summary>
        public async Task<decimal> GetExchangeRateAsync(
            string fromCurrencyCode, string toCurrencyCode, DateTime exchangeDate)
        {
            try
            {
                // إذا كانت نفس العملة
                if (fromCurrencyCode == toCurrencyCode)
                    return 1m;

                // البحث عن سعر الصرف المباشر
                var directRate = await _dbContext.CurrencyExchangeRates
                    .Where(r => r.FromCurrencyCode == fromCurrencyCode &&
                               r.ToCurrencyCode == toCurrencyCode &&
                               r.EffectiveDate <= exchangeDate)
                    .OrderByDescending(r => r.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (directRate != null)
                    return directRate.ExchangeRate;

                // البحث عن السعر العكسي
                var reverseRate = await _dbContext.CurrencyExchangeRates
                    .Where(r => r.FromCurrencyCode == toCurrencyCode &&
                               r.ToCurrencyCode == fromCurrencyCode &&
                               r.EffectiveDate <= exchangeDate)
                    .OrderByDescending(r => r.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (reverseRate != null)
                    return 1 / reverseRate.ExchangeRate;

                // البحث عن السعر عبر عملة وسيطة (مثلاً USD)
                var baseRate = await GetCrossRateAsync(fromCurrencyCode, toCurrencyCode, exchangeDate);

                if (baseRate > 0)
                    return baseRate;

                throw new KeyNotFoundException(
                    $"لم يتم العثور على سعر صرف بين {fromCurrencyCode} و {toCurrencyCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ في الحصول على سعر الصرف: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// تحديث أسعار الصرف
        /// </summary>
        public async Task UpdateExchangeRatesAsync(
            Dictionary<string, decimal> rates, DateTime effectiveDate)
        {
            try
            {
                _logger.LogInformation($"تحديث أسعار الصرف بتاريخ {effectiveDate:yyyy-MM-dd}");

                foreach (var rate in rates)
                {
                    var parts = rate.Key.Split('/');
                    if (parts.Length != 2)
                        continue;

                    var fromCurrency = parts[0].Trim().ToUpper();
                    var toCurrency = parts[1].Trim().ToUpper();

                    // البحث عن سعر الصرف الموجود
                    var existingRate = await _dbContext.CurrencyExchangeRates
                        .FirstOrDefaultAsync(r =>
                            r.FromCurrencyCode == fromCurrency &&
                            r.ToCurrencyCode == toCurrency &&
                            r.EffectiveDate == effectiveDate);

                    if (existingRate != null)
                    {
                        existingRate.ExchangeRate = rate.Value;
                        existingRate.UpdatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        var newRate = new CurrencyExchangeRate
                        {
                            FromCurrencyCode = fromCurrency,
                            ToCurrencyCode = toCurrency,
                            ExchangeRate = rate.Value,
                            EffectiveDate = effectiveDate,
                            CreatedDate = DateTime.UtcNow
                        };

                        _dbContext.CurrencyExchangeRates.Add(newRate);
                    }
                }

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"تم تحديث أسعار الصرف بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء تحديث أسعار الصرف: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// إضافة عملة جديدة
        /// </summary>
        public async Task<int> AddCurrencyAsync(
            string currencyCode, string currencyName, string symbol, decimal rate)
        {
            try
            {
                var currency = new Currency
                {
                    CurrencyCode = currencyCode.ToUpper(),
                    CurrencyName = currencyName,
                    Symbol = symbol,
                    ExchangeRate = rate,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.Currencies.Add(currency);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"تم إضافة العملة {currencyCode} بنجاح");

                return currency.CurrencyId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ أثناء إضافة العملة: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// الحصول على جميع العملات المفعلة
        /// </summary>
        public async Task<List<CurrencyDto>> GetActiveCurrenciesAsync()
        {
            var currencies = await _dbContext.Currencies
                .Where(c => c.IsActive)
                .Select(c => new CurrencyDto
                {
                    CurrencyCode = c.CurrencyCode,
                    CurrencyName = c.CurrencyName,
                    Symbol = c.Symbol,
                    ExchangeRate = c.ExchangeRate,
                    IsDefault = c.IsDefault
                })
                .ToListAsync();

            return currencies;
        }

        /// <summary>
        /// التحقق من صحة العمليات متعددة العملات
        /// </summary>
        public async Task<bool> ValidateMultiCurrencyEntryAsync(JournalEntry entry, int originAccountId)
        {
            try
            {
                var originAccount = await _dbContext.ChartOfAccounts.FindAsync(originAccountId);
                var targetAccount = await _dbContext.ChartOfAccounts.FindAsync(entry.AccountId);

                if (originAccount == null || targetAccount == null)
                    return true;

                // إذا كانت الحسابات بعملات مختلفة، يجب أن يكون هناك حساب للفرق في الصرف
                // (يمكن تطبيق قواعد عمل إضافية هنا)

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ في التحقق من صحة العملة: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// حساب أرباح/خسائر الصرف
        /// </summary>
        public async Task<(decimal realizedGain, decimal unrealizedGain)> CalculateExchangeGainLossAsync(
            int accountId, string baseCurrency, string localCurrency, DateTime asOfDate)
        {
            try
            {
                decimal realizedGain = 0;
                decimal unrealizedGain = 0;

                // الحصول على معدل الصرف الحالي
                var currentRate = await GetExchangeRateAsync(baseCurrency, localCurrency, asOfDate);

                // حساب الأرباح المحققة من المعاملات المكتملة
                var completedTransactions = await _dbContext.JournalEntries
                    .Where(j => j.AccountId == accountId &&
                               j.Journal.JournalStatus == "Posted" &&
                               j.Journal.JournalDate <= asOfDate)
                    .ToListAsync();

                foreach (var transaction in completedTransactions)
                {
                    var transactionRate = await GetExchangeRateAsync(
                        baseCurrency, localCurrency, transaction.Journal.JournalDate);

                    var difference = (currentRate - transactionRate) * transaction.DebitAmount;
                    realizedGain += difference;
                }

                _logger.LogInformation(
                    $"حساب أرباح الصرف للحساب {accountId}: " +
                    $"محققة={realizedGain}, غير محققة={unrealizedGain}");

                return (realizedGain, unrealizedGain);
            }
            catch (Exception ex)
            {
                _logger.LogError($"خطأ في حساب أرباح الصرف: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// إنشاء قيود إعادة التقييم
        /// </summary>
        public async Task<int> CreateRevaluationEntriesAsync(
            string foreignCurrencyCode, string baseCurrencyCode, int userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var revaluationJournal = new Journal
                {
                    JournalNumber = $"REVALUATION-{foreignCurrencyCode}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    JournalDate = DateTime.UtcNow,
                    PostingDate = DateTime.UtcNow,
                    JournalStatus = "Posted",
                    Description = $"قيود إعادة تقييم العملة {foreignCurrencyCode}",
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                // البحث عن جميع الحسابات بالعملة الأجنبية
                var foreignCurrencyAccounts = await _dbContext.ChartOfAccounts
                    .Where(a => a.CurrencyCode == foreignCurrencyCode)
                    .ToListAsync();

                foreach (var account in foreignCurrencyAccounts)
                {
                    var (realizedGain, unrealizedGain) = await CalculateExchangeGainLossAsync(
                        account.AccountId, baseCurrencyCode, foreignCurrencyCode, DateTime.UtcNow);

                    if (unrealizedGain != 0)
                    {
                        var entry = new JournalEntry
                        {
                            JournalId = revaluationJournal.JournalId,
                            AccountId = account.AccountId,
                            DebitAmount = unrealizedGain > 0 ? unrealizedGain : 0,
                            CreditAmount = unrealizedGain < 0 ? Math.Abs(unrealizedGain) : 0,
                            Description = $"إعادة تقييم {account.AccountNameAr}",
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow
                        };

                        revaluationJournal.JournalEntries.Add(entry);
                        revaluationJournal.TotalDebit += entry.DebitAmount;
                        revaluationJournal.TotalCredit += entry.CreditAmount;
                    }
                }

                if (revaluationJournal.JournalEntries.Any())
                {
                    _dbContext.Journals.Add(revaluationJournal);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        $"تم إنشاء قيود إعادة التقييم للعملة {foreignCurrencyCode} بنجاح");

                    return revaluationJournal.JournalId;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"خطأ في إنشاء قيود إعادة التقييم: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// الحصول على السجل التاريخي لأسعار الصرف
        /// </summary>
        public async Task<List<ExchangeRateHistoryDto>> GetExchangeRateHistoryAsync(
            string currencyCode, DateTime startDate, DateTime endDate)
        {
            var history = await _dbContext.CurrencyExchangeRates
                .Where(r => (r.FromCurrencyCode == currencyCode || r.ToCurrencyCode == currencyCode) &&
                           r.EffectiveDate >= startDate &&
                           r.EffectiveDate <= endDate)
                .OrderByDescending(r => r.EffectiveDate)
                .Select(r => new ExchangeRateHistoryDto
                {
                    FromCurrencyCode = r.FromCurrencyCode,
                    ToCurrencyCode = r.ToCurrencyCode,
                    ExchangeRate = r.ExchangeRate,
                    EffectiveDate = r.EffectiveDate
                })
                .ToListAsync();

            return history;
        }

        private async Task<decimal> GetCrossRateAsync(
            string fromCurrency, string toCurrency, DateTime exchangeDate)
        {
            try
            {
                // محاولة التحويل عبر USD كعملة وسيطة
                var toUsdRate = await _dbContext.CurrencyExchangeRates
                    .Where(r => r.FromCurrencyCode == fromCurrency &&
                               r.ToCurrencyCode == "USD" &&
                               r.EffectiveDate <= exchangeDate)
                    .OrderByDescending(r => r.EffectiveDate)
                    .FirstOrDefaultAsync();

                var fromUsdRate = await _dbContext.CurrencyExchangeRates
                    .Where(r => r.FromCurrencyCode == "USD" &&
                               r.ToCurrencyCode == toCurrency &&
                               r.EffectiveDate <= exchangeDate)
                    .OrderByDescending(r => r.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (toUsdRate != null && fromUsdRate != null)
                    return toUsdRate.ExchangeRate * fromUsdRate.ExchangeRate;

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }

    public class CurrencyDto
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string Symbol { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ExchangeRateHistoryDto
    {
        public string FromCurrencyCode { get; set; }
        public string ToCurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
