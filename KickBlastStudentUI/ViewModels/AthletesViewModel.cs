using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows;
using KickBlastStudentUI.Helpers;
using KickBlastStudentUI.Models;
using Microsoft.EntityFrameworkCore;

namespace KickBlastStudentUI.ViewModels;

public class AthletesViewModel : ObservableObject
{
    public ObservableCollection<Athlete> Athletes { get; } = new();
    public ObservableCollection<TrainingPlan> Plans { get; } = new();
    public ObservableCollection<string> PlanFilters { get; } = new() { "All" };

    public ICollectionView AthletesView { get; }

    private Athlete? _selectedAthlete;
    private TrainingPlan? _selectedPlan;
    private string _selectedPlanFilter = "All";
    private string _searchText = string.Empty;
    private string _nameInput = string.Empty;
    private string _currentWeightInput = string.Empty;
    private string _categoryWeightInput = string.Empty;
    private string _nameError = string.Empty;
    private string _currentWeightError = string.Empty;
    private string _categoryWeightError = string.Empty;

    public AthletesViewModel()
    {
        LoadData();
        AthletesView = CollectionViewSource.GetDefaultView(Athletes);
        AthletesView.Filter = FilterAthlete;

        AddCommand = new RelayCommand(_ => AddAthlete(), _ => IsValid);
        UpdateCommand = new RelayCommand(_ => UpdateAthlete(), _ => SelectedAthlete != null && IsValid);
        DeleteCommand = new RelayCommand(_ => DeleteAthlete(), _ => SelectedAthlete != null);
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand UpdateCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public Athlete? SelectedAthlete
    {
        get => _selectedAthlete;
        set
        {
            SetProperty(ref _selectedAthlete, value);
            if (value != null)
            {
                NameInput = value.Name;
                CurrentWeightInput = value.CurrentWeightKg.ToString();
                CategoryWeightInput = value.CompetitionCategoryKg.ToString();
                SelectedPlan = value.TrainingPlan;
            }
        }
    }

    public TrainingPlan? SelectedPlan { get => _selectedPlan; set => SetProperty(ref _selectedPlan, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); AthletesView.Refresh(); } }
    public string SelectedPlanFilter { get => _selectedPlanFilter; set { SetProperty(ref _selectedPlanFilter, value); AthletesView.Refresh(); } }

    public string NameInput { get => _nameInput; set { SetProperty(ref _nameInput, value); Validate(); } }
    public string CurrentWeightInput { get => _currentWeightInput; set { SetProperty(ref _currentWeightInput, value); Validate(); } }
    public string CategoryWeightInput { get => _categoryWeightInput; set { SetProperty(ref _categoryWeightInput, value); Validate(); } }

    public string NameError { get => _nameError; set => SetProperty(ref _nameError, value); }
    public string CurrentWeightError { get => _currentWeightError; set => SetProperty(ref _currentWeightError, value); }
    public string CategoryWeightError { get => _categoryWeightError; set => SetProperty(ref _categoryWeightError, value); }

    public bool IsValid => string.IsNullOrWhiteSpace(NameError) && string.IsNullOrWhiteSpace(CurrentWeightError) && string.IsNullOrWhiteSpace(CategoryWeightError) && SelectedPlan != null;

    private void LoadData()
    {
        var plans = App.DbContext.TrainingPlans.ToList();
        Plans.Clear();
        foreach (var plan in plans)
        {
            Plans.Add(plan);
            PlanFilters.Add(plan.Name);
        }

        var athletes = App.DbContext.Athletes.Include(x => x.TrainingPlan).ToList();
        Athletes.Clear();
        foreach (var athlete in athletes)
        {
            Athletes.Add(athlete);
        }

        SelectedPlan = Plans.FirstOrDefault();
    }

    private bool FilterAthlete(object obj)
    {
        if (obj is not Athlete athlete) return false;
        var searchOk = string.IsNullOrWhiteSpace(SearchText) || athlete.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var planOk = SelectedPlanFilter == "All" || athlete.TrainingPlan?.Name == SelectedPlanFilter;
        return searchOk && planOk;
    }

    private void Validate()
    {
        NameError = Validators.Required(NameInput, "Name");
        CurrentWeightError = Validators.DecimalRange(CurrentWeightInput, 1, 300, "Current weight");
        CategoryWeightError = Validators.DecimalRange(CategoryWeightInput, 1, 300, "Category weight");
        AddCommand.RaiseCanExecuteChanged();
        UpdateCommand.RaiseCanExecuteChanged();
    }

    private void AddAthlete()
    {
        try
        {
            var athlete = new Athlete
            {
                Name = NameInput,
                TrainingPlanId = SelectedPlan!.Id,
                CurrentWeightKg = decimal.Parse(CurrentWeightInput),
                CompetitionCategoryKg = decimal.Parse(CategoryWeightInput),
                CreatedAt = DateTime.Now
            };
            App.DbContext.Athletes.Add(athlete);
            App.DbContext.SaveChanges();
            athlete.TrainingPlan = SelectedPlan;
            Athletes.Add(athlete);
            App.AppEvents.PublishAthleteChanged();
            App.ToastService.Show("Athlete added");
            ClearForm();
        }
        catch (Exception)
        {
            App.ToastService.Show("Could not add athlete.");
        }
    }

    private void UpdateAthlete()
    {
        if (SelectedAthlete == null) return;
        try
        {
            var entity = App.DbContext.Athletes.Include(x => x.TrainingPlan).First(x => x.Id == SelectedAthlete.Id);
            entity.Name = NameInput;
            entity.CurrentWeightKg = decimal.Parse(CurrentWeightInput);
            entity.CompetitionCategoryKg = decimal.Parse(CategoryWeightInput);
            entity.TrainingPlanId = SelectedPlan!.Id;
            App.DbContext.SaveChanges();

            SelectedAthlete.Name = entity.Name;
            SelectedAthlete.CurrentWeightKg = entity.CurrentWeightKg;
            SelectedAthlete.CompetitionCategoryKg = entity.CompetitionCategoryKg;
            SelectedAthlete.TrainingPlan = SelectedPlan;
            AthletesView.Refresh();
            App.AppEvents.PublishAthleteChanged();
            App.ToastService.Show("Athlete updated");
        }
        catch
        {
            App.ToastService.Show("Could not update athlete.");
        }
    }

    private void DeleteAthlete()
    {
        if (SelectedAthlete == null) return;
        var result = MessageBox.Show($"Delete {SelectedAthlete.Name}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            var entity = App.DbContext.Athletes.First(x => x.Id == SelectedAthlete.Id);
            App.DbContext.Athletes.Remove(entity);
            App.DbContext.SaveChanges();
            Athletes.Remove(SelectedAthlete);
            App.AppEvents.PublishAthleteChanged();
            App.ToastService.Show("Athlete deleted");
            ClearForm();
        }
        catch
        {
            App.ToastService.Show("Could not delete athlete.");
        }
    }

    private void ClearForm()
    {
        NameInput = string.Empty;
        CurrentWeightInput = string.Empty;
        CategoryWeightInput = string.Empty;
        SelectedAthlete = null;
    }
}
