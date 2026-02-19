using System.IO;
using System.Windows;
using KickBlastStudentUI.Data;
using KickBlastStudentUI.Services;
using Microsoft.Extensions.Configuration;

namespace KickBlastStudentUI;

public partial class App : Application
{
    public static IConfigurationRoot Configuration { get; private set; } = default!;
    public static AppDbContext DbContext { get; private set; } = default!;
    public static AuthService AuthService { get; private set; } = default!;
    public static PricingService PricingService { get; private set; } = default!;
    public static FeeCalculatorService FeeCalculatorService { get; private set; } = default!;
    public static NavigationService NavigationService { get; private set; } = default!;
    public static ToastService ToastService { get; private set; } = default!;
    public static AppEvents AppEvents { get; private set; } = default!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var basePath = Directory.GetCurrentDirectory();
        Configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        DbContext = new AppDbContext();
        DbContext.Database.EnsureCreated();
        DbInitializer.Seed(DbContext, Configuration);

        AppEvents = new AppEvents();
        ToastService = new ToastService();
        PricingService = new PricingService(Configuration, DbContext, AppEvents);
        AuthService = new AuthService(DbContext);
        FeeCalculatorService = new FeeCalculatorService(PricingService);
        NavigationService = new NavigationService();

        var loginWindow = new Views.LoginWindow();
        loginWindow.Show();
    }
}
