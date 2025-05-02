using Project.Services;

namespace Project.Pages;

public partial class EditProfilePage : ContentPage
{
    private readonly AuthService _authService;
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] ProfileImageSource { get; set; }

    public EditProfilePage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        var currentUser = _authService.CurrentUser;
        if (currentUser != null)
        {
            Username = currentUser.username;
            Email = currentUser.email;
            ProfileImageSource = currentUser.ProfileImage;
        }

        BindingContext = this;
    }

    private async void OnChangeProfileImageClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "�������� ���� �������"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                if (imageData.Length > 1000000)
                {
                    await DisplayAlert("��������������", "����������� ������� �������, �������� ������", "OK");
                    return;
                }

                await _authService.UpdateProfileImageAsync(imageData);
                ProfileImageSource = imageData;

                OnPropertyChanged(nameof(ProfileImageSource));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", $"�� ������� ��������� �����������: {ex.Message}", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
        {
            await DisplayAlert("������", "��� ������������ � Email ����������� ��� ����������", "OK");
            return;
        }

        if (!Email.Contains('@') || !Email.Contains('.'))
        {
            await DisplayAlert("������", "������� ���������� email", "OK");
            return;
        }

        bool result = await _authService.UpdateProfileAsync(Username, Email);

        if (result)
        {
            await DisplayAlert("�����", "������� ������� ��������", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("������", "�� ������� �������� �������", "OK");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
