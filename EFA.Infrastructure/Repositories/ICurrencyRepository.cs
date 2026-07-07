using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories;

public interface ICurrencyRepository : IGenericRepository<Currency>
{
    Task<Currency?> GetByCodeAsync(string code);
    Task<Currency?> GetDefaultCurrencyAsync();
    Task<IEnumerable<Currency>> GetActiveCurrenciesAsync();
    Task UpdateExchangeRateAsync(int currencyId, decimal rate);
}
