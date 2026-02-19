using KickBlastStudentUI.Helpers;

namespace KickBlastStudentUI.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private decimal _beginnerWeeklyFee;
    private decimal _intermediateWeeklyFee;
    private decimal _eliteWeeklyFee;
    private decimal _competitionFee;
    private decimal _coachingHourlyRate;

    public SettingsViewModel()
    {
        Load();
        SavePricingCommand = new RelayCommand(_ => Save());
    }

    public decimal BeginnerWeeklyFee { get => _beginnerWeeklyFee; set => SetProperty(ref _beginnerWeeklyFee, value); }
    public decimal IntermediateWeeklyFee { get => _intermediateWeeklyFee; set => SetProperty(ref _intermediateWeeklyFee, value); }
    public decimal EliteWeeklyFee { get => _eliteWeeklyFee; set => SetProperty(ref _eliteWeeklyFee, value); }
    public decimal CompetitionFee { get => _competitionFee; set => SetProperty(ref _competitionFee, value); }
    public decimal CoachingHourlyRate { get => _coachingHourlyRate; set => SetProperty(ref _coachingHourlyRate, value); }

    public RelayCommand SavePricingCommand { get; }

    private void Load()
    {
        var pricing = App.PricingService.GetPricing();
        BeginnerWeeklyFee = pricing.BeginnerWeeklyFee;
        IntermediateWeeklyFee = pricing.IntermediateWeeklyFee;
        EliteWeeklyFee = pricing.EliteWeeklyFee;
        CompetitionFee = pricing.CompetitionFee;
        CoachingHourlyRate = pricing.CoachingHourlyRate;
    }

    private void Save()
    {
        App.PricingService.SavePricing(new Services.PricingSettings
        {
            BeginnerWeeklyFee = BeginnerWeeklyFee,
            IntermediateWeeklyFee = IntermediateWeeklyFee,
            EliteWeeklyFee = EliteWeeklyFee,
            CompetitionFee = CompetitionFee,
            CoachingHourlyRate = CoachingHourlyRate
        });
        App.ToastService.Show("Pricing saved and reloaded.");
    }
}
