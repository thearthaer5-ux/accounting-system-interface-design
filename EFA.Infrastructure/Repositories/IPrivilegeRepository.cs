using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories;

public interface IPrivilegeRepository : IGenericRepository<Privilege>
{
    Task<Privilege?> GetByCodeAsync(string code);
    Task<IEnumerable<Privilege>> GetUserPrivilegesAsync(int userId);
    Task<IEnumerable<Privilege>> GetGroupPrivilegesAsync(int groupId);
    Task<bool> HasPrivilegeAsync(int userId, string privilegeCode);
    Task<IEnumerable<Privilege>> GetPrivilegesByCategoryAsync(string category);
}
