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
        if (!role.Equals("client", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var product in Products)
            {
                product.IsFavoriteButtonVisible = false;
                product.IsCartButtonVisible = false;
            }
        }
        else
        {
            foreach (var product in Products)
            {
                product.IsFavoriteButtonVisible = true;
                product.IsCartButtonVisible = true;
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

    private void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            if (button.Source is FileImageSource imageSource)
            {
                // Проверяем текущую картинку
                if (imageSource.File == "favourite_grey.png")
                {
                    button.Source = "favourite_green.png";
                }
                else
                {
                    button.Source = "favourite_grey.png";
                }
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