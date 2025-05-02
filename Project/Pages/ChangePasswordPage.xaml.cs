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

        // ��������� ���������� �����
        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            await DisplayAlert("������", "��� ���� ������ ���� ���������", "��");
            return;
        }

        // ��������� ���������� �������
        if (newPassword != confirmPassword)
        {
            await DisplayAlert("������", "����� ������ � ������������� �� ���������", "��");
            return;
        }

        // �������� �������� ������
        bool result = await _authService.ChangePasswordAsync(currentPassword, newPassword);

        if (result)
        {
            await DisplayAlert("�����", "������ ������� �������", "��");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("������", "�������� ������� ������", "��");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
