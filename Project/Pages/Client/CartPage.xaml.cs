using Project.Models;
using System.Collections.ObjectModel;

namespace Project.Pages.Client;

public partial class CartPage : ContentPage
{
    public ObservableCollection<Product> CartItems { get; set; } = new();

    public CartPage()
    {
        InitializeComponent();

        // Пример: наполняем тестовыми товарами
        CartItems.Add(new Product { Name = "Яблоко", Price = "500 ₸", Quantity = 1 });
        CartItems.Add(new Product { Name = "Банан", Price = "600 ₸", Quantity = 2 });
        CartItems.Add(new Product { Name = "Груша", Price = "700 ₸", Quantity = 1 });

        CartCollectionView.ItemsSource = CartItems;

        UpdateSummary();
    }

    private void OnIncreaseQuantityClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var product = (button.BindingContext as Product);
        if (product != null)
        {
            product.Quantity++;
            CartCollectionView.ItemsSource = null;
            CartCollectionView.ItemsSource = CartItems;
            UpdateSummary();
        }
    }

    private void OnDecreaseQuantityClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var product = (button.BindingContext as Product);
        if (product != null && product.Quantity > 1)
        {
            product.Quantity--;
            CartCollectionView.ItemsSource = null;
            CartCollectionView.ItemsSource = CartItems;
            UpdateSummary();
        }
    }

    private void OnDeleteItemClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var product = (button.BindingContext as Product);
        if (product != null)
        {
            CartItems.Remove(product);
            UpdateSummary();
        }
    }

    private async void OnOrderClicked(object sender, EventArgs e)
    {
        if (CartItems.Count == 0)
        {
            await DisplayAlert("Корзина пуста", "Добавьте товары перед оформлением заказа.", "OK");
            return;
        }

        await Navigation.PushAsync(new OrderSummaryPage(CartItems));


        CartItems.Clear();
        UpdateSummary();
    }

    // Метод для обновления общей информации
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

        SummaryLabel.Text = $"Товаров: {totalItems} | Итого: {totalPrice} ₸";

        // Если корзина пуста — показываем сообщение, скрываем список
        bool isCartEmpty = CartItems.Count == 0;
        CartCollectionView.IsVisible = !isCartEmpty;
        EmptyCartLabel.IsVisible = isCartEmpty;
    }
}