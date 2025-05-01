using Microsoft.Extensions.Logging;
using Project.Pages;
using Project.Pages.Admin;
using Project.Pages.Client;
using Project.Pages.Seller;
using Project.Services;

namespace Project;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        // Dependency Injection Services
        builder.Services.AddSingleton<DatabaseService>();
		builder.Services.AddSingleton<AuthService>();

        // Registering Pages
        builder.Services.AddSingleton<SplashPage>();
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<UserListPage>();
        builder.Services.AddTransient<SellerApplicationsPage>();

        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<AddProductPage>();

        builder.Services.AddTransient<ProductDetail>();
        builder.Services.AddTransient<ProductListPage>();
        builder.Services.AddTransient<CatalogPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<FavouritePage>();




        return builder.Build();
	}
}
