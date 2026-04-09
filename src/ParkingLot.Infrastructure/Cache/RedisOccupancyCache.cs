using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ParkingLot.Application.DTOs;
using ParkingLot.Application.Interfaces;
using StackExchange.Redis;

namespace ParkingLot.Infrastructure.Cache;

public class RedisOccupancyCache : IOccupancyCache
{
    private readonly IDatabase _database;

    public RedisOccupancyCache(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    private string GetKey(Guid floorId) => $"occupancy:floor:{floorId}";

    public async Task<FloorAvailabilityResponse?> GetAsync(Guid floorId)
    {
        var value = await _database.StringGetAsync(GetKey(floorId));
        if (value.IsNullOrEmpty) return null;

        return JsonSerializer.Deserialize<FloorAvailabilityResponse>(value.ToString());
    }

    public async Task SetAsync(Guid floorId, FloorAvailabilityResponse data, TimeSpan ttl)
    {
        var json = JsonSerializer.Serialize(data);
        await _database.StringSetAsync(GetKey(floorId), json, ttl);
    }

    public async Task InvalidateAsync(Guid floorId)
    {
        await _database.KeyDeleteAsync(GetKey(floorId));
    }
}
