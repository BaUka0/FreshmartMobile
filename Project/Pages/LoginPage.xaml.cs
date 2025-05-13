
using Project.Models;
using Project.Services;
namespace Project.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    public LoginPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
    }

	private async void OnLoginClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        await button.ScaleTo(0.9, 100);
        await button.ScaleTo(1.0, 100);

        try
        {
            if (string.IsNullOrWhiteSpace(entryEmail.Text) || string.IsNullOrWhiteSpace(entryPassword.Text))
            {
                await DisplayAlert("Қате!", "Пайдаланушы аты мен құпия сөз бос болмауы керек.", "OK");
                return;
            }

            var success = await _authService.LoginAsync(entryEmail.Text, entryPassword.Text);

            if (success)
            {
                if (Application.Current.MainPage is AppShell appShell)
                {
                    appShell.OnUserLoggedIn();
                }
                await DisplayAlert("Сәтті!", "Сіз жүйеге сәтті кірдіңіз!", "OK");
                //await Shell.Current.GoToAsync("/home");
            }
            else
            {
                await DisplayAlert("Қате!", "Пайдаланушы аты немесе құпиясөз дұрыс емес.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Кіру кезінде қате: {ex.Message}");
            await DisplayAlert("Қате", "Кіру кезінде қате орын алды. Қайтадан көріңіз.", "OK");
        }
    }
    private async void OnRegisterTapped(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync("register");
    }
}