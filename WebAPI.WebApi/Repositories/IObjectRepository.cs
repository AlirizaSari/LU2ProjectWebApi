using ProjectLU2.WebApi.Models;

namespace ProjectLU2.WebApi.Repositories;

public interface IObjectRepository
{
    Task<Object2D> InsertAsync(Object2D obj);
    Task<Object2D?> ReadAsync(Guid id);
    Task<IEnumerable<Object2D>> ReadByEnvironmentIdAsync(Guid environmentId);
    Task<IEnumerable<Object2D>> ReadAllAsync();
    Task UpdateAsync(Object2D obj);
    Task DeleteAsync(Guid id);
}
