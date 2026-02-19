using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using KickBlastStudentUI.Helpers;
using KickBlastStudentUI.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastStudentUI.ViewModels;

public class HistoryViewModel : ObservableObject
{
    public ObservableCollection<MonthlyCalculation> Calculations { get; } = new();
    public ObservableCollection<string> AthleteFilters { get; } = new() { "All" };
    public ObservableCollection<int> MonthFilters { get; } = new();
    public ObservableCollection<int> YearFilters { get; } = new();

    public ICollectionView CalculationsView { get; }

    private MonthlyCalculation? _selectedCalculation;
    private string _selectedAthleteFilter = "All";
    private int _selectedMonthFilter;
    private int _selectedYearFilter;

    public HistoryViewModel()
    {
        LoadData();
        CalculationsView = CollectionViewSource.GetDefaultView(Calculations);
        CalculationsView.Filter = FilterCalculation;

        App.AppEvents.CalculationSaved += LoadData;
        App.AppEvents.AthleteChanged += LoadData;
    }

    public MonthlyCalculation? SelectedCalculation { get => _selectedCalculation; set { SetProperty(ref _selectedCalculation, value); OnPropertyChanged(nameof(DetailText)); } }
    public string SelectedAthleteFilter { get => _selectedAthleteFilter; set { SetProperty(ref _selectedAthleteFilter, value); CalculationsView.Refresh(); } }
    public int SelectedMonthFilter { get => _selectedMonthFilter; set { SetProperty(ref _selectedMonthFilter, value); CalculationsView.Refresh(); } }
    public int SelectedYearFilter { get => _selectedYearFilter; set { SetProperty(ref _selectedYearFilter, value); CalculationsView.Refresh(); } }

    public string DetailText => SelectedCalculation == null
        ? "Select a record to see full breakdown."
        : $"Athlete: {SelectedCalculation.Athlete?.Name}
Training: {CurrencyHelper.Format(SelectedCalculation.TrainingCost)}
Coaching: {CurrencyHelper.Format(SelectedCalculation.CoachingCost)}
Competition: {CurrencyHelper.Format(SelectedCalculation.CompetitionCost)}
Total: {CurrencyHelper.Format(SelectedCalculation.TotalCost)}
Weight: {SelectedCalculation.WeightStatusMessage}
Second Saturday: {SelectedCalculation.SecondSaturdayDate:dd MMM yyyy}";

    private void LoadData()
    {
        var list = App.DbContext.MonthlyCalculations.Include(x => x.Athlete).ThenInclude(a => a.TrainingPlan).OrderByDescending(x => x.CreatedAt).ToList();
        Calculations.Clear();
        foreach (var item in list)
            Calculations.Add(item);

        AthleteFilters.Clear();
        AthleteFilters.Add("All");
        foreach (var athlete in App.DbContext.Athletes.Select(x => x.Name).OrderBy(x => x).ToList())
            AthleteFilters.Add(athlete);

        MonthFilters.Clear();
        for (int i = 1; i <= 12; i++) MonthFilters.Add(i);
        YearFilters.Clear();
        var years = list.Select(x => x.Year).Distinct().OrderByDescending(x => x).ToList();
        if (years.Count == 0) years.Add(DateTime.Now.Year);
        foreach (var year in years) YearFilters.Add(year);

        if (SelectedMonthFilter == 0) SelectedMonthFilter = DateTime.Now.Month;
        if (SelectedYearFilter == 0) SelectedYearFilter = DateTime.Now.Year;

        CalculationsView?.Refresh();
    }

    private bool FilterCalculation(object obj)
    {
        if (obj is not MonthlyCalculation calc) return false;
        var athleteOk = SelectedAthleteFilter == "All" || calc.Athlete?.Name == SelectedAthleteFilter;
        var monthOk = SelectedMonthFilter == 0 || calc.Month == SelectedMonthFilter;
        var yearOk = SelectedYearFilter == 0 || calc.Year == SelectedYearFilter;
        return athleteOk && monthOk && yearOk;
    }
}
