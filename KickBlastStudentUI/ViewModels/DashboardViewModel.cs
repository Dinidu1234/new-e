using System;
using System.Collections.ObjectModel;
using System.Linq;
using KickBlastStudentUI.Helpers;

namespace KickBlastStudentUI.ViewModels;

public class DashboardViewModel : ObservableObject
{
    private int _totalAthletes;
    private int _monthCalculationCount;
    private string _monthRevenue = "LKR 0.00";

    public DashboardViewModel()
    {
        LatestUpdates = new ObservableCollection<string>();
        Refresh();

        App.AppEvents.AthleteChanged += () => { Refresh(); AddUpdate("Athlete records updated."); };
        App.AppEvents.CalculationSaved += () => { Refresh(); AddUpdate("New monthly calculation saved."); };
    }

    public int TotalAthletes { get => _totalAthletes; set => SetProperty(ref _totalAthletes, value); }
    public int MonthCalculationCount { get => _monthCalculationCount; set => SetProperty(ref _monthCalculationCount, value); }
    public string MonthRevenue { get => _monthRevenue; set => SetProperty(ref _monthRevenue, value); }
    public ObservableCollection<string> LatestUpdates { get; }

    private void Refresh()
    {
        var now = DateTime.Now;
        TotalAthletes = App.DbContext.Athletes.Count();
        MonthCalculationCount = App.DbContext.MonthlyCalculations.Count(x => x.Month == now.Month && x.Year == now.Year);
        var sum = App.DbContext.MonthlyCalculations
            .Where(x => x.Month == now.Month && x.Year == now.Year)
            .Select(x => x.TotalCost)
            .DefaultIfEmpty(0m)
            .Sum();
        MonthRevenue = CurrencyHelper.Format(sum);
    }

    private void AddUpdate(string text)
    {
        LatestUpdates.Insert(0, $"{DateTime.Now:t} - {text}");
        if (LatestUpdates.Count > 8)
        {
            LatestUpdates.RemoveAt(LatestUpdates.Count - 1);
        }
    }
}
