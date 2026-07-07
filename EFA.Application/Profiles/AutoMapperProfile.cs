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
    }
}
