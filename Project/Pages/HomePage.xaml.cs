using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Project.Pages;

public partial class HomePage : ContentPage, INotifyPropertyChanged
{
    private DatabaseService _databaseService;
    private AuthService _authService;

    private ObservableCollection<string> _carouselItems;
    public ObservableCollection<string> CarouselItems
    {
        get => _carouselItems;
        set => SetProperty(ref _carouselItems, value);
    }

    private ObservableCollection<Product> _popularProducts;
    public ObservableCollection<Product> PopularProducts
    {
        get => _popularProducts;
        set => SetProperty(ref _popularProducts, value);
    }

    public HomePage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;

        CarouselItems = new ObservableCollection<string>
        {
            "carusel_one.png",
            "carusel_two.png",
            "carusel_three.png"
        };

        PopularProducts = new ObservableCollection<Product>();

        BindingContext = this;

        LoadMockProducts();
    }

    private void LoadMockProducts()
    {
        PopularProducts = new ObservableCollection<Product>
        {
            new Product { Id = 1, Name = "Яблоко", Price = "150", Image = "default_product.png", FavoriteIcon = "favourite_grey.png", CartIcon = "basket_grey.png", IsFavoriteButtonVisible = true, IsCartButtonVisible = true },
            new Product { Id = 2, Name = "Банан", Price = "200", Image = "default_product.png", FavoriteIcon = "favourite_grey.png", CartIcon = "basket_grey.png", IsFavoriteButtonVisible = true, IsCartButtonVisible = true },
            new Product { Id = 3, Name = "Молоко", Price = "300", Image = "default_product.png", FavoriteIcon = "favourite_grey.png", CartIcon = "basket_grey.png", IsFavoriteButtonVisible = true, IsCartButtonVisible = true }
        };
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
                product.FavoriteIcon = "favourite_grey.png";
                await DisplayAlert("Избранное", "Товар удален из избранного", "Ок");
            }
            else
            {
                await _databaseService.AddFavoriteProductAsync(userId, product.Id);
                product.FavoriteIcon = "favourite_green.png";
                await DisplayAlert("Избранное", "Товар добавлен в избранное", "Ок");
            }

            RefreshProducts();
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
                await DisplayAlert("Корзина", "Товар уже в корзине", "Ок");
            }
            else
            {
                await _databaseService.AddToCartAsync(userId, product.Id, 1);
                product.CartIcon = "basket_green.png";
                await DisplayAlert("Корзина", "Товар добавлен!", "Ок");
            }

            RefreshProducts();
        }
    }

    private void RefreshProducts()
    {
        // Принудительно обновляем привязку UI
        PopularProducts = new ObservableCollection<Product>(PopularProducts);
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion

    private async void OnSearchBarFocused(object sender, FocusEventArgs e)
    {
        await Navigation.PushAsync(new SearchPage(string.Empty));
    }


}
