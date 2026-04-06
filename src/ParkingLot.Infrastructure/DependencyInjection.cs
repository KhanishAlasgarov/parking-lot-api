using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkingLot.Domain.Interfaces;
using ParkingLot.Infrastructure.Persistence;
using ParkingLot.Infrastructure.Repositories;

namespace ParkingLot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IParkingSpotRepository, ParkingSpotRepository>();
        services.AddScoped<IParkingTicketRepository, ParkingTicketRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IParkingRateRepository, ParkingRateRepository>();

        return services;
    }
}
