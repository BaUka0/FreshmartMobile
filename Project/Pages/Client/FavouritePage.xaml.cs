using Project.Models;
using Project.Services;
using Project.Pages;
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

        // Получаем все товары в корзине текущего пользователя
        var cartItems = await _databaseService.GetCartItemsAsync(userId);
        var cartProductIds = cartItems.Select(ci => ci.ProductId).ToHashSet();

        var favoriteProducts = await _databaseService.GetFavoriteProductsAsync(userId);

        FavoriteProducts.Clear();
        foreach (var product in favoriteProducts)
        {
            // Проверяем, находится ли продукт в корзине
            product.CartIcon = cartProductIds.Contains(product.Id) ? "basket_green.png" : "basket_grey.png";

            FavoriteProducts.Add(product);
        }
    }

    // Новый обработчик нажатия на продукт
    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is Product product)
        {
            await Navigation.PushAsync(new ProductDetail(product, _databaseService, _authService));
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
        if (sender is ImageButton button && button.BindingContext is Product product)
        {
            var userId = _authService.GetCurrentUserId();

            // Проверяем, находится ли продукт в корзине
            var cartItems = await _databaseService.GetCartItemsAsync(userId);
            var isInCart = cartItems.Any(ci => ci.ProductId == product.Id);

            if (isInCart)
            {
                await DisplayAlert("Корзина", "Товар уже находится в корзине!", "Ок");
            }
            else
            {
                await _databaseService.AddToCartAsync(userId, product.Id, 1);
                product.CartIcon = "basket_green.png";
                await DisplayAlert("Добавлено", "Товар добавлен в корзину!", "Ок");
            }
        }
    }
}
