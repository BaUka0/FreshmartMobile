using Project.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Pages;

public partial class SplashPage : ContentPage
{
	public SplashPage()
	{
		InitializeComponent();
		Animate();
	}

	private async void Animate()
	{
        await Task.Delay(1000);

        var appShell = Application.Current.Handler.MauiContext.Services.GetService<AppShell>();


        Application.Current.MainPage = appShell; 
    }
}