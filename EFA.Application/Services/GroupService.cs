using AutoMapper;
using EFA.Application.DTOs;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;

namespace EFA.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IPrivilegeRepository _privilegeRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IMapper _mapper;

    public GroupService(
        IGroupRepository groupRepository,
        IPrivilegeRepository privilegeRepository,
        IAuditRepository auditRepository,
        IMapper mapper)
    {
        _groupRepository = groupRepository;
        _privilegeRepository = privilegeRepository;
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<GroupDto>> CreateGroupAsync(CreateGroupDto createGroupDto)
    {
        try
        {
            var existingGroup = await _groupRepository.GetByCodeAsync(createGroupDto.GroupCode);
            if (existingGroup != null)
                return new ResponseDto<GroupDto> { Success = false, Message = "رمز المجموعة موجود بالفعل" };

            var group = new Group
            {
                GroupCode = createGroupDto.GroupCode,
                GroupName = createGroupDto.GroupName,
                Description = createGroupDto.Description,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _groupRepository.AddAsync(group);
            await _groupRepository.SaveChangesAsync();

            await LogAuditAsync(null, "Group", "Create", group.GroupId, null, createGroupDto.GroupName);

            var groupDto = _mapper.Map<GroupDto>(group);
            return new ResponseDto<GroupDto> { Success = true, Message = "تم إنشاء المجموعة بنجاح", Data = groupDto };
        }
        catch (Exception ex)
        {
            return new ResponseDto<GroupDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء إنشاء المجموعة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<GroupDto>> UpdateGroupAsync(UpdateGroupDto updateGroupDto)
    {
        try
        {
            var group = await _groupRepository.GetByIdAsync(updateGroupDto.GroupId);
            if (group == null)
                return new ResponseDto<GroupDto> { Success = false, Message = "المجموعة غير موجودة" };

            group.GroupCode = updateGroupDto.GroupCode;
            group.GroupName = updateGroupDto.GroupName;
            group.Description = updateGroupDto.Description;
            group.IsActive = updateGroupDto.IsActive;
            group.LastModifiedDate = DateTime.UtcNow;

            _groupRepository.Update(group);
            await _groupRepository.SaveChangesAsync();

            await LogAuditAsync(null, "Group", "Update", group.GroupId, null, updateGroupDto.GroupName);

            var groupDto = _mapper.Map<GroupDto>(group);
            return new ResponseDto<GroupDto> { Success = true, Message = "تم تحديث المجموعة بنجاح", Data = groupDto };
        }
        catch (Exception ex)
        {
            return new ResponseDto<GroupDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تحديث المجموعة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<bool>> DeleteGroupAsync(int groupId)
    {
        try
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                return new ResponseDto<bool> { Success = false, Message = "المجموعة غير موجودة" };

            _groupRepository.Delete(group);
            await _groupRepository.SaveChangesAsync();

            await LogAuditAsync(null, "Group", "Delete", groupId, null, group.GroupName);

            return new ResponseDto<bool> { Success = true, Message = "تم حذف المجموعة بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء حذف المجموعة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<GroupDetailDto?> GetGroupDetailsAsync(int groupId)
    {
        var group = await _groupRepository.GetGroupWithPrivilegesAsync(groupId);
        if (group == null) return null;

        return _mapper.Map<GroupDetailDto>(group);
    }

    public async Task<GroupDto?> GetGroupByIdAsync(int groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        return group != null ? _mapper.Map<GroupDto>(group) : null;
    }

    public async Task<PaginatedResponseDto<GroupDto>> GetAllGroupsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var (groups, total) = await _groupRepository.GetPagedAsync(pageNumber, pageSize, null, q => q.OrderByDescending(g => g.CreatedDate));

        return new PaginatedResponseDto<GroupDto>
        {
            Items = _mapper.Map<List<GroupDto>>(groups),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ResponseDto<bool>> AssignPrivilegesToGroupAsync(int groupId, List<int> privilegeIds)
    {
        try
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                return new ResponseDto<bool> { Success = false, Message = "المجموعة غير موجودة" };

            await _groupRepository.AssignPrivilegesToGroupAsync(groupId, privilegeIds);

            await LogAuditAsync(null, "Group", "AssignPrivileges", groupId, null, $"تعيين {privilegeIds.Count} صلاحيات للمجموعة");

            return new ResponseDto<bool> { Success = true, Message = "تم تعيين الصلاحيات بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تعيين الصلاحيات",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<IEnumerable<PrivilegeDto>> GetGroupPrivilegesAsync(int groupId)
    {
        var privileges = await _privilegeRepository.GetGroupPrivilegesAsync(groupId);
        return _mapper.Map<IEnumerable<PrivilegeDto>>(privileges);
    }

    private async Task LogAuditAsync(int? userId, string entityName, string action, int? entityId, string? oldValue, string? newValue)
    {
        var audit = new Audit
        {
            UserId = userId,
            EntityName = entityName,
            Action = action,
            EntityId = entityId,
            OldValues = oldValue,
            NewValues = newValue,
            AuditDate = DateTime.UtcNow,
            IsSuccessful = true
        };

        await _auditRepository.AddAsync(audit);
        await _auditRepository.SaveChangesAsync();
    }
}
