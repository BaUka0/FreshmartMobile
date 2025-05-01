using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Project.Pages.Client;

public partial class FavouritePage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;

    // Используем ObservableCollection для автоматического обновления интерфейса
    public ObservableCollection<Product> FavoriteProducts { get; set; }

    public FavouritePage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;

        FavoriteProducts = new ObservableCollection<Product>();
        FavouritesCollectionView.ItemsSource = FavoriteProducts;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadFavouriteProducts();
    }

    private async void LoadFavouriteProducts()
    {
        var userId = _authService.CurrentUser.Id;
        var favoriteProducts = await _databaseService.GetFavoriteProductsAsync(userId);

        FavoriteProducts.Clear();
        foreach (var product in favoriteProducts)
        {
            FavoriteProducts.Add(product);
        }
    }

    private async void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Product product)
        {
            var userId = _authService.GetCurrentUserId();

            await _databaseService.RemoveFavoriteProductAsync(userId, product.Id);
            await DisplayAlert("Избранное", "Товар удален из избранного", "Ок");

            FavoriteProducts.Remove(product);
        }
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            button.Source = "basket_green.png";
            await DisplayAlert("Добавлено", "Товар добавлен в корзину!", "Ок");
            button.Source = "basket_grey.png";
        }
    }
}
