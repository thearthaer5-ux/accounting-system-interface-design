using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories
{
    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(DbContext context) : base(context)
        {
        }

        public async Task<Item> GetByCodeAsync(string code)
        {
            return await _context.Set<Item>()
                .Include(i => i.ItemCategory)
                .Include(i => i.ItemUnits)
                .FirstOrDefaultAsync(i => i.ItemCode == code);
        }

        public async Task<Item> GetWithCategoryAsync(int itemId)
        {
            return await _context.Set<Item>()
                .Include(i => i.ItemCategory)
                .FirstOrDefaultAsync(i => i.ItemId == itemId);
        }

        public async Task<List<Item>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Set<Item>()
                .Include(i => i.ItemCategory)
                .Where(i => i.ItemCategoryId == categoryId && i.IsActive)
                .OrderBy(i => i.ItemNameAr)
                .ToListAsync();
        }

        public async Task<List<Item>> GetActiveOnlyAsync()
        {
            return await _context.Set<Item>()
                .Include(i => i.ItemCategory)
                .Where(i => i.IsActive)
                .OrderBy(i => i.ItemNameAr)
                .ToListAsync();
        }

        public async Task<List<Item>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetActiveOnlyAsync();

            var term = searchTerm.ToLower();
            return await _context.Set<Item>()
                .Include(i => i.ItemCategory)
                .Where(i => i.IsActive && 
                    (i.ItemCode.ToLower().Contains(term) ||
                     i.ItemNameAr.ToLower().Contains(term) ||
                     (i.ItemNameEn != null && i.ItemNameEn.ToLower().Contains(term))))
                .OrderBy(i => i.ItemNameAr)
                .ToListAsync();
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _context.Set<Item>().Where(i => i.ItemCode == code);
            if (excludeId.HasValue)
                query = query.Where(i => i.ItemId != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<List<Item>> GetLowStockItemsAsync()
        {
            return await _context.Set<Item>()
                .Include(i => i.ItemBalances)
                .Where(i => i.IsActive && 
                    i.ItemBalances.Any(ib => ib.BalanceQuantity <= i.MinimumQuantity))
                .OrderBy(i => i.ItemNameAr)
                .ToListAsync();
        }
    }
}
