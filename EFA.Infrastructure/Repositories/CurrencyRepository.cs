using EFA.Domain.Entities;
using EFA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories;

public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
{
    private readonly EFADbContext _context;

    public CurrencyRepository(EFADbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Currency?> GetByCodeAsync(string code)
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.CurrencyCode == code);
    }

    public async Task<Currency?> GetDefaultCurrencyAsync()
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.IsDefault && c.IsActive);
    }

    public async Task<IEnumerable<Currency>> GetActiveCurrenciesAsync()
    {
        return await _context.Currencies
            .Where(c => c.IsActive)
            .OrderBy(c => c.CurrencyName)
            .ToListAsync();
    }

    public async Task UpdateExchangeRateAsync(int currencyId, decimal rate)
    {
        var currency = await _context.Currencies.FindAsync(currencyId);
        if (currency != null)
        {
            currency.ExchangeRate = rate;
            currency.LastRateUpdate = DateTime.UtcNow;
            _context.Currencies.Update(currency);
            await _context.SaveChangesAsync();
        }
    }
}
