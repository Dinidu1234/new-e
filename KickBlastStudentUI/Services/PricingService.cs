using System.IO;
using System.Text.Json;
using KickBlastStudentUI.Data;
using Microsoft.Extensions.Configuration;

namespace KickBlastStudentUI.Services;

public class PricingSettings
{
    public decimal BeginnerWeeklyFee { get; set; }
    public decimal IntermediateWeeklyFee { get; set; }
    public decimal EliteWeeklyFee { get; set; }
    public decimal CompetitionFee { get; set; }
    public decimal CoachingHourlyRate { get; set; }
}

public class PricingService
{
    private readonly IConfigurationRoot _configuration;
    private readonly AppDbContext _db;
    private readonly AppEvents _events;

    public PricingService(IConfigurationRoot configuration, AppDbContext db, AppEvents events)
    {
        _configuration = configuration;
        _db = db;
        _events = events;
    }

    public PricingSettings GetPricing()
    {
        return new PricingSettings
        {
            BeginnerWeeklyFee = _configuration.GetValue<decimal>("Pricing:BeginnerWeeklyFee"),
            IntermediateWeeklyFee = _configuration.GetValue<decimal>("Pricing:IntermediateWeeklyFee"),
            EliteWeeklyFee = _configuration.GetValue<decimal>("Pricing:EliteWeeklyFee"),
            CompetitionFee = _configuration.GetValue<decimal>("Pricing:CompetitionFee"),
            CoachingHourlyRate = _configuration.GetValue<decimal>("Pricing:CoachingHourlyRate")
        };
    }

    public void SavePricing(PricingSettings settings)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        var wrapper = new { Pricing = settings };
        var json = JsonSerializer.Serialize(wrapper, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
        _configuration.Reload();

        foreach (var plan in _db.TrainingPlans)
        {
            if (plan.Name == "Beginner") plan.WeeklyFee = settings.BeginnerWeeklyFee;
            if (plan.Name == "Intermediate") plan.WeeklyFee = settings.IntermediateWeeklyFee;
            if (plan.Name == "Elite") plan.WeeklyFee = settings.EliteWeeklyFee;
        }
        _db.SaveChanges();
        _events.PublishPricingUpdated();
    }
}
