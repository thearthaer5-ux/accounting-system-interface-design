using EFA.Domain.Entities;
using EFA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories;

/// <summary>
/// تطبيق مستودع المستخدمين
/// يوفر عمليات مخصصة للمستخدمين
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly EFADbContext _context;

    public UserRepository(EFADbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<User>> GetUsersByGroupAsync(int groupId)
    {
        return await _context.Users
            .Where(u => u.GroupId == groupId && u.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId)
    {
        return await _context.Users
            .Where(u => u.BranchId == branchId && u.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .Include(u => u.Group)
            .Include(u => u.Branch)
            .ToListAsync();
    }

    public async Task<User?> GetUserWithGroupAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Group)
            .Include(u => u.Group!.Privileges)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserWithDevicesAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Devices)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
