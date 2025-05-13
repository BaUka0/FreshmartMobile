using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;

namespace Project.Pages.Seller;

public partial class DashboardPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;
    public ObservableCollection<Product> Products { get; set; } = new();

    public DashboardPage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;

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
            await Navigation.PushAsync(new ProductDetail(product, _databaseService, _authService));
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
            bool confirm = await DisplayAlert("Жою", $"{product.Name} жою керек па?", "Иә", "Жоқ");
            if (confirm)
            {
                await _databaseService.DeleteProductAsync(product);
                Products.Remove(product);
            }
        }
    }
}