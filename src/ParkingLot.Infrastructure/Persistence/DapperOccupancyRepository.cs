using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ParkingLot.Application.Interfaces;

namespace ParkingLot.Infrastructure.Persistence;

public class DapperOccupancyRepository : IDapperOccupancyRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperOccupancyRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<int> GetFreeSpotCountAsync(Guid floorId)
    {
        const string sql = "SELECT COUNT(*) FROM \"ParkingSpots\" WHERE \"FloorId\" = @floorId AND \"Status\" = 'Free'";
        
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        return await connection.ExecuteScalarAsync<int>(sql, new { floorId });
    }
}
