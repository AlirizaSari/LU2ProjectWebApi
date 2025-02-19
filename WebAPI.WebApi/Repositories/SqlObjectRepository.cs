using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using ProjectLU2.WebApi.Models;

namespace ProjectLU2.WebApi.Repositories;

public class ObjectRepository : IObjectRepository
{
    private readonly string sqlConnectionString;

    public ObjectRepository(string sqlConnectionString)
    {
        this.sqlConnectionString = sqlConnectionString;
    }

    public async Task<Object2D> InsertAsync(Object2D obj)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            var objId = await sqlConnection.ExecuteAsync("INSERT INTO [Object2D] (Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) VALUES (@Id, @EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)", obj);
            return obj;
        }
    }

    public async Task<Object2D?> ReadAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QuerySingleOrDefaultAsync<Object2D>("SELECT * FROM [Object2D] WHERE Id = @Id", new { id });
        }
    }

    public async Task<IEnumerable<Object2D>> ReadByEnvironmentIdAsync(Guid environmentId)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE EnvironmentId = @EnvironmentId", new { EnvironmentId = environmentId });
        }
    }

    public async Task<IEnumerable<Object2D>> ReadAllAsync()
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D]");
        }
    }

    public async Task UpdateAsync(Object2D obj)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("UPDATE [Object2D] SET EnvironmentId = @EnvironmentId, PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY, ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer WHERE Id = @Id", obj);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("DELETE FROM [Object2D] WHERE Id = @Id", new { id });
        }
    }
}
