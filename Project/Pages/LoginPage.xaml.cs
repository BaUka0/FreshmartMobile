
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

	private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        await button.ScaleTo(0.9, 100);
        await button.ScaleTo(1.0, 100);

        try
        {
            if (string.IsNullOrWhiteSpace(usernameEntry.Text) || string.IsNullOrWhiteSpace(passwordEntry.Text))
            {
                await DisplayAlert("������", "��� ������������ � ������ �� ����� ���� �������.", "OK");
                return;
            }

            var success = await _authService.LoginAsync(usernameEntry.Text, passwordEntry.Text);

            if (success)
            {
                if (Application.Current.MainPage is AppShell appShell)
                {
                    appShell.UpdateUsername();
                }
                await DisplayAlert("�����", "���� �������� �������!", "OK");
                await Shell.Current.GoToAsync("//books");
            }
            else
            {
                await DisplayAlert("������", "�������� ��� ������������ ��� ������.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������ ��� �����: {ex.Message}");
            await DisplayAlert("������", "��������� ������ ��� �����. ���������� ��� ���.", "OK");
        }
    }
    private async void OnRegisterButtonClicked(object sender, EventArgs e)
	{
        var button = (Button)sender;

        await button.ScaleTo(0.9, 100);
        await button.ScaleTo(1.0, 100);

        await Shell.Current.GoToAsync("//register");
    }
}