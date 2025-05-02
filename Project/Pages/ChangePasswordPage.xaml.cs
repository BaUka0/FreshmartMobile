using Project.Services;

namespace Project.Pages;

public partial class ChangePasswordPage : ContentPage
{
    private readonly AuthService _authService;

    public ChangePasswordPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string currentPassword = CurrentPasswordEntry.Text;
        string newPassword = NewPasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        // Проверяем заполнение полей
        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            await DisplayAlert("Ошибка", "Все поля должны быть заполнены", "ОК");
            return;
        }

        // Проверяем совпадение паролей
        if (newPassword != confirmPassword)
        {
            await DisplayAlert("Ошибка", "Новый пароль и подтверждение не совпадают", "ОК");
            return;
        }

        // Пытаемся изменить пароль
        bool result = await _authService.ChangePasswordAsync(currentPassword, newPassword);

        if (result)
        {
            await DisplayAlert("Успех", "Пароль успешно изменен", "ОК");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный текущий пароль", "ОК");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
