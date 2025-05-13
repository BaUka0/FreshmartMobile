using Project.Models;
using Project.Services;
using Project.Pages;
using System.Collections.ObjectModel;

namespace Project.Pages.Client;

public partial class CartPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;
    public ObservableCollection<CartDisplayItem> CartItems { get; set; } = new();

    public CartPage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;

        CartCollectionView.ItemsSource = CartItems;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCartItems();
    }

    private async Task LoadCartItems()
    {
        CartItems.Clear();

        var userId = _authService.GetCurrentUserId();
        var cartItems = await _databaseService.GetCartItemsAsync(userId);

        foreach (var ci in cartItems)
        {
            var product = await _databaseService.GetProductAsync(ci.ProductId);
            if (product == null)
                continue;

            CartItems.Add(new CartDisplayItem
            {
                CartItemId = ci.Id,
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = ci.Quantity,
                ImageData = product.ImageData
            });
        }

        UpdateSummary();
    }

    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is CartDisplayItem item)
        {
            var product = await _databaseService.GetProductAsync(item.ProductId);
            if (product != null)
            {
                await Navigation.PushAsync(new ProductDetail(product, _databaseService, _authService));
            }
        }
    }

    private void OnIncreaseQuantityClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CartDisplayItem item)
        {
            item.Quantity++;
            _databaseService.UpdateCartItemQuantityAsync(item.CartItemId, item.Quantity);
            UpdateSummary();
            CartCollectionView.ItemsSource = null;
            CartCollectionView.ItemsSource = CartItems;
        }
    }

    private void OnDecreaseQuantityClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CartDisplayItem item)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
                _databaseService.UpdateCartItemQuantityAsync(item.CartItemId, item.Quantity);
                UpdateSummary();
                CartCollectionView.ItemsSource = null;
                CartCollectionView.ItemsSource = CartItems;
            }
        }
    }

    private async void OnDeleteItemClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CartDisplayItem item)
        {
            await _databaseService.RemoveCartItemAsync(item.CartItemId);

            CartItems.Remove(item);
            UpdateSummary();
        }
    }

    private async void OnOrderClicked(object sender, EventArgs e)
    {
        if (CartItems.Count == 0)
        {
            await DisplayAlert("Себет бос", "Тапсырыс жасамас бұрын тауарларды қосыңыз.", "OK");
            return;
        }
        var cartItemsCopy = new ObservableCollection<Product>(CartItems.Select(item => new Product
        {
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity,
        }));

        await Navigation.PushAsync(new OrderSummaryPage(cartItemsCopy, _databaseService, _authService));

        CartItems.Clear();

        var userId = _authService.GetCurrentUserId();
        await _databaseService.ClearCartAsync(userId);

        UpdateSummary();
    }

    private void UpdateSummary()
    {
        int totalItems = CartItems.Sum(item => item.Quantity);
        int totalPrice = 0;

        foreach (var item in CartItems)
        {
            if (int.TryParse(item.Price.Replace("₸", "").Trim(), out int price))
            {
                totalPrice += price * item.Quantity;
            }
        }

        SummaryLabel.Text = $"Тауарлар: {totalItems} | Барлығы: {totalPrice} ₸";

        bool isCartEmpty = CartItems.Count == 0;
        CartCollectionView.IsVisible = !isCartEmpty;
        EmptyCartLabel.IsVisible = isCartEmpty;
    }
}
