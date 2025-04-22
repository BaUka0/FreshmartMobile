
using Project.Models;
namespace Project.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
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

            var user = await App.DatabaseService.GetUserByCredentialsAsync(usernameEntry.Text, passwordEntry.Text);

            if (user != null)
            {
                App.currentUser = user;

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