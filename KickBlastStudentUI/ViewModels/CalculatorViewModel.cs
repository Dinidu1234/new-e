using System;
using System.Collections.ObjectModel;
using System.Linq;
using KickBlastStudentUI.Helpers;
using KickBlastStudentUI.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastStudentUI.ViewModels;

public class CalculatorViewModel : ObservableObject
{
    public ObservableCollection<Athlete> Athletes { get; } = new();
    private Athlete? _selectedAthlete;
    private int _competitionsCount;
    private decimal _coachingHoursPerWeek;
    private MonthlyCalculation? _currentCalculation;
    private string _beginnerCompetitionNote = string.Empty;

    public CalculatorViewModel()
    {
        LoadAthletes();
        CalculateCommand = new RelayCommand(_ => Calculate());
        SaveCommand = new RelayCommand(_ => Save(), _ => CanSave);

        App.AppEvents.AthleteChanged += LoadAthletes;
        App.AppEvents.PricingUpdated += () => App.ToastService.Show("Pricing reloaded for calculator.");
    }

    public Athlete? SelectedAthlete { get => _selectedAthlete; set => SetProperty(ref _selectedAthlete, value); }
    public int CompetitionsCount { get => _competitionsCount; set => SetProperty(ref _competitionsCount, value < 0 ? 0 : value); }
    public decimal CoachingHoursPerWeek { get => _coachingHoursPerWeek; set => SetProperty(ref _coachingHoursPerWeek, Math.Clamp(value, 0, 5)); }

    public string BeginnerCompetitionNote { get => _beginnerCompetitionNote; set => SetProperty(ref _beginnerCompetitionNote, value); }
    public string TrainingCostDisplay => $"Training: {CurrencyHelper.Format(_currentCalculation?.TrainingCost ?? 0)}";
    public string CoachingCostDisplay => $"Coaching: {CurrencyHelper.Format(_currentCalculation?.CoachingCost ?? 0)}";
    public string CompetitionCostDisplay => $"Competition: {CurrencyHelper.Format(_currentCalculation?.CompetitionCost ?? 0)}";
    public string TotalCostDisplay => $"Total: {CurrencyHelper.Format(_currentCalculation?.TotalCost ?? 0)}";
    public string WeightStatusDisplay => $"Weight status: {_currentCalculation?.WeightStatusMessage ?? "-"}";
    public string SecondSaturdayDisplay => _currentCalculation == null ? "Second Saturday: -" : $"Second Saturday: {_currentCalculation.SecondSaturdayDate:dd MMM yyyy}";
    public bool CanSave => _currentCalculation != null;

    public RelayCommand CalculateCommand { get; }
    public RelayCommand SaveCommand { get; }

    private void LoadAthletes()
    {
        Athletes.Clear();
        foreach (var item in App.DbContext.Athletes.Include(x => x.TrainingPlan).ToList())
            Athletes.Add(item);
        if (SelectedAthlete == null) SelectedAthlete = Athletes.FirstOrDefault();
    }

    private void Calculate()
    {
        if (SelectedAthlete == null) return;
        var calc = App.FeeCalculatorService.Calculate(SelectedAthlete, CompetitionsCount, CoachingHoursPerWeek);
        _currentCalculation = calc;
        BeginnerCompetitionNote = SelectedAthlete.TrainingPlan?.Name == "Beginner" ? "Beginner plan: competitions set to 0 automatically." : string.Empty;
        SaveCommand.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(TrainingCostDisplay));
        OnPropertyChanged(nameof(CoachingCostDisplay));
        OnPropertyChanged(nameof(CompetitionCostDisplay));
        OnPropertyChanged(nameof(TotalCostDisplay));
        OnPropertyChanged(nameof(WeightStatusDisplay));
        OnPropertyChanged(nameof(SecondSaturdayDisplay));
    }

    private void Save()
    {
        if (_currentCalculation == null || SelectedAthlete == null) return;

        try
        {
            _currentCalculation.AthleteId = SelectedAthlete.Id;
            _currentCalculation.CreatedAt = DateTime.Now;
            App.DbContext.MonthlyCalculations.Add(_currentCalculation);
            App.DbContext.SaveChanges();
            App.AppEvents.PublishCalculationSaved();
            App.ToastService.Show("Calculation saved.");
        }
        catch
        {
            App.ToastService.Show("Could not save calculation.");
        }
    }
}
