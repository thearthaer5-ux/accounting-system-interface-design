using AutoMapper;
using EFA.Application.DTOs;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;

namespace EFA.Application.Services;

public interface IBranchService
{
    Task<ResponseDto<BranchDto>> CreateBranchAsync(BranchDto branchDto);
    Task<ResponseDto<BranchDto>> UpdateBranchAsync(BranchDto branchDto);
    Task<ResponseDto<bool>> DeleteBranchAsync(int branchId);
    Task<BranchDto?> GetBranchByIdAsync(int branchId);
    Task<PaginatedResponseDto<BranchDto>> GetAllBranchesAsync(int pageNumber = 1, int pageSize = 10);
}

public class BranchService : IBranchService
{
    private readonly IBranchRepository _branchRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IMapper _mapper;

    public BranchService(IBranchRepository branchRepository, IAuditRepository auditRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<BranchDto>> CreateBranchAsync(BranchDto branchDto)
    {
        try
        {
            var existingBranch = await _branchRepository.GetByCodeAsync(branchDto.BranchCode);
            if (existingBranch != null)
                return new ResponseDto<BranchDto> { Success = false, Message = "رمز الفرع موجود بالفعل" };

            var branch = _mapper.Map<Branch>(branchDto);
            branch.CreatedDate = DateTime.UtcNow;
            branch.IsActive = true;

            await _branchRepository.AddAsync(branch);
            await _branchRepository.SaveChangesAsync();

            var result = _mapper.Map<BranchDto>(branch);
            return new ResponseDto<BranchDto> { Success = true, Message = "تم إنشاء الفرع بنجاح", Data = result };
        }
        catch (Exception ex)
        {
            return new ResponseDto<BranchDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء إنشاء الفرع",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<BranchDto>> UpdateBranchAsync(BranchDto branchDto)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(branchDto.BranchId);
            if (branch == null)
                return new ResponseDto<BranchDto> { Success = false, Message = "الفرع غير موجود" };

            branch.BranchName = branchDto.BranchName;
            branch.Address = branchDto.Address;
            branch.City = branchDto.City;
            branch.PhoneNumber = branchDto.PhoneNumber;
            branch.IsActive = branchDto.IsActive;
            branch.LastModifiedDate = DateTime.UtcNow;

            _branchRepository.Update(branch);
            await _branchRepository.SaveChangesAsync();

            var result = _mapper.Map<BranchDto>(branch);
            return new ResponseDto<BranchDto> { Success = true, Message = "تم تحديث الفرع بنجاح", Data = result };
        }
        catch (Exception ex)
        {
            return new ResponseDto<BranchDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تحديث الفرع",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<bool>> DeleteBranchAsync(int branchId)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null)
                return new ResponseDto<bool> { Success = false, Message = "الفرع غير موجود" };

            _branchRepository.Delete(branch);
            await _branchRepository.SaveChangesAsync();

            return new ResponseDto<bool> { Success = true, Message = "تم حذف الفرع بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء حذف الفرع",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<BranchDto?> GetBranchByIdAsync(int branchId)
    {
        var branch = await _branchRepository.GetByIdAsync(branchId);
        return branch != null ? _mapper.Map<BranchDto>(branch) : null;
    }

    public async Task<PaginatedResponseDto<BranchDto>> GetAllBranchesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var (branches, total) = await _branchRepository.GetPagedAsync(pageNumber, pageSize, null, q => q.OrderByDescending(b => b.CreatedDate));

        return new PaginatedResponseDto<BranchDto>
        {
            Items = _mapper.Map<List<BranchDto>>(branches),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}

public interface ICurrencyService
{
    Task<ResponseDto<CurrencyDto>> CreateCurrencyAsync(CurrencyDto currencyDto);
    Task<ResponseDto<CurrencyDto>> UpdateCurrencyAsync(CurrencyDto currencyDto);
    Task<ResponseDto<bool>> DeleteCurrencyAsync(int currencyId);
    Task<CurrencyDto?> GetCurrencyByIdAsync(int currencyId);
    Task<PaginatedResponseDto<CurrencyDto>> GetAllCurrenciesAsync(int pageNumber = 1, int pageSize = 10);
    Task<CurrencyDto?> GetDefaultCurrencyAsync();
}

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IMapper _mapper;

    public CurrencyService(ICurrencyRepository currencyRepository, IMapper mapper)
    {
        _currencyRepository = currencyRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<CurrencyDto>> CreateCurrencyAsync(CurrencyDto currencyDto)
    {
        try
        {
            var existingCurrency = await _currencyRepository.GetByCodeAsync(currencyDto.CurrencyCode);
            if (existingCurrency != null)
                return new ResponseDto<CurrencyDto> { Success = false, Message = "رمز العملة موجود بالفعل" };

            var currency = _mapper.Map<Currency>(currencyDto);
            currency.CreatedDate = DateTime.UtcNow;

            await _currencyRepository.AddAsync(currency);
            await _currencyRepository.SaveChangesAsync();

            var result = _mapper.Map<CurrencyDto>(currency);
            return new ResponseDto<CurrencyDto> { Success = true, Message = "تم إنشاء العملة بنجاح", Data = result };
        }
        catch (Exception ex)
        {
            return new ResponseDto<CurrencyDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء إنشاء العملة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<CurrencyDto>> UpdateCurrencyAsync(CurrencyDto currencyDto)
    {
        try
        {
            var currency = await _currencyRepository.GetByIdAsync(currencyDto.CurrencyId);
            if (currency == null)
                return new ResponseDto<CurrencyDto> { Success = false, Message = "العملة غير موجودة" };

            currency.CurrencyName = currencyDto.CurrencyName;
            currency.Symbol = currencyDto.Symbol;
            currency.ExchangeRate = currencyDto.ExchangeRate;
            currency.IsDefault = currencyDto.IsDefault;
            currency.IsActive = currencyDto.IsActive;
            currency.LastModifiedDate = DateTime.UtcNow;

            _currencyRepository.Update(currency);
            await _currencyRepository.SaveChangesAsync();

            var result = _mapper.Map<CurrencyDto>(currency);
            return new ResponseDto<CurrencyDto> { Success = true, Message = "تم تحديث العملة بنجاح", Data = result };
        }
        catch (Exception ex)
        {
            return new ResponseDto<CurrencyDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تحديث العملة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<bool>> DeleteCurrencyAsync(int currencyId)
    {
        try
        {
            var currency = await _currencyRepository.GetByIdAsync(currencyId);
            if (currency == null)
                return new ResponseDto<bool> { Success = false, Message = "العملة غير موجودة" };

            _currencyRepository.Delete(currency);
            await _currencyRepository.SaveChangesAsync();

            return new ResponseDto<bool> { Success = true, Message = "تم حذف العملة بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء حذف العملة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<CurrencyDto?> GetCurrencyByIdAsync(int currencyId)
    {
        var currency = await _currencyRepository.GetByIdAsync(currencyId);
        return currency != null ? _mapper.Map<CurrencyDto>(currency) : null;
    }

    public async Task<PaginatedResponseDto<CurrencyDto>> GetAllCurrenciesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var (currencies, total) = await _currencyRepository.GetPagedAsync(pageNumber, pageSize, null, q => q.OrderBy(c => c.CurrencyName));

        return new PaginatedResponseDto<CurrencyDto>
        {
            Items = _mapper.Map<List<CurrencyDto>>(currencies),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<CurrencyDto?> GetDefaultCurrencyAsync()
    {
        var currency = await _currencyRepository.GetDefaultCurrencyAsync();
        return currency != null ? _mapper.Map<CurrencyDto>(currency) : null;
    }
}
