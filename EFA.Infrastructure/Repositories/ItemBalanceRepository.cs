using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories
{
    public interface IItemBalanceRepository : IGenericRepository<ItemBalance>
    {
        Task<ItemBalance> GetByItemAndWarehouseAsync(int itemId, int warehouseId);
        Task<List<ItemBalance>> GetByWarehouseAsync(int warehouseId);
        Task<List<ItemBalance>> GetByItemAsync(int itemId);
        Task<decimal> GetTotalValueAsync(int warehouseId);
        Task<bool> ExistsAsync(int itemId, int warehouseId);
    }

    public class ItemBalanceRepository : GenericRepository<ItemBalance>, IItemBalanceRepository
    {
        public ItemBalanceRepository(DbContext context) : base(context)
        {
        }

        public async Task<ItemBalance> GetByItemAndWarehouseAsync(int itemId, int warehouseId)
        {
            return await _context.Set<ItemBalance>()
                .Include(ib => ib.Item)
                .Include(ib => ib.Warehouse)
                .FirstOrDefaultAsync(ib => ib.ItemId == itemId && ib.WarehouseId == warehouseId);
        }

        public async Task<List<ItemBalance>> GetByWarehouseAsync(int warehouseId)
        {
            return await _context.Set<ItemBalance>()
                .Include(ib => ib.Item)
                .Where(ib => ib.WarehouseId == warehouseId && ib.BalanceQuantity > 0)
                .OrderBy(ib => ib.Item.ItemNameAr)
                .ToListAsync();
        }

        public async Task<List<ItemBalance>> GetByItemAsync(int itemId)
        {
            return await _context.Set<ItemBalance>()
                .Include(ib => ib.Warehouse)
                .Where(ib => ib.ItemId == itemId && ib.BalanceQuantity > 0)
                .OrderBy(ib => ib.Warehouse.WarehouseNameAr)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalValueAsync(int warehouseId)
        {
            return await _context.Set<ItemBalance>()
                .Where(ib => ib.WarehouseId == warehouseId)
                .SumAsync(ib => ib.BalanceQuantity * ib.AverageCost);
        }

        public async Task<bool> ExistsAsync(int itemId, int warehouseId)
        {
            return await _context.Set<ItemBalance>()
                .AnyAsync(ib => ib.ItemId == itemId && ib.WarehouseId == warehouseId);
        }
    }
}
