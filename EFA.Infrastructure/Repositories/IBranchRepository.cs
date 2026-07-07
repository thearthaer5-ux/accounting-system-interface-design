using EFA.Domain.Entities;

namespace EFA.Infrastructure.Repositories;

public interface IBranchRepository : IGenericRepository<Branch>
{
    Task<Branch?> GetByCodeAsync(string code);
    Task<IEnumerable<Branch>> GetActiveBranchesAsync();
    Task<Branch?> GetHeadOfficeAsync();
    Task<IEnumerable<Branch>> GetBranchesWithUsersAsync();
}
