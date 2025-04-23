
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
                await DisplayAlert("Ошибка", "Имя пользователя и пароль не могут быть пустыми.", "OK");
                return;
            }

            var success = await _authService.LoginAsync(entryEmail.Text, entryPassword.Text);

            if (success)
            {
                if (Application.Current.MainPage is AppShell appShell)
                {
                    appShell.UpdateUsername();
                }
                await DisplayAlert("Успех", "Вход выполнен успешно!", "OK");
                await Shell.Current.GoToAsync("//books");
            }
            else
            {
                await DisplayAlert("Ошибка", "Неверное имя пользователя или пароль.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при входе: {ex.Message}");
            await DisplayAlert("Ошибка", "Произошла ошибка при входе. Попробуйте еще раз.", "OK");
        }
    }
    private async void OnRegisterTapped(object sender, EventArgs e)
	{
        var button = (Button)sender;

        await button.ScaleTo(0.9, 100);
        await button.ScaleTo(1.0, 100);

        await Shell.Current.GoToAsync("//register");
    }
}