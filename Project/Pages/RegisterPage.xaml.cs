using Project.Models;
using Project.Services;

namespace Project.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;
    public RegisterPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
    }
    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        await button.ScaleTo(0.9, 100);
        await button.ScaleTo(1.0, 100);

        try
        {

            if (string.IsNullOrWhiteSpace(usernameEntry.Text) || string.IsNullOrWhiteSpace(passwordEntry.Text))
            {
                await DisplayAlert("Ошибка", "Имя пользователя и пароль не могут быть пустыми.", "OK");
                return;
            }

            var result = await _authService.RegisterAsync(usernameEntry.Text, passwordEntry.Text);
            if(result)
            {
                await DisplayAlert("Успех", "Регистрация прошла успешно!", "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Пользователь с таким именем уже существует.", "OK");
                return;
            }

            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при регистрации пользователя: {ex.Message}");

            await DisplayAlert("Ошибка", "Произошла ошибка при регистрации. Попробуйте еще раз.", "OK");
        }
    }
}