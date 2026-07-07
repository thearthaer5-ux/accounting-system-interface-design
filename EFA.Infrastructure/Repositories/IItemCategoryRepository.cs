using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories
{
    public interface IItemCategoryRepository : IGenericRepository<ItemCategory>
    {
        Task<ItemCategory> GetByNameAsync(string nameAr);
        Task<List<ItemCategory>> GetActiveOnlyAsync();
        Task<bool> HasItemsAsync(int categoryId);
        Task<List<ItemCategory>> SearchAsync(string searchTerm);
    }
}
