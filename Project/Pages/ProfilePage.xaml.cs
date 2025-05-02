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

        Options = new List<ProfileOption>
        {
            new ProfileOption { Name = "История покупок", Icon = "history.png" },
            new ProfileOption { Name = "Мои отзывы", Icon = "review.png" },
            new ProfileOption { Name = "Способ оплаты", Icon = "payment.png" },
            new ProfileOption { Name = "Изменить профиль", Icon = "change_profile.png" },
            new ProfileOption { Name = "Изменить пароль", Icon = "password.png" },
            new ProfileOption { Name = "Выход", Icon = "exit.png" },
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
                Title = "Выберите фото профиля"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                if (imageData.Length > 1000000) // Если больше ~1MB
                {
                    await DisplayAlert("Предупреждение", "Изображение слишком большое, выберите другое", "OK");
                    return;
                }

                await _authService.UpdateProfileImageAsync(imageData);
                ProfileImageSource = imageData;

                OnPropertyChanged(nameof(ProfileImageSource));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить изображение: {ex.Message}", "OK");
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
        await DisplayAlert("Действие", "Открыть способы оплаты", "ОК");
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
        bool confirm = await DisplayAlert("Выход", "Вы уверены, что хотите выйти из аккаунта?", "Да", "Нет");
        if (confirm)
        {
            _authService.Logout();
            if (Application.Current.MainPage is AppShell appShell)
            {
                appShell.OnUserLoggedOut();
            }
            await DisplayAlert("Выход", "Вы успешно вышли из аккаунта", "ОК");
        }
    }

    private async void OnButtonTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is ProfileOption option)
        {
            switch (option.Name)
            {
                case "История покупок":
                    OnPurchaseHistoryTapped();
                    break;
                case "Мои отзывы":
                    OnMyReviewsTapped();
                    break;
                case "Способ оплаты":
                    OnPaymentMethodTapped();
                    break;
                case "Изменить профиль":
                    OnEditProfileTapped();
                    break;
                case "Изменить пароль":
                    OnChangePasswordTapped();
                    break;
                case "Выход":
                    OnLogoutTapped();
                    break;
                default:
                    await DisplayAlert("Ошибка", "Неизвестная опция", "ОК");
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
