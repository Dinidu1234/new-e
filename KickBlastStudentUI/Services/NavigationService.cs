namespace KickBlastStudentUI.Services;

public class NavigationService
{
    public event Action<string>? ViewChanged;
    public void Navigate(string viewName) => ViewChanged?.Invoke(viewName);
}
