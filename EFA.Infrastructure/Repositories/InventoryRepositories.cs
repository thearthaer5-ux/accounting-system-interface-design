using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories
{
    public interface IItemBatchRepository : IGenericRepository<ItemBatch>
    {
        Task<ItemBatch> GetByBatchNumberAsync(string batchNumber);
        Task<List<ItemBatch>> GetByItemAsync(int itemId);
        Task<List<ItemBatch>> GetExpiredBatchesAsync();
        Task<List<ItemBatch>> GetAvailableBatchesAsync(int itemId);
    }

    public interface IInventoryCountRepository : IGenericRepository<InventoryCount>
    {
        Task<InventoryCount> GetWithDetailsAsync(int countId);
        Task<List<InventoryCount>> GetByWarehouseAsync(int warehouseId);
        Task<List<InventoryCount>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<bool> HasDetailsAsync(int countId);
    }

    public class ItemBatchRepository : GenericRepository<ItemBatch>, IItemBatchRepository
    {
        public ItemBatchRepository(DbContext context) : base(context)
        {
        }

        public async Task<ItemBatch> GetByBatchNumberAsync(string batchNumber)
        {
            return await _context.Set<ItemBatch>()
                .Include(ib => ib.Item)
                .FirstOrDefaultAsync(ib => ib.BatchNumber == batchNumber);
        }

        public async Task<List<ItemBatch>> GetByItemAsync(int itemId)
        {
            return await _context.Set<ItemBatch>()
                .Where(ib => ib.ItemId == itemId && ib.IsAvailable)
                .OrderBy(ib => ib.ExpiryDate)
                .ToListAsync();
        }

        public async Task<List<ItemBatch>> GetExpiredBatchesAsync()
        {
            var today = DateTime.Now;
            return await _context.Set<ItemBatch>()
                .Include(ib => ib.Item)
                .Where(ib => ib.ExpiryDate.HasValue && ib.ExpiryDate < today && ib.IsAvailable)
                .OrderBy(ib => ib.ExpiryDate)
                .ToListAsync();
        }

        public async Task<List<ItemBatch>> GetAvailableBatchesAsync(int itemId)
        {
            var today = DateTime.Now;
            return await _context.Set<ItemBatch>()
                .Where(ib => ib.ItemId == itemId && 
                    ib.IsAvailable && 
                    ib.BatchQuantity > 0 &&
                    (!ib.ExpiryDate.HasValue || ib.ExpiryDate >= today))
                .OrderBy(ib => ib.ExpiryDate)
                .ToListAsync();
        }
    }

    public class InventoryCountRepository : GenericRepository<InventoryCount>, IInventoryCountRepository
    {
        public InventoryCountRepository(DbContext context) : base(context)
        {
        }

        public async Task<InventoryCount> GetWithDetailsAsync(int countId)
        {
            return await _context.Set<InventoryCount>()
                .Include(ic => ic.Warehouse)
                .Include(ic => ic.InventoryCountDetails)
                .ThenInclude(icd => icd.Item)
                .FirstOrDefaultAsync(ic => ic.InventoryCountId == countId);
        }

        public async Task<List<InventoryCount>> GetByWarehouseAsync(int warehouseId)
        {
            return await _context.Set<InventoryCount>()
                .Include(ic => ic.Warehouse)
                .Where(ic => ic.WarehouseId == warehouseId)
                .OrderByDescending(ic => ic.CountDate)
                .ToListAsync();
        }

        public async Task<List<InventoryCount>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<InventoryCount>()
                .Include(ic => ic.Warehouse)
                .Where(ic => ic.CountDate >= fromDate && ic.CountDate <= toDate)
                .OrderByDescending(ic => ic.CountDate)
                .ToListAsync();
        }

        public async Task<bool> HasDetailsAsync(int countId)
        {
            return await _context.Set<InventoryCountDetail>()
                .AnyAsync(icd => icd.InventoryCountId == countId);
        }
    }
}
