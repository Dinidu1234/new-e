using KickBlastStudentUI.Helpers;
using KickBlastStudentUI.Models;

namespace KickBlastStudentUI.Services;

public class FeeCalculatorService
{
    private readonly PricingService _pricingService;

    public FeeCalculatorService(PricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public MonthlyCalculation Calculate(Athlete athlete, int competitionsThisMonth, decimal coachingHoursPerWeek)
    {
        var pricing = _pricingService.GetPricing();
        var planName = athlete.TrainingPlan?.Name ?? string.Empty;

        var trainingFee = athlete.TrainingPlan?.WeeklyFee ?? 0;
        var trainingCost = trainingFee * 4;
        var coachingCost = coachingHoursPerWeek * 4 * pricing.CoachingHourlyRate;

        var allowedCompetitions = planName == "Beginner" ? 0 : Math.Max(0, competitionsThisMonth);
        var competitionCost = allowedCompetitions * pricing.CompetitionFee;

        var diff = athlete.CurrentWeightKg - athlete.CompetitionCategoryKg;
        var weightStatus = diff switch
        {
            > 0 => $"Over target by {diff:0.00} kg",
            < 0 => $"Under target by {Math.Abs(diff):0.00} kg",
            _ => "On target"
        };

        return new MonthlyCalculation
        {
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year,
            TrainingCost = trainingCost,
            CoachingCost = coachingCost,
            CompetitionCost = competitionCost,
            TotalCost = trainingCost + coachingCost + competitionCost,
            CompetitionsCount = allowedCompetitions,
            CoachingHoursPerWeek = coachingHoursPerWeek,
            WeightStatusMessage = weightStatus,
            SecondSaturdayDate = DateHelper.GetSecondSaturday(DateTime.Now.Year, DateTime.Now.Month)
        };
    }
}
