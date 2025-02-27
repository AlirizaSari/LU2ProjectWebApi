using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using ProjectLU2.WebApi.Models;

namespace ProjectLU2.WebApi.Repositories;

public class SqlEnvironmentRepository : IEnvironmentRepository
{
    private readonly string sqlConnectionString;

    public SqlEnvironmentRepository(string sqlConnectionString)
    {
        this.sqlConnectionString = sqlConnectionString;
    }

    public async Task<Environment2D> InsertAsync(Environment2D environment)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            var environmentId = await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (Id, Name, OwnerUserId, MaxLength, MaxHeight) VALUES (@Id, @Name, @OwnerUserId, @MaxLength, @MaxHeight)", environment);
            return environment;
        }
    }

    public async Task<Environment2D?> ReadAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { id });
        }
    }

    public async Task<IEnumerable<Environment2D>> ReadAllAsync()
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D]");
        }
    }

    public async Task<IEnumerable<Environment2D>> ReadByUserIdAsync(string userId)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE OwnerUserId = @OwnerUserId", new { OwnerUserId = userId });
        }
    }

    public async Task UpdateAsync(Environment2D environment)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET Name = @Name, OwnerUserId = @OwnerUserId, MaxLength = @MaxLength, MaxHeight = @MaxHeight WHERE Id = @id", environment);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
        }
    }
}
