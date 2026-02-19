using System.Windows;
using KickBlastStudentUI.ViewModels;

namespace KickBlastStudentUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
