using AutoMapper;
using EFA.Application.DTOs;
using EFA.Domain.Entities;

namespace EFA.Application.Profiles;

/// <summary>
/// ملف تكوين AutoMapper
/// يحدد تحويلات البيانات بين Models و DTOs
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group!.GroupName))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch!.BranchName));

        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        // Group mappings
        CreateMap<Group, GroupDto>()
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Users.Count));

        CreateMap<CreateGroupDto, Group>();
        CreateMap<UpdateGroupDto, Group>();

        CreateMap<Group, GroupDetailDto>()
            .ForMember(dest => dest.Privileges, opt => opt.MapFrom(src => src.Privileges.Select(gp => gp.Privilege)))
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));

        // Privilege mappings
        CreateMap<Privilege, PrivilegeDto>();

        // Branch mappings
        CreateMap<Branch, BranchDto>()
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Users.Count));

        // Currency mappings
        CreateMap<Currency, CurrencyDto>();

        // Audit mappings
        CreateMap<Audit, AuditDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.Username));

        // Item Category mappings
        CreateMap<ItemCategory, ItemCategoryDto>();
        CreateMap<ItemCategoryCreateDto, ItemCategory>();

        // Item mappings
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.ItemCategory!.ItemCategoryNameAr));
        CreateMap<ItemCreateUpdateDto, Item>();

        // Warehouse mappings
        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch!.BranchName));
        CreateMap<WarehouseCreateUpdateDto, Warehouse>();

        // Item Balance mappings
        CreateMap<ItemBalance, ItemBalanceDto>()
            .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.Item!.ItemCode))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item!.ItemNameAr))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.WarehouseNameAr))
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.BalanceQuantity * src.AverageCost));

        // Item Movement mappings
        CreateMap<ItemMovement, ItemMovementDto>()
            .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.Item!.ItemCode))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item!.ItemNameAr))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.WarehouseNameAr))
            .ForMember(dest => dest.WarehouseToName, opt => opt.MapFrom(src => src.WarehouseTo!.WarehouseNameAr));
        CreateMap<ItemMovementCreateDto, ItemMovement>();

        // Item Batch mappings
        CreateMap<ItemBatch, ItemBatchDto>()
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item!.ItemNameAr));
        CreateMap<ItemBatchCreateDto, ItemBatch>();

        // Inventory Count mappings
        CreateMap<InventoryCount, InventoryCountDto>()
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse!.WarehouseNameAr));
        CreateMap<InventoryCountCreateDto, InventoryCount>();

        // Inventory Count Detail mappings
        CreateMap<InventoryCountDetail, InventoryCountDetailDto>()
            .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.Item!.ItemCode))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item!.ItemNameAr));
        CreateMap<InventoryCountDetailDto, InventoryCountDetail>();

        // Chart of Account mappings
        CreateMap<ChartOfAccount, ChartOfAccountDto>();
        CreateMap<ChartOfAccountCreateUpdateDto, ChartOfAccount>();

        // Journal mappings
        CreateMap<Journal, JournalDto>()
            .ForMember(dest => dest.JournalTypeName, opt => opt.MapFrom(src => src.JournalType!.JournalTypeNameAr));
        CreateMap<JournalCreateUpdateDto, Journal>();

        // Journal Entry mappings
        CreateMap<JournalEntry, JournalEntryDto>()
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account!.AccountNumber))
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account!.AccountNameAr))
            .ForMember(dest => dest.CostCenterName, opt => opt.MapFrom(src => src.CostCenter!.CostCenterNameAr));
        CreateMap<JournalEntryCreateDto, JournalEntry>();

        // Fiscal Period mappings
        CreateMap<FiscalPeriod, FiscalPeriodDto>();
        CreateMap<FiscalPeriodCreateUpdateDto, FiscalPeriod>();

        // Opening Balance mappings
        CreateMap<OpeningBalance, OpeningBalanceDto>()
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account!.AccountNumber))
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account!.AccountNameAr));
        CreateMap<OpeningBalanceCreateUpdateDto, OpeningBalance>();

        // Account Balance mappings
        CreateMap<AccountBalance, AccountBalanceDto>()
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account!.AccountNumber))
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account!.AccountNameAr));

        // Ledger Report mappings
        CreateMap<LedgerReport, LedgerReportDto>()
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account!.AccountNumber))
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account!.AccountNameAr));

        // Cost Center mappings
        CreateMap<CostCenter, CostCenterDto>();
        CreateMap<CostCenterCreateUpdateDto, CostCenter>();
    }
}
