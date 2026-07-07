using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories
{
    public interface IItemRepository : IGenericRepository<Item>
    {
        Task<Item> GetByCodeAsync(string code);
        Task<Item> GetWithCategoryAsync(int itemId);
        Task<List<Item>> GetByCategoryAsync(int categoryId);
        Task<List<Item>> GetActiveOnlyAsync();
        Task<List<Item>> SearchAsync(string searchTerm);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
        Task<List<Item>> GetLowStockItemsAsync();
    }
}
