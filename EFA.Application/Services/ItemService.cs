using AutoMapper;
using EFA.Application.DTOs;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;

namespace EFA.Application.Services
{
    public interface IItemService
    {
        Task<ItemDto> GetByIdAsync(int id);
        Task<ItemDto> GetByCodeAsync(string code);
        Task<List<ItemDto>> GetAllAsync();
        Task<List<ItemDto>> GetActiveAsync();
        Task<List<ItemDto>> SearchAsync(string searchTerm);
        Task<List<ItemDto>> GetByCategoryAsync(int categoryId);
        Task<ItemDto> CreateAsync(ItemCreateUpdateDto dto, int userId);
        Task<ItemDto> UpdateAsync(int id, ItemCreateUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id);
        Task<List<ItemDto>> GetLowStockAsync();
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
    }

    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemCategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ItemService(IItemRepository itemRepository, IItemCategoryRepository categoryRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ItemDto> GetByIdAsync(int id)
        {
            var item = await _itemRepository.GetWithCategoryAsync(id);
            if (item == null)
                throw new ArgumentException("الصنف غير موجود");

            return _mapper.Map<ItemDto>(item);
        }

        public async Task<ItemDto> GetByCodeAsync(string code)
        {
            var item = await _itemRepository.GetByCodeAsync(code);
            if (item == null)
                throw new ArgumentException("الصنف بهذا الكود غير موجود");

            return _mapper.Map<ItemDto>(item);
        }

        public async Task<List<ItemDto>> GetAllAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<List<ItemDto>> GetActiveAsync()
        {
            var items = await _itemRepository.GetActiveOnlyAsync();
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<List<ItemDto>> SearchAsync(string searchTerm)
        {
            var items = await _itemRepository.SearchAsync(searchTerm);
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<List<ItemDto>> GetByCategoryAsync(int categoryId)
        {
            var items = await _itemRepository.GetByCategoryAsync(categoryId);
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<ItemDto> CreateAsync(ItemCreateUpdateDto dto, int userId)
        {
            // التحقق من وجود الفئة
            var category = await _categoryRepository.GetByIdAsync(dto.ItemCategoryId);
            if (category == null)
                throw new ArgumentException("الفئة المحددة غير موجودة");

            // التحقق من تفرد الكود
            if (!await _itemRepository.IsCodeUniqueAsync(dto.ItemCode))
                throw new ArgumentException("كود الصنف موجود بالفعل");

            var item = _mapper.Map<Item>(dto);
            item.CreatedBy = userId;
            item.IsActive = true;

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();

            return _mapper.Map<ItemDto>(item);
        }

        public async Task<ItemDto> UpdateAsync(int id, ItemCreateUpdateDto dto, int userId)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                throw new ArgumentException("الصنف غير موجود");

            // التحقق من الفئة إذا تغيرت
            if (item.ItemCategoryId != dto.ItemCategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.ItemCategoryId);
                if (category == null)
                    throw new ArgumentException("الفئة المحددة غير موجودة");
            }

            // التحقق من تفرد الكود إذا تغير
            if (item.ItemCode != dto.ItemCode)
            {
                if (!await _itemRepository.IsCodeUniqueAsync(dto.ItemCode, id))
                    throw new ArgumentException("كود الصنف موجود بالفعل");
            }

            _mapper.Map(dto, item);
            item.ModifiedBy = userId;
            item.ModifiedDate = DateTime.Now;

            await _itemRepository.UpdateAsync(item);
            await _itemRepository.SaveChangesAsync();

            return _mapper.Map<ItemDto>(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                return false;

            item.IsActive = false;
            item.ModifiedDate = DateTime.Now;

            await _itemRepository.UpdateAsync(item);
            await _itemRepository.SaveChangesAsync();

            return true;
        }

        public async Task<List<ItemDto>> GetLowStockAsync()
        {
            var items = await _itemRepository.GetLowStockItemsAsync();
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            return await _itemRepository.IsCodeUniqueAsync(code, excludeId);
        }
    }
}
