using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories;

public interface IAuditRepository : IGenericRepository<Audit>
{
    Task<IEnumerable<Audit>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Audit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Audit>> GetByEntityAsync(string entityName, int? entityId = null);
    Task<IEnumerable<Audit>> GetFailedAuditsAsync();
}
