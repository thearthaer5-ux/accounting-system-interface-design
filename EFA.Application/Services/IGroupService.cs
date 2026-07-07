using EFA.Application.DTOs;

namespace EFA.Application.Services;

public interface IGroupService
{
    Task<ResponseDto<GroupDto>> CreateGroupAsync(CreateGroupDto createGroupDto);
    Task<ResponseDto<GroupDto>> UpdateGroupAsync(UpdateGroupDto updateGroupDto);
    Task<ResponseDto<bool>> DeleteGroupAsync(int groupId);
    Task<GroupDetailDto?> GetGroupDetailsAsync(int groupId);
    Task<GroupDto?> GetGroupByIdAsync(int groupId);
    Task<PaginatedResponseDto<GroupDto>> GetAllGroupsAsync(int pageNumber = 1, int pageSize = 10);
    Task<ResponseDto<bool>> AssignPrivilegesToGroupAsync(int groupId, List<int> privilegeIds);
    Task<IEnumerable<PrivilegeDto>> GetGroupPrivilegesAsync(int groupId);
}
