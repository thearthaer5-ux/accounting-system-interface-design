using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories;

/// <summary>
/// واجهة مستودع المستخدمين
/// توفر عمليات مخصصة للمستخدمين
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByGroupAsync(int groupId);
    Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User?> GetUserWithGroupAsync(int userId);
    Task<User?> GetUserWithDevicesAsync(int userId);
    Task UpdateLastLoginAsync(int userId);
}
