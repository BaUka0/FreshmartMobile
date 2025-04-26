using Project.Models;
using Project.Services;

namespace Project.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly DatabaseService _databaseService;
    public RegisterPage(AuthService authService, DatabaseService databaseService)
    {
        InitializeComponent();
        _authService = authService;
        _databaseService = databaseService;
    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        await button.ScaleTo(0.9, 100);
        await button.ScaleTo(1.0, 100);

        try
        {

            if (string.IsNullOrWhiteSpace(entryUsername.Text) || string.IsNullOrWhiteSpace(entryPassword.Text) || string.IsNullOrWhiteSpace(entryEmail.Text))
            {
                await DisplayAlert("Ошибка", "Имя пользователя и пароль не могут быть пустыми.", "OK");
                return;
            }

            var result = await _authService.RegisterAsync(entryUsername.Text, entryEmail.Text, entryPassword.Text);

            string role = radioClient.IsChecked ? "client" : "seller";

            if (result)
            {
                await DisplayAlert("Успех", "Регистрация прошла успешно!", "OK");
            }
            if (role == "seller")
            {
                var user = await _databaseService.GetUserByCredentialsAsync(entryUsername.Text, entryPassword.Text);
                var application = new SellerApplication
                {
                    UserId = user.Id,
                    ApplicationDate = DateTime.UtcNow,
                    Status = "Pending"
                };
                await _databaseService.CreateSellerApplicationAsync(application);
                await DisplayAlert("Успех", "Заявка на продавца отправлена на рассмотрение.", "OK");
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