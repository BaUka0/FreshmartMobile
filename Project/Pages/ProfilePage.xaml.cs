using Project.Models;
using Project.Services;
using Project.Pages.Client;

namespace Project.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;
    private readonly DatabaseService _databaseService;
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] ProfileImageSource { get; set; }
    public List<ProfileOption> Options { get; set; }

    public ProfilePage(AuthService authService, DatabaseService databaseService)
    {
        InitializeComponent();

        _authService = authService;
        _databaseService = databaseService;

        LoadUserData();

        string userRole = _authService.GetCurrentUserRole();
        Options = new List<ProfileOption>();
        if (userRole == "client")
        {
            new ProfileOption { Name = "Сатып алу тарихы", Icon = "history.png" },
            new ProfileOption { Name = "Менің пікірлерім", Icon = "review.png" },
            new ProfileOption { Name = "Төлем әдісі", Icon = "payment.png" },
            new ProfileOption { Name = "Профильді өңдеу", Icon = "change_profile.png" },
            new ProfileOption { Name = "Құпия сөзді өзгерту", Icon = "password.png" },
            new ProfileOption { Name = "Шығу", Icon = "exit.png" },
        };

        BindingContext = this;
    }

    private void LoadUserData()
    {
        var currentUser = _authService.CurrentUser;
        if (currentUser != null)
        {
            Username = currentUser.username;
            Email = currentUser.email;
            ProfileImageSource = currentUser.ProfileImage;
        }
    }

    private async void OnChangeProfileImageClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Профиль фотосуретін таңдаңыз"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                if (imageData.Length > 1000000) // Если больше ~1MB
                {
                    await DisplayAlert("Ескерту", "Сурет тым үлкен, басқасын таңдаңыз", "OK");
                    return;
                }

                await _authService.UpdateProfileImageAsync(imageData);
                ProfileImageSource = imageData;

                OnPropertyChanged(nameof(ProfileImageSource));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Қате", $"Сурет жүктелмеді: {ex.Message}", "OK");
        }
    }

    private async void OnPurchaseHistoryTapped()
    {
        await Navigation.PushAsync(new OrderHistoryPage(_databaseService, _authService));
    }

    private async void OnMyReviewsTapped()
    {
        await Navigation.PushAsync(new UserReviewsPage(_databaseService, _authService));
    }


    private async void OnPaymentMethodTapped()
    {
        await Navigation.PushAsync(new PaymentMethodsPage(_databaseService, _authService));
    }

    private async void OnEditProfileTapped()
    {
        await Navigation.PushAsync(new EditProfilePage(_authService));
    }

    private async void OnChangePasswordTapped()
    {
        await Navigation.PushAsync(new ChangePasswordPage(_authService));
    }

    private async void OnLogoutTapped()
    {
        bool confirm = await DisplayAlert("Шығу", "Жүйеден шыққыңыз келетініне сенімдісіз бе?", "Иә", "Жоқ");
        if (confirm)
        {
            _authService.Logout();
            if (Application.Current.MainPage is AppShell appShell)
            {
                appShell.OnUserLoggedOut();
            }
            await DisplayAlert("Шығу", "Жүйеден сәтті шықтыңыз.", "ОК");
        }
    }

    private async void OnButtonTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is ProfileOption option)
        {
            switch (option.Name)
            {
                case "Сатып алу тарихы":
                    OnPurchaseHistoryTapped();
                    break;
                case "Менің пікірлерім":
                    OnMyReviewsTapped();
                    break;
                case "Төлем әдісі":
                    OnPaymentMethodTapped();
                    break;
                case "Профильді өңдеу":
                    OnEditProfileTapped();
                    break;
                case "Құпия сөзді өзгерту":
                    OnChangePasswordTapped();
                    break;
                case "Шығу":
                    OnLogoutTapped();
                    break;
                default:
                    await DisplayAlert("Қате", "Белгісіз опция", "ОК");
                    break;
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadUserData();
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(Email));
        OnPropertyChanged(nameof(ProfileImageSource));
    }
}
