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

        var cartItems = await _databaseService.GetCartItemsAsync(userId);
        var cartProductIds = cartItems.Select(ci => ci.ProductId).ToHashSet();

        var favoriteProducts = await _databaseService.GetFavoriteProductsAsync(userId);

        FavoriteProducts.Clear();
        foreach (var product in favoriteProducts)
        {
            product.CartIcon = cartProductIds.Contains(product.Id) ? "basket_green.png" : "basket_grey.png";

            FavoriteProducts.Add(product);
        }
    }

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
            await DisplayAlert("Таңдаулылар", "Тауар таңдаулылардан жойылды", "Ок");

            FavoriteProducts.Remove(product);
        }
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Product product)
        {
            var userId = _authService.GetCurrentUserId();

            var cartItems = await _databaseService.GetCartItemsAsync(userId);
            var isInCart = cartItems.Any(ci => ci.ProductId == product.Id);

            if (isInCart)
            {
                await DisplayAlert("Себет", "Тауар қазірдің өзінде себетке салынған!", "Ок");
            }
            else
            {
                await _databaseService.AddToCartAsync(userId, product.Id, 1);
                product.CartIcon = "basket_green.png";
                await DisplayAlert("Қосылды", "Тауар себетке қосылды!", "Ок");
            }
        }
    }
}
