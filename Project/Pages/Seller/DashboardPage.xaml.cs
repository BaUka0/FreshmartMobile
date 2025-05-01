using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;

namespace Project.Pages.Seller;

public partial class DashboardPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    public ObservableCollection<Product> Products { get; set; } = new();

    public DashboardPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;

        ProductCollectionView.ItemsSource = Products;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProductsAsync();
    }
    private async Task LoadProductsAsync()
    {
        Products.Clear();
        var productsFromDb = await _databaseService.GetProductsAsync();
        foreach (var product in productsFromDb)
            Products.Add(product);
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage(_databaseService));
    }

    private async void OnViewProductClicked(object sender, EventArgs e)
    {
        var product = (sender as Button)?.BindingContext as Product;
        if (product != null)
        {
            await Navigation.PushAsync(new ProductDetail(product, _databaseService));
        }
    }

    private async void OnEditProductClicked(object sender, EventArgs e)
    {
        var product = (sender as Button)?.BindingContext as Product;
        if (product != null)
        {
            await Navigation.PushAsync(new EditProductPage(product, _databaseService));
        }
    }

    private async void OnDeleteProductClicked(object sender, EventArgs e)
    {
        var product = (sender as Button)?.BindingContext as Product;
        if (product != null)
        {
            bool confirm = await DisplayAlert("Удалить", $"Удалить {product.Name}?", "Да", "Нет");
            if (confirm)
            {
                // Удаляем из БД
                await _databaseService.DeleteProductAsync(product);
                Products.Remove(product);
            }
        }
    }
}