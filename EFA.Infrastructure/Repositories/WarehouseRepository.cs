using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories
{
    public interface IWarehouseRepository : IGenericRepository<Warehouse>
    {
        Task<Warehouse> GetWithBalancesAsync(int warehouseId);
        Task<List<Warehouse>> GetByBranchAsync(int branchId);
        Task<Warehouse> GetMainWarehouseAsync(int branchId);
        Task<bool> HasMovementsAsync(int warehouseId);
        Task<List<Warehouse>> SearchAsync(string searchTerm);
    }

    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(DbContext context) : base(context)
        {
        }

        public async Task<Warehouse> GetWithBalancesAsync(int warehouseId)
        {
            return await _context.Set<Warehouse>()
                .Include(w => w.Branch)
                .Include(w => w.ItemBalances)
                .ThenInclude(ib => ib.Item)
                .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId);
        }

        public async Task<List<Warehouse>> GetByBranchAsync(int branchId)
        {
            return await _context.Set<Warehouse>()
                .Include(w => w.Branch)
                .Where(w => w.BranchId == branchId && w.IsActive)
                .OrderBy(w => w.WarehouseNameAr)
                .ToListAsync();
        }

        public async Task<Warehouse> GetMainWarehouseAsync(int branchId)
        {
            return await _context.Set<Warehouse>()
                .FirstOrDefaultAsync(w => w.BranchId == branchId && w.IsMain && w.IsActive);
        }

        public async Task<bool> HasMovementsAsync(int warehouseId)
        {
            return await _context.Set<ItemMovement>()
                .AnyAsync(m => m.WarehouseId == warehouseId || m.WarehouseIdTo == warehouseId);
        }

        public async Task<List<Warehouse>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await _context.Set<Warehouse>()
                    .Where(w => w.IsActive)
                    .OrderBy(w => w.WarehouseNameAr)
                    .ToListAsync();

            var term = searchTerm.ToLower();
            return await _context.Set<Warehouse>()
                .Where(w => w.IsActive && 
                    (w.WarehouseNameAr.ToLower().Contains(term) ||
                     (w.WarehouseNameEn != null && w.WarehouseNameEn.ToLower().Contains(term))))
                .OrderBy(w => w.WarehouseNameAr)
                .ToListAsync();
        }
    }
}
