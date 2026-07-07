using AutoMapper;
using EFA.Application.DTOs;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;

namespace EFA.Application.Services
{
    public interface IWarehouseService
    {
        Task<WarehouseDto> GetByIdAsync(int id);
        Task<List<WarehouseDto>> GetByBranchAsync(int branchId);
        Task<WarehouseDto> GetMainWarehouseAsync(int branchId);
        Task<List<WarehouseDto>> GetAllAsync();
        Task<WarehouseDto> CreateAsync(WarehouseCreateUpdateDto dto, int userId);
        Task<WarehouseDto> UpdateAsync(int id, WarehouseCreateUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id);
        Task<WarehouseInventorySummaryDto> GetInventorySummaryAsync(int warehouseId);
    }

    public interface IInventoryService
    {
        Task<ItemBalanceDto> GetBalanceAsync(int itemId, int warehouseId);
        Task<List<ItemBalanceDto>> GetWarehouseBalancesAsync(int warehouseId);
        Task<List<ItemBalanceDto>> GetItemBalancesAsync(int itemId);
        Task<decimal> GetWarehouseValueAsync(int warehouseId);
        Task<ItemBalanceDto> AddMovementAsync(ItemMovementCreateDto dto, int userId);
        Task<ItemMovementDto> GetMovementAsync(int movementId);
        Task<List<ItemMovementDto>> GetMovementsAsync(DateTime fromDate, DateTime toDate);
        Task<bool> PostMovementAsync(int movementId);
    }

    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;

        public WarehouseService(IWarehouseRepository warehouseRepository, IBranchRepository branchRepository, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _branchRepository = branchRepository;
            _mapper = mapper;
        }

        public async Task<WarehouseDto> GetByIdAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                throw new ArgumentException("المستودع غير موجود");

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<List<WarehouseDto>> GetByBranchAsync(int branchId)
        {
            var warehouses = await _warehouseRepository.GetByBranchAsync(branchId);
            return _mapper.Map<List<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto> GetMainWarehouseAsync(int branchId)
        {
            var warehouse = await _warehouseRepository.GetMainWarehouseAsync(branchId);
            if (warehouse == null)
                throw new ArgumentException("لا يوجد مستودع رئيسي للفرع");

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            return _mapper.Map<List<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto> CreateAsync(WarehouseCreateUpdateDto dto, int userId)
        {
            var branch = await _branchRepository.GetByIdAsync(dto.BranchId);
            if (branch == null)
                throw new ArgumentException("الفرع المحدد غير موجود");

            var warehouse = _mapper.Map<Warehouse>(dto);
            warehouse.CreatedBy = userId;
            warehouse.IsActive = true;

            await _warehouseRepository.AddAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<WarehouseDto> UpdateAsync(int id, WarehouseCreateUpdateDto dto, int userId)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                throw new ArgumentException("المستودع غير موجود");

            _mapper.Map(dto, warehouse);
            warehouse.ModifiedBy = userId;
            warehouse.ModifiedDate = DateTime.Now;

            await _warehouseRepository.UpdateAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                return false;

            if (await _warehouseRepository.HasMovementsAsync(id))
                throw new InvalidOperationException("لا يمكن حذف مستودع يحتوي على حركات");

            warehouse.IsActive = false;
            warehouse.ModifiedDate = DateTime.Now;

            await _warehouseRepository.UpdateAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();

            return true;
        }

        public async Task<WarehouseInventorySummaryDto> GetInventorySummaryAsync(int warehouseId)
        {
            var warehouse = await _warehouseRepository.GetWithBalancesAsync(warehouseId);
            if (warehouse == null)
                throw new ArgumentException("المستودع غير موجود");

            var totalValue = warehouse.ItemBalances.Sum(ib => ib.BalanceQuantity * ib.AverageCost);
            var lowStockCount = warehouse.ItemBalances.Count(ib => ib.BalanceQuantity <= ib.Item.MinimumQuantity);

            return new WarehouseInventorySummaryDto
            {
                WarehouseId = warehouse.WarehouseId,
                WarehouseName = warehouse.WarehouseNameAr,
                TotalItems = warehouse.ItemBalances.Count,
                TotalQuantity = warehouse.ItemBalances.Sum(ib => ib.BalanceQuantity),
                TotalValue = totalValue,
                LowStockItems = lowStockCount
            };
        }
    }

    public class InventoryService : IInventoryService
    {
        private readonly IItemBalanceRepository _balanceRepository;
        private readonly IItemMovementRepository _movementRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public InventoryService(IItemBalanceRepository balanceRepository, IItemMovementRepository movementRepository, 
            IItemRepository itemRepository, IMapper mapper)
        {
            _balanceRepository = balanceRepository;
            _movementRepository = movementRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<ItemBalanceDto> GetBalanceAsync(int itemId, int warehouseId)
        {
            var balance = await _balanceRepository.GetByItemAndWarehouseAsync(itemId, warehouseId);
            if (balance == null)
                throw new ArgumentException("لا توجد أرصدة لهذا الصنف في هذا المستودع");

            return _mapper.Map<ItemBalanceDto>(balance);
        }

        public async Task<List<ItemBalanceDto>> GetWarehouseBalancesAsync(int warehouseId)
        {
            var balances = await _balanceRepository.GetByWarehouseAsync(warehouseId);
            return _mapper.Map<List<ItemBalanceDto>>(balances);
        }

        public async Task<List<ItemBalanceDto>> GetItemBalancesAsync(int itemId)
        {
            var balances = await _balanceRepository.GetByItemAsync(itemId);
            return _mapper.Map<List<ItemBalanceDto>>(balances);
        }

        public async Task<decimal> GetWarehouseValueAsync(int warehouseId)
        {
            return await _balanceRepository.GetTotalValueAsync(warehouseId);
        }

        public async Task<ItemBalanceDto> AddMovementAsync(ItemMovementCreateDto dto, int userId)
        {
            var item = await _itemRepository.GetByIdAsync(dto.ItemId);
            if (item == null)
                throw new ArgumentException("الصنف غير موجود");

            var movement = _mapper.Map<ItemMovement>(dto);
            movement.CreatedBy = userId;
            movement.MovementDate = DateTime.Now;

            await _movementRepository.AddAsync(movement);

            // تحديث الرصيد
            var balance = await _balanceRepository.GetByItemAndWarehouseAsync(dto.ItemId, dto.WarehouseId);
            if (balance == null)
            {
                balance = new ItemBalance
                {
                    ItemId = dto.ItemId,
                    WarehouseId = dto.WarehouseId,
                    BalanceQuantity = dto.MovementQuantity,
                    AverageCost = dto.MovementCost,
                    LastMovementDate = DateTime.Now,
                    CreatedBy = userId
                };
                await _balanceRepository.AddAsync(balance);
            }
            else
            {
                // حساب متوسط التكلفة
                decimal newQuantity = balance.BalanceQuantity + dto.MovementQuantity;
                balance.AverageCost = (balance.BalanceQuantity * balance.AverageCost + dto.MovementQuantity * dto.MovementCost) / newQuantity;
                balance.BalanceQuantity = newQuantity;
                balance.LastMovementDate = DateTime.Now;
                await _balanceRepository.UpdateAsync(balance);
            }

            await _balanceRepository.SaveChangesAsync();
            await _movementRepository.SaveChangesAsync();

            return _mapper.Map<ItemBalanceDto>(balance);
        }

        public async Task<ItemMovementDto> GetMovementAsync(int movementId)
        {
            var movement = await _movementRepository.GetByIdAsync(movementId);
            if (movement == null)
                throw new ArgumentException("الحركة غير موجودة");

            return _mapper.Map<ItemMovementDto>(movement);
        }

        public async Task<List<ItemMovementDto>> GetMovementsAsync(DateTime fromDate, DateTime toDate)
        {
            var movements = await _movementRepository.GetByDateRangeAsync(fromDate, toDate);
            return _mapper.Map<List<ItemMovementDto>>(movements);
        }

        public async Task<bool> PostMovementAsync(int movementId)
        {
            var movement = await _movementRepository.GetByIdAsync(movementId);
            if (movement == null)
                throw new ArgumentException("الحركة غير موجودة");

            movement.IsPosted = true;
            await _movementRepository.UpdateAsync(movement);
            await _movementRepository.SaveChangesAsync();

            return true;
        }
    }
}
