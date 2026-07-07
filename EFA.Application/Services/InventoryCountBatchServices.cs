using AutoMapper;
using EFA.Application.DTOs;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;

namespace EFA.Application.Services
{
    public interface IInventoryCountService
    {
        Task<InventoryCountDto> GetByIdAsync(int id);
        Task<List<InventoryCountDto>> GetByWarehouseAsync(int warehouseId);
        Task<InventoryCountDto> CreateAsync(InventoryCountCreateDto dto, int userId);
        Task<InventoryCountDto> UpdateAsync(int id, InventoryCountCreateDto dto, int userId);
        Task<bool> AddDetailAsync(int countId, InventoryCountDetailDto detail, int userId);
        Task<List<InventoryCountDetailDto>> GetDetailsAsync(int countId);
        Task<bool> ApproveAsync(int countId, int userId);
        Task<bool> PostAsync(int countId, int userId);
    }

    public interface IItemBatchService
    {
        Task<ItemBatchDto> GetByIdAsync(int id);
        Task<ItemBatchDto> GetByBatchNumberAsync(string batchNumber);
        Task<List<ItemBatchDto>> GetByItemAsync(int itemId);
        Task<List<ItemBatchDto>> GetAvailableBatchesAsync(int itemId);
        Task<List<ItemBatchDto>> GetExpiredBatchesAsync();
        Task<ItemBatchDto> CreateAsync(ItemBatchCreateDto dto, int userId);
        Task<bool> UpdateAsync(int id, ItemBatchCreateDto dto, int userId);
        Task<bool> DeactivateAsync(int id);
    }

    public class InventoryCountService : IInventoryCountService
    {
        private readonly IInventoryCountRepository _countRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IItemBalanceRepository _balanceRepository;
        private readonly IMapper _mapper;

        public InventoryCountService(IInventoryCountRepository countRepository, IWarehouseRepository warehouseRepository,
            IItemBalanceRepository balanceRepository, IMapper mapper)
        {
            _countRepository = countRepository;
            _warehouseRepository = warehouseRepository;
            _balanceRepository = balanceRepository;
            _mapper = mapper;
        }

        public async Task<InventoryCountDto> GetByIdAsync(int id)
        {
            var count = await _countRepository.GetWithDetailsAsync(id);
            if (count == null)
                throw new ArgumentException("الجرد غير موجود");

            var dto = _mapper.Map<InventoryCountDto>(count);
            dto.DetailCount = count.InventoryCountDetails.Count;
            dto.TotalDifference = count.InventoryCountDetails.Sum(d => d.DifferenceCost);

            return dto;
        }

        public async Task<List<InventoryCountDto>> GetByWarehouseAsync(int warehouseId)
        {
            var counts = await _countRepository.GetByWarehouseAsync(warehouseId);
            return _mapper.Map<List<InventoryCountDto>>(counts);
        }

        public async Task<InventoryCountDto> CreateAsync(InventoryCountCreateDto dto, int userId)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                throw new ArgumentException("المستودع غير موجود");

            var count = new InventoryCount
            {
                WarehouseId = dto.WarehouseId,
                CountNumber = $"CNT-{DateTime.Now:yyyyMMddHHmmss}",
                CountDate = DateTime.Now,
                Notes = dto.Notes,
                Status = 1, // Draft
                CreatedBy = userId
            };

            await _countRepository.AddAsync(count);
            await _countRepository.SaveChangesAsync();

            return _mapper.Map<InventoryCountDto>(count);
        }

        public async Task<InventoryCountDto> UpdateAsync(int id, InventoryCountCreateDto dto, int userId)
        {
            var count = await _countRepository.GetByIdAsync(id);
            if (count == null)
                throw new ArgumentException("الجرد غير موجود");

            if (count.Status != 1)
                throw new InvalidOperationException("لا يمكن تعديل جرد تم بدء عملية الجرد به");

            count.Notes = dto.Notes;

            await _countRepository.UpdateAsync(count);
            await _countRepository.SaveChangesAsync();

            return _mapper.Map<InventoryCountDto>(count);
        }

        public async Task<bool> AddDetailAsync(int countId, InventoryCountDetailDto detail, int userId)
        {
            var count = await _countRepository.GetByIdAsync(countId);
            if (count == null)
                throw new ArgumentException("الجرد غير موجود");

            var balance = await _balanceRepository.GetByItemAndWarehouseAsync(detail.ItemId, count.WarehouseId);
            
            var countDetail = new InventoryCountDetail
            {
                InventoryCountId = countId,
                ItemId = detail.ItemId,
                SystemQuantity = balance?.BalanceQuantity ?? 0,
                PhysicalQuantity = detail.PhysicalQuantity,
                Difference = (detail.PhysicalQuantity) - (balance?.BalanceQuantity ?? 0),
                UnitCost = balance?.AverageCost ?? 0,
                DifferenceCost = ((detail.PhysicalQuantity) - (balance?.BalanceQuantity ?? 0)) * (balance?.AverageCost ?? 0),
                Notes = detail.Notes,
                CreatedBy = userId
            };

            // إضافة التفصيل
            var context = _countRepository as dynamic;
            var dbContext = context._context;
            await dbContext.InventoryCountDetails.AddAsync(countDetail);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<InventoryCountDetailDto>> GetDetailsAsync(int countId)
        {
            var count = await _countRepository.GetWithDetailsAsync(countId);
            if (count == null)
                throw new ArgumentException("الجرد غير موجود");

            return _mapper.Map<List<InventoryCountDetailDto>>(count.InventoryCountDetails);
        }

        public async Task<bool> ApproveAsync(int countId, int userId)
        {
            var count = await _countRepository.GetByIdAsync(countId);
            if (count == null)
                throw new ArgumentException("الجرد غير موجود");

            count.Status = 3; // Completed
            count.ApprovedDate = DateTime.Now;
            count.ApprovedBy = userId;

            await _countRepository.UpdateAsync(count);
            await _countRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PostAsync(int countId, int userId)
        {
            var count = await _countRepository.GetWithDetailsAsync(countId);
            if (count == null)
                throw new ArgumentException("الجرد غير موجود");

            count.IsPosted = true;
            count.Status = 4; // Approved

            await _countRepository.UpdateAsync(count);
            await _countRepository.SaveChangesAsync();

            return true;
        }
    }

    public class ItemBatchService : IItemBatchService
    {
        private readonly IItemBatchRepository _batchRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public ItemBatchService(IItemBatchRepository batchRepository, IItemRepository itemRepository, IMapper mapper)
        {
            _batchRepository = batchRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<ItemBatchDto> GetByIdAsync(int id)
        {
            var batch = await _batchRepository.GetByIdAsync(id);
            if (batch == null)
                throw new ArgumentException("الدفعة غير موجودة");

            return _mapper.Map<ItemBatchDto>(batch);
        }

        public async Task<ItemBatchDto> GetByBatchNumberAsync(string batchNumber)
        {
            var batch = await _batchRepository.GetByBatchNumberAsync(batchNumber);
            if (batch == null)
                throw new ArgumentException("الدفعة غير موجودة");

            return _mapper.Map<ItemBatchDto>(batch);
        }

        public async Task<List<ItemBatchDto>> GetByItemAsync(int itemId)
        {
            var batches = await _batchRepository.GetByItemAsync(itemId);
            return _mapper.Map<List<ItemBatchDto>>(batches);
        }

        public async Task<List<ItemBatchDto>> GetAvailableBatchesAsync(int itemId)
        {
            var batches = await _batchRepository.GetAvailableBatchesAsync(itemId);
            return _mapper.Map<List<ItemBatchDto>>(batches);
        }

        public async Task<List<ItemBatchDto>> GetExpiredBatchesAsync()
        {
            var batches = await _batchRepository.GetExpiredBatchesAsync();
            return _mapper.Map<List<ItemBatchDto>>(batches);
        }

        public async Task<ItemBatchDto> CreateAsync(ItemBatchCreateDto dto, int userId)
        {
            var item = await _itemRepository.GetByIdAsync(dto.ItemId);
            if (item == null)
                throw new ArgumentException("الصنف غير موجود");

            var batch = _mapper.Map<ItemBatch>(dto);
            batch.CreatedBy = userId;
            batch.IsAvailable = true;

            await _batchRepository.AddAsync(batch);
            await _batchRepository.SaveChangesAsync();

            return _mapper.Map<ItemBatchDto>(batch);
        }

        public async Task<bool> UpdateAsync(int id, ItemBatchCreateDto dto, int userId)
        {
            var batch = await _batchRepository.GetByIdAsync(id);
            if (batch == null)
                throw new ArgumentException("الدفعة غير موجودة");

            _mapper.Map(dto, batch);

            await _batchRepository.UpdateAsync(batch);
            await _batchRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var batch = await _batchRepository.GetByIdAsync(id);
            if (batch == null)
                throw new ArgumentException("الدفعة غير موجودة");

            batch.IsAvailable = false;

            await _batchRepository.UpdateAsync(batch);
            await _batchRepository.SaveChangesAsync();

            return true;
        }
    }
}
