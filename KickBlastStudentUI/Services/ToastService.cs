namespace KickBlastStudentUI.Services;

public class ToastService
{
    public event Action<string>? MessageChanged;
    public void Show(string message) => MessageChanged?.Invoke(message);
}
