using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ParkingLot.Application.Interfaces;
using ParkingLot.Application.Services;
using ParkingLot.Application.Validators;

namespace ParkingLot.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IPricingEngine, PricingEngine>();
        services.AddScoped<IOccupancyService, OccupancyService>();
        
        services.AddValidatorsFromAssemblyContaining<IssueTicketRequestValidator>();
        
        return services;
    }
}
