using Project.Models;
using Project.Services;

namespace Project.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;

    public List<ProfileOption> Options { get; set; }

    public ProfilePage(AuthService authService)
    {
        InitializeComponent();

        _authService = authService;

        Options = new List<ProfileOption>
        {
            new ProfileOption { Name = "������� �������", Icon = "history.png" },
            new ProfileOption { Name = "��� ������", Icon = "review.png" },
            new ProfileOption { Name = "������ ������", Icon = "payment.png" },
            new ProfileOption { Name = "�������� �������", Icon = "change_profile.png" },
            new ProfileOption { Name = "�������� ������", Icon = "password.png" },
            new ProfileOption { Name = "�����", Icon = "exit.png" },
        };

        BindingContext = this;
    }

    private async void OnPurchaseHistoryTapped()
    {
        await DisplayAlert("��������", "������� ������� �������", "��");
    }

    private async void OnMyReviewsTapped()
    {
        await DisplayAlert("��������", "������� ��� ������", "��");
    }

    private async void OnPaymentMethodTapped()
    {
        await DisplayAlert("��������", "������� ������� ������", "��");
    }

    private async void OnEditProfileTapped()
    {
        await DisplayAlert("��������", "�������� �������", "��");
    }

    private async void OnChangePasswordTapped()
    {
        await DisplayAlert("��������", "�������� ������", "��");
    }

    private async void OnLogoutTapped()
    {
        bool confirm = await DisplayAlert("�����", "�� �������, ��� ������ ����� �� ��������?", "��", "���");
        if (confirm)
        {
            _authService.Logout();
            if (Application.Current.MainPage is AppShell appShell)
            {
                appShell.OnUserLoggedOut();
            }
            await DisplayAlert("�����", "�� ������� ����� �� ��������", "��");
        }
    }

    private async void OnButtonTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is ProfileOption option)
        {
            switch (option.Name)
            {
                case "������� �������":
                    OnPurchaseHistoryTapped();
                    break;
                case "��� ������":
                    OnMyReviewsTapped();
                    break;
                case "������ ������":
                    OnPaymentMethodTapped();
                    break;
                case "�������� �������":
                    OnEditProfileTapped();
                    break;
                case "�������� ������":
                    OnChangePasswordTapped();
                    break;
                case "�����":
                    OnLogoutTapped();
                    break;
                default:
                    await DisplayAlert("������", "����������� �����", "��");
                    break;
            }
        }
    }
}
