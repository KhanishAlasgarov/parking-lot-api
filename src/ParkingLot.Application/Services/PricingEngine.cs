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
        // 4. Return 0 if entryTime == exitTime (edge case for immediate exit)
        if (entryTime >= exitTime) return 0m;

        var rates = (await _rateRepo.GetRatesForLotAsync(lotId, ct)).ToList();
        if (!rates.Any()) return 0m;

        // 1. Minimum charge: if duration < 30 minutes → charge 1 full hour minimum
        var duration = exitTime - entryTime;
        if (duration < TimeSpan.FromMinutes(30))
        {
            exitTime = entryTime.AddHours(1);
        }

        var totalFee = 0m;
        var current = entryTime;

        // 2. Midnight crossing: split into day segments and calculate each
        while (current.Date < exitTime.Date)
        {
            var nextMidnight = current.Date.AddDays(1);
            totalFee += CalculateFeeForSegment(rates, current, nextMidnight);
            current = nextMidnight;
        }

        if (current < exitTime)
        {
            totalFee += CalculateFeeForSegment(rates, current, exitTime);
        }

        return totalFee;
    }

    private static decimal CalculateFeeForSegment(List<ParkingRate> rates, DateTime start, DateTime end)
    {
        var fee = 0m;
        var current = start;

        while (current < end)
        {
            var hourOfDay = current.Hour;
            var rate = FindRateForHour(rates, hourOfDay, current);

            if (rate is not null)
            {
                fee += rate.RatePerHour;
            }

            current = current.AddHours(1);
        }

        return fee;
    }

    private static ParkingRate? FindRateForHour(List<ParkingRate> rates, int hourOfDay, DateTime currentTime)
    {
        return rates
            .Where(r => r.EffectiveFrom <= currentTime &&
                        r.HourFrom <= hourOfDay &&
                        r.HourTo > hourOfDay)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefault()
            ?? rates
                .Where(r => r.EffectiveFrom <= currentTime)
                .OrderByDescending(r => r.HourFrom)     // 3. Last defined rate (highest HourFrom)
                .ThenByDescending(r => r.EffectiveFrom)
                .FirstOrDefault();
    }
}
