using Project.Models;

namespace Project.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}
    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        await button.ScaleTo(0.9, 100);
        await button.ScaleTo(1.0, 100);

        try
        {
            var user = new User
            {
                username = usernameEntry.Text,
                password = passwordEntry.Text
            };

            if (string.IsNullOrWhiteSpace(user.username) || string.IsNullOrWhiteSpace(user.password))
            {
                await DisplayAlert("Ошибка", "Имя пользователя и пароль не могут быть пустыми.", "OK");
                return;
            }

            await App.DatabaseService.AddUserAsync(user);

            await DisplayAlert("Успех", "Пользователь успешно зарегистрирован!", "OK");
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при регистрации пользователя: {ex.Message}");

            await DisplayAlert("Ошибка", "Произошла ошибка при регистрации. Попробуйте еще раз.", "OK");
        }
    }
}