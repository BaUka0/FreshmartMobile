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

        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}