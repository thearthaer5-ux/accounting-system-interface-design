using EFA.Domain.Entities;
using EFA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories;

public class PrivilegeRepository : GenericRepository<Privilege>, IPrivilegeRepository
{
    private readonly EFADbContext _context;

    public PrivilegeRepository(EFADbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Privilege?> GetByCodeAsync(string code)
    {
        return await _context.Privileges
            .FirstOrDefaultAsync(p => p.Code == code && p.IsActive);
    }

    public async Task<IEnumerable<Privilege>> GetUserPrivilegesAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Group)
            .ThenInclude(g => g!.Privileges)
            .ThenInclude(gp => gp.Privilege)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user?.Group == null) return Enumerable.Empty<Privilege>();

        return user.Group.Privileges
            .Where(gp => gp.IsGranted)
            .Select(gp => gp.Privilege)
            .ToList();
    }

    public async Task<IEnumerable<Privilege>> GetGroupPrivilegesAsync(int groupId)
    {
        return await _context.GroupPrivileges
            .Where(gp => gp.GroupId == groupId && gp.IsGranted)
            .Include(gp => gp.Privilege)
            .Select(gp => gp.Privilege)
            .ToListAsync();
    }

    public async Task<bool> HasPrivilegeAsync(int userId, string privilegeCode)
    {
        var user = await _context.Users
            .Include(u => u.Group)
            .ThenInclude(g => g!.Privileges)
            .ThenInclude(gp => gp.Privilege)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user?.Group == null) return false;

        return user.Group.Privileges
            .Any(gp => gp.IsGranted && gp.Privilege.Code == privilegeCode);
    }

    public async Task<IEnumerable<Privilege>> GetPrivilegesByCategoryAsync(string category)
    {
        return await _context.Privileges
            .Where(p => p.Category == category && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
    }
}
