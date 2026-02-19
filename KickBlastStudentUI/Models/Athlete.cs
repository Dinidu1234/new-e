namespace KickBlastStudentUI.Models;

public class Athlete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TrainingPlanId { get; set; }
    public TrainingPlan? TrainingPlan { get; set; }
    public decimal CurrentWeightKg { get; set; }
    public decimal CompetitionCategoryKg { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<MonthlyCalculation> MonthlyCalculations { get; set; } = new List<MonthlyCalculation>();
}
