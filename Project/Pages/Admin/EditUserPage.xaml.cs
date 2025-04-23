using Microsoft.Maui.Storage;
using Project.Models;
using Project.Services;

namespace Project.Pages.Admin;

public partial class EditUserPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly User _user;

    public EditUserPage(User user, DatabaseService databaseService)
    {
        InitializeComponent();
        _user = user;
        _databaseService = databaseService;

        UsernameEntry.Text = _user.username;
        PasswordEntry.Text = _user.password;
        EmailEntry.Text = _user.email;
        RolePicker.SelectedItem = string.IsNullOrEmpty(_user.role) ? "user" : _user.role;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        _user.username = UsernameEntry.Text;
        _user.password = PasswordEntry.Text;
        _user.email = EmailEntry.Text;
        _user.role = RolePicker.SelectedItem?.ToString();


        try
        {
            await _databaseService.UpdateUserAsync(_user);
            await DisplayAlert("Успех", "Данные пользователя успешно обновлены.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось сохранить изменения: {ex.Message}", "OK");
        }
    }
}
