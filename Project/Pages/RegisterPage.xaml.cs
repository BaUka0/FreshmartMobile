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
                await DisplayAlert("������", "��� ������������ � ������ �� ����� ���� �������.", "OK");
                return;
            }

            await App.DatabaseService.AddUserAsync(user);

            await DisplayAlert("�����", "������������ ������� ���������������!", "OK");
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������ ��� ����������� ������������: {ex.Message}");

            await DisplayAlert("������", "��������� ������ ��� �����������. ���������� ��� ���.", "OK");
        }
    }
}