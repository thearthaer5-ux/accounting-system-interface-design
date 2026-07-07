using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories
{
    public interface IItemMovementRepository : IGenericRepository<ItemMovement>
    {
        Task<List<ItemMovement>> GetByItemAsync(int itemId);
        Task<List<ItemMovement>> GetByWarehouseAsync(int warehouseId);
        Task<List<ItemMovement>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<List<ItemMovement>> GetUnpostedAsync();
        Task<List<ItemMovement>> GetByReferenceDocAsync(string docType, int docId);
        Task<decimal> GetMovementTotalAsync(int itemId, int warehouseId, DateTime fromDate, DateTime toDate);
    }

    public class ItemMovementRepository : GenericRepository<ItemMovement>, IItemMovementRepository
    {
        public ItemMovementRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<ItemMovement>> GetByItemAsync(int itemId)
        {
            return await _context.Set<ItemMovement>()
                .Include(im => im.Item)
                .Include(im => im.Warehouse)
                .Where(im => im.ItemId == itemId)
                .OrderByDescending(im => im.MovementDate)
                .ToListAsync();
        }

        public async Task<List<ItemMovement>> GetByWarehouseAsync(int warehouseId)
        {
            return await _context.Set<ItemMovement>()
                .Include(im => im.Item)
                .Where(im => im.WarehouseId == warehouseId || im.WarehouseIdTo == warehouseId)
                .OrderByDescending(im => im.MovementDate)
                .ToListAsync();
        }

        public async Task<List<ItemMovement>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<ItemMovement>()
                .Include(im => im.Item)
                .Include(im => im.Warehouse)
                .Where(im => im.MovementDate >= fromDate && im.MovementDate <= toDate)
                .OrderByDescending(im => im.MovementDate)
                .ToListAsync();
        }

        public async Task<List<ItemMovement>> GetUnpostedAsync()
        {
            return await _context.Set<ItemMovement>()
                .Include(im => im.Item)
                .Include(im => im.Warehouse)
                .Where(im => !im.IsPosted)
                .OrderBy(im => im.MovementDate)
                .ToListAsync();
        }

        public async Task<List<ItemMovement>> GetByReferenceDocAsync(string docType, int docId)
        {
            return await _context.Set<ItemMovement>()
                .Include(im => im.Item)
                .Where(im => im.ReferenceDocumentType == docType && im.ReferenceDocumentId == docId)
                .OrderBy(im => im.MovementDate)
                .ToListAsync();
        }

        public async Task<decimal> GetMovementTotalAsync(int itemId, int warehouseId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<ItemMovement>()
                .Where(im => im.ItemId == itemId && 
                    im.WarehouseId == warehouseId &&
                    im.MovementDate >= fromDate && 
                    im.MovementDate <= toDate)
                .SumAsync(im => im.MovementQuantity);
        }
    }
}
