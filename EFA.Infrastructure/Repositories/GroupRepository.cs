using EFA.Domain.Entities;
using EFA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories;

public class GroupRepository : GenericRepository<Group>, IGroupRepository
{
    private readonly EFADbContext _context;

    public GroupRepository(EFADbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Group?> GetGroupWithPrivilegesAsync(int groupId)
    {
        return await _context.Groups
            .Include(g => g.Privileges)
            .ThenInclude(gp => gp.Privilege)
            .FirstOrDefaultAsync(g => g.GroupId == groupId);
    }

    public async Task<IEnumerable<Group>> GetActiveGroupsAsync()
    {
        return await _context.Groups
            .Where(g => g.IsActive)
            .ToListAsync();
    }

    public async Task<Group?> GetByCodeAsync(string code)
    {
        return await _context.Groups
            .FirstOrDefaultAsync(g => g.GroupCode == code);
    }

    public async Task AssignPrivilegesToGroupAsync(int groupId, List<int> privilegeIds)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null) return;

        // Remove existing privileges
        var existingPrivileges = await _context.GroupPrivileges
            .Where(gp => gp.GroupId == groupId)
            .ToListAsync();
        _context.GroupPrivileges.RemoveRange(existingPrivileges);

        // Add new privileges
        var newPrivileges = privilegeIds.Select(pid => new GroupPrivilege
        {
            GroupId = groupId,
            PrivilegeId = pid,
            IsGranted = true,
            CreatedDate = DateTime.UtcNow
        }).ToList();

        await _context.GroupPrivileges.AddRangeAsync(newPrivileges);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveGroupPrivilegesAsync(int groupId)
    {
        var privileges = await _context.GroupPrivileges
            .Where(gp => gp.GroupId == groupId)
            .ToListAsync();
        
        _context.GroupPrivileges.RemoveRange(privileges);
        await _context.SaveChangesAsync();
    }
}
