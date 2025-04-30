using Project.Models;

namespace Project.Pages;

public partial class ProfilePage : ContentPage
{
    public List<ProfileOption> Options { get; set; }

    public ProfilePage()
    {
        InitializeComponent();

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
    private async void OnButtonTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is ProfileOption option)
        {
            string selectedOption = option.Name;
            await DisplayAlert("�� �������", selectedOption, "��");
        }
    }
}