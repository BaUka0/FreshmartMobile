using Project.Models;
using Project.Services;
using System.Threading.Tasks;

namespace Project.Pages;

public partial class ProductListPage : ContentPage
{
    private DatabaseService _databaseService;
    private AuthService _authService;
    public Category SelectedCategory { get; set; }
    public List<Product> Products { get; set; }

    public ProductListPage(Category category, DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;

        SelectedCategory = category;
        CategoryNameLabel.Text = SelectedCategory.Name;

        LoadProducts();
    }

    private async Task LoadProducts()
    {
        Products = await _databaseService.GetProductsByCategoryAsync(SelectedCategory.Name);

        var role = _authService.GetCurrentUserRole();
        var userId = _authService.GetCurrentUserId();

        foreach (var product in Products)
        {
            if (role.Equals("client", StringComparison.OrdinalIgnoreCase))
            {
                product.IsFavoriteButtonVisible = true;
                product.IsCartButtonVisible = true;

                var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, product.Id);
                product.FavoriteIcon = isFavorite ? "favourite_green.png" : "favourite_grey.png";
            }
            else
            {
                product.IsFavoriteButtonVisible = false;
                product.IsCartButtonVisible = false;
            }
        }

        ProductsCollectionView.ItemsSource = Products;
    }


    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Product tappedProduct)
        {
            await Navigation.PushAsync(new ProductDetail(tappedProduct, _databaseService));
        }
    }

    private async void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Product product)
        {
            var userId = _authService.GetCurrentUserId();
            var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, product.Id);

            if (isFavorite)
            {
                await _databaseService.RemoveFavoriteProductAsync(userId, product.Id);
                button.Source = "favourite_grey.png";
                await DisplayAlert("Избранное", "Товар удален из избранного", "Ок");
            }
            else
            {
                await _databaseService.AddFavoriteProductAsync(userId, product.Id);
                button.Source = "favourite_green.png";
                await DisplayAlert("Избранное", "Товар добавлен в избранное", "Ок");
            }
        }
    }



    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            // Можно, например, поменять иконку корзины на зеленую корзину на секунду
            button.Source = "basket_green.png";

            await DisplayAlert("Добавлено", "Товар добавлен в корзину!", "Ок");

            // После уведомления вернуть обратно серую корзину
            button.Source = "basket_grey.png";
        }
    }
}