using ProjectLU2.WebApi.Models;

namespace ProjectLU2.WebApi.Repositories;

public interface IEnvironmentRepository
{
    Task<Environment2D> InsertAsync(Environment2D environment);
    Task<Environment2D?> ReadAsync(Guid id);
    Task<IEnumerable<Environment2D>> ReadAllAsync();
    Task<IEnumerable<Environment2D>> ReadByUserIdAsync(string ownerUserId);
    Task UpdateAsync(Environment2D environment);
    Task DeleteAsync(Guid id);
}
