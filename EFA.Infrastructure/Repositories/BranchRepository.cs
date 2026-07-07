using EFA.Domain.Entities;
using EFA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories;

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    private readonly EFADbContext _context;

    public BranchRepository(EFADbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Branch?> GetByCodeAsync(string code)
    {
        return await _context.Branches
            .FirstOrDefaultAsync(b => b.BranchCode == code);
    }

    public async Task<IEnumerable<Branch>> GetActiveBranchesAsync()
    {
        return await _context.Branches
            .Where(b => b.IsActive)
            .ToListAsync();
    }

    public async Task<Branch?> GetHeadOfficeAsync()
    {
        return await _context.Branches
            .FirstOrDefaultAsync(b => b.IsHeadOffice && b.IsActive);
    }

    public async Task<IEnumerable<Branch>> GetBranchesWithUsersAsync()
    {
        return await _context.Branches
            .Include(b => b.Users)
            .Where(b => b.IsActive)
            .ToListAsync();
    }
}
