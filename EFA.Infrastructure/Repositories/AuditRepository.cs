using EFA.Domain.Entities;
using EFA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Repositories;

public class AuditRepository : GenericRepository<Audit>, IAuditRepository
{
    private readonly EFADbContext _context;

    public AuditRepository(EFADbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Audit>> GetByUserIdAsync(int userId)
    {
        return await _context.Audits
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AuditDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Audit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Audits
            .Where(a => a.AuditDate >= startDate && a.AuditDate <= endDate)
            .OrderByDescending(a => a.AuditDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Audit>> GetByEntityAsync(string entityName, int? entityId = null)
    {
        IQueryable<Audit> query = _context.Audits.Where(a => a.EntityName == entityName);
        
        if (entityId.HasValue)
            query = query.Where(a => a.EntityId == entityId);

        return await query
            .OrderByDescending(a => a.AuditDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Audit>> GetFailedAuditsAsync()
    {
        return await _context.Audits
            .Where(a => !a.IsSuccessful)
            .OrderByDescending(a => a.AuditDate)
            .ToListAsync();
    }
}
