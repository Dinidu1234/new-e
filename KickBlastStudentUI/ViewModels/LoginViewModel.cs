using System.Threading.Tasks;
using System.Windows;
using KickBlastStudentUI.Helpers;

namespace KickBlastStudentUI.ViewModels;

public class LoginViewModel : ObservableObject
{
    private readonly Window _window;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public LoginViewModel(Window window)
    {
        _window = window;
        LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin);
    }

    public string Username { get => _username; set { SetProperty(ref _username, value); OnPropertyChanged(nameof(CanLogin)); } }
    public string Password { get => _password; set { SetProperty(ref _password, value); OnPropertyChanged(nameof(CanLogin)); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool CanLogin => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    public RelayCommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        var success = await App.AuthService.LoginAsync(Username, Password);
        if (!success)
        {
            ErrorMessage = "Invalid username or password";
            return;
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
        _window.Close();
    }
}
