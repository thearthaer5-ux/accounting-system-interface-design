using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories;

public interface IGroupRepository : IGenericRepository<Group>
{
    Task<Group?> GetGroupWithPrivilegesAsync(int groupId);
    Task<IEnumerable<Group>> GetActiveGroupsAsync();
    Task<Group?> GetByCodeAsync(string code);
    Task AssignPrivilegesToGroupAsync(int groupId, List<int> privilegeIds);
    Task RemoveGroupPrivilegesAsync(int groupId);
}
