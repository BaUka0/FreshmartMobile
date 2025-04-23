using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Project.Pages.Admin;

public partial class UserListPage : ContentPage
{
    public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

    private readonly DatabaseService _databaseService;
    public UserListPage(DatabaseService databaseService)
	{
		InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsersAsync();
    }
    private async Task LoadUsersAsync()
    {
        try
        {
            var userList = await _databaseService.GetUsersAsync();
            Users.Clear();
            foreach (var user in userList)
            {
                Users.Add(user);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка загрузки", $"Не удалось загрузить список пользователей: {ex.Message}", "OK");
            Debug.WriteLine($"Error loading users: {ex.Message}");
        }
    }
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        var swipeItem = (SwipeItem)sender;
        var user = (User)swipeItem.BindingContext;

        if (user != null)
        {
            bool confirmed = await DisplayAlert("Подтверждение", $"Вы уверены, что хотите удалить пользователя '{user.username}'?", "Да", "Нет");
            if (confirmed)
            {
                try
                {
                    await _databaseService.DeleteUserAsync(user);
                    Users.Remove(user);

                    await DisplayAlert("Успех", "Пользователь успешно удалён.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Не удалось удалить пользователя: {ex.Message}", "OK");
                    Debug.WriteLine($"Error deleting user: {ex.Message}");
                }
            }
        }
    }
    private async void OnEditSwipeItemInvoked(object sender, EventArgs e)
    {
        var swipeItem = (SwipeItem)sender;
        var user = (User)swipeItem.BindingContext;

        // Переход на страницу редактирования пользователя
        await Navigation.PushAsync(new EditUserPage(user, _databaseService));
    }

}