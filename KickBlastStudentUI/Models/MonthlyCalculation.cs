using KickBlastStudentUI.Helpers;

namespace KickBlastStudentUI.Models;

public class MonthlyCalculation
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public Athlete? Athlete { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TrainingCost { get; set; }
    public decimal CoachingCost { get; set; }
    public decimal CompetitionCost { get; set; }
    public decimal TotalCost { get; set; }
    public int CompetitionsCount { get; set; }
    public decimal CoachingHoursPerWeek { get; set; }
    public string WeightStatusMessage { get; set; } = string.Empty;
    public DateTime SecondSaturdayDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TotalCostDisplay => CurrencyHelper.Format(TotalCost);
}
