using ParkingLot.Application.Interfaces;
using ParkingLot.Domain.Entities;
using ParkingLot.Domain.Interfaces;

namespace ParkingLot.Application.Services;

public class PricingEngine : IPricingEngine
{
    private readonly IParkingRateRepository _rateRepo;

    public PricingEngine(IParkingRateRepository rateRepo)
    {
        _rateRepo = rateRepo;
    }

    public async Task<decimal> CalculateFeeAsync(Guid lotId, DateTime entryTime, DateTime exitTime, CancellationToken ct = default)
    {
        var rates = (await _rateRepo.GetRatesForLotAsync(lotId, ct)).ToList();

        if (!rates.Any())
        {
            // Fallback: no rates configured, return 0
            return 0m;
        }

        var totalFee = 0m;
        var current = entryTime;

        // Calculate hour-by-hour
        while (current < exitTime)
        {
            var nextHour = current.AddHours(1);
            if (nextHour > exitTime)
                nextHour = exitTime;

            var hourOfDay = current.Hour;

            // Find the matching rate for this hour
            var rate = FindRateForHour(rates, hourOfDay, current);

            if (rate is not null)
            {
                // Ceiling: any partial hour counts as a full hour
                totalFee += rate.RatePerHour;
            }

            current = current.AddHours(1);
        }

        return totalFee;
    }

    private static ParkingRate? FindRateForHour(List<ParkingRate> rates, int hourOfDay, DateTime currentTime)
    {
        // Find the most recent effective rate that covers this hour
        return rates
            .Where(r => r.EffectiveFrom <= currentTime &&
                        r.HourFrom <= hourOfDay &&
                        r.HourTo > hourOfDay)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefault()
            ?? rates
                .Where(r => r.EffectiveFrom <= currentTime)
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefault();
    }
}
