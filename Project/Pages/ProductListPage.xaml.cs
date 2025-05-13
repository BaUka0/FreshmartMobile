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

        // Получаем все товары в корзине текущего пользователя
        var cartItems = await _databaseService.GetCartItemsAsync(userId);
        var cartProductIds = cartItems.Select(ci => ci.ProductId).ToHashSet();

        foreach (var product in Products)
        {
            if (role.Equals("client", StringComparison.OrdinalIgnoreCase))
            {
                product.IsFavoriteButtonVisible = true;
                product.IsCartButtonVisible = true;

                // Проверяем, является ли продукт избранным
                var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, product.Id);
                product.FavoriteIcon = isFavorite ? "favourite_green.png" : "favourite_grey.png";

                // Проверяем, находится ли продукт в корзине
                product.CartIcon = cartProductIds.Contains(product.Id) ? "basket_green.png" : "basket_grey.png";
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
            await Navigation.PushAsync(new ProductDetail(tappedProduct, _databaseService, _authService));
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
                await DisplayAlert("Таңдаулылар", "Тауар таңдаулылардан жойылды", "Ок");
            }
            else
            {
                await _databaseService.AddFavoriteProductAsync(userId, product.Id);
                button.Source = "favourite_green.png";
                await DisplayAlert("Таңдаулылар", "Тауар таңдаулыларыңызға қосылды", "Ок");
            }
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
                await DisplayAlert("Себет", "Тауар қазірдің өзінде себетке салынған!", "Ок");
            }
            else
            {
                await _databaseService.AddToCartAsync(userId, product.Id, 1);
                product.CartIcon = "basket_green.png";
                await DisplayAlert("Қосылды", "Тауар себетке қосылды!", "Ок");
            }

            // Обновляем привязку
            ProductsCollectionView.ItemsSource = null;
            ProductsCollectionView.ItemsSource = Products;
        }
    }

}