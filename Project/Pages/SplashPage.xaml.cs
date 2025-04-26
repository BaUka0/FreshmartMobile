using Project.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Pages;

public partial class SplashPage : ContentPage
{
	private readonly AuthService _authService;
	public SplashPage(AuthService authService)
	{
		InitializeComponent();
		Animate();
        _authService = authService;
    }

	private async void Animate()
	{
        await Task.Delay(1000);

        Application.Current.MainPage = new AppShell(_authService); 
    }
}