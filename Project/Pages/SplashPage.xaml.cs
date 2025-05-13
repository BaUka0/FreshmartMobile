using Project.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Pages;

public partial class SplashPage : ContentPage
{
    private readonly AuthService _authService;
    private bool _isAnimating = true;

    public SplashPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        StartLoadingAnimation();
        NavigateToNextPage();
    }

    private async void StartLoadingAnimation()
    {
        string baseText = "Жүктелуде";
        int dotCount = 0;

        while (_isAnimating)
        {
            dotCount = (dotCount + 1) % 4;
            LoadingLabel.Text = baseText + new string('.', dotCount);
            await Task.Delay(500);
        }
    }

    private async void NavigateToNextPage()
    {
        await Task.Delay(3000);
        _isAnimating = false;

        await Task.WhenAll(
            LogoImage.ScaleTo(2, 500, Easing.CubicOut),
            LogoImage.FadeTo(0, 500),
            this.FadeTo(0, 500)
        );

        Application.Current.MainPage = new AppShell(_authService);
    }


}
