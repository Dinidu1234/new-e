using KickBlastStudentUI.Helpers;
using System.Windows;

namespace KickBlastStudentUI.ViewModels;

public class MainViewModel : ObservableObject
{
    private object _currentViewModel;
    private string _toastMessage = "Ready";

    public MainViewModel()
    {
        DashboardVM = new DashboardViewModel();
        AthletesVM = new AthletesViewModel();
        CalculatorVM = new CalculatorViewModel();
        HistoryVM = new HistoryViewModel();
        SettingsVM = new SettingsViewModel();

        _currentViewModel = DashboardVM;

        NavigateDashboardCommand = new RelayCommand(_ => CurrentViewModel = DashboardVM);
        NavigateAthletesCommand = new RelayCommand(_ => CurrentViewModel = AthletesVM);
        NavigateCalculatorCommand = new RelayCommand(_ => CurrentViewModel = CalculatorVM);
        NavigateHistoryCommand = new RelayCommand(_ => CurrentViewModel = HistoryVM);
        NavigateSettingsCommand = new RelayCommand(_ => CurrentViewModel = SettingsVM);
        LogoutCommand = new RelayCommand(_ => Logout());

        App.ToastService.MessageChanged += message => ToastMessage = message;
    }

    public object CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }
    public string ToastMessage { get => _toastMessage; set => SetProperty(ref _toastMessage, value); }

    public DashboardViewModel DashboardVM { get; }
    public AthletesViewModel AthletesVM { get; }
    public CalculatorViewModel CalculatorVM { get; }
    public HistoryViewModel HistoryVM { get; }
    public SettingsViewModel SettingsVM { get; }

    public RelayCommand NavigateDashboardCommand { get; }
    public RelayCommand NavigateAthletesCommand { get; }
    public RelayCommand NavigateCalculatorCommand { get; }
    public RelayCommand NavigateHistoryCommand { get; }
    public RelayCommand NavigateSettingsCommand { get; }
    public RelayCommand LogoutCommand { get; }

    private void Logout()
    {
        App.AuthService.Logout();
        var login = new Views.LoginWindow();
        login.Show();
        foreach (Window w in Application.Current.Windows)
        {
            if (w is MainWindow)
            {
                w.Close();
                break;
            }
        }
    }
}
