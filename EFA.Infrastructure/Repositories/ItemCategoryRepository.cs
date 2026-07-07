using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories
{
    public class ItemCategoryRepository : GenericRepository<ItemCategory>, IItemCategoryRepository
    {
        public ItemCategoryRepository(DbContext context) : base(context)
        {
        }

        public async Task<ItemCategory> GetByNameAsync(string nameAr)
        {
            return await _context.Set<ItemCategory>()
                .FirstOrDefaultAsync(c => c.ItemCategoryNameAr == nameAr);
        }

        public async Task<List<ItemCategory>> GetActiveOnlyAsync()
        {
            return await _context.Set<ItemCategory>()
                .Where(c => c.IsActive)
                .OrderBy(c => c.ItemCategoryNameAr)
                .ToListAsync();
        }

        public async Task<bool> HasItemsAsync(int categoryId)
        {
            return await _context.Set<Item>()
                .AnyAsync(i => i.ItemCategoryId == categoryId);
        }

        public async Task<List<ItemCategory>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetActiveOnlyAsync();

            var term = searchTerm.ToLower();
            return await _context.Set<ItemCategory>()
                .Where(c => c.IsActive && 
                    (c.ItemCategoryNameAr.ToLower().Contains(term) ||
                     (c.ItemCategoryNameEn != null && c.ItemCategoryNameEn.ToLower().Contains(term))))
                .OrderBy(c => c.ItemCategoryNameAr)
                .ToListAsync();
        }
    }
}
