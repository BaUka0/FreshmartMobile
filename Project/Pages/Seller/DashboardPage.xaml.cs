using Project.Models;
using System.Collections.ObjectModel;

namespace Project.Pages.Seller;

public partial class DashboardPage : ContentPage
{
    public ObservableCollection<Product> Products { get; set; } = new();

    public DashboardPage()
    {
        InitializeComponent();

        // Пример товаров
        Products.Add(new Product
        {
            Name = "Молоко",
            Price = "350 ₸",
            Category = "Молочные продукты",
            Image = "default_product.png"
        });

        Products.Add(new Product
        {
            Name = "Хлеб",
            Price = "150 ₸",
            Category = "Хлеб и выпечка",
            Image = "default_product.png"
        });


        ProductCollectionView.ItemsSource = Products;
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage());
    }

    private async void OnViewProductClicked(object sender, EventArgs e)
    {
        var product = (sender as Button)?.BindingContext as Product;
        if (product != null)
        {
            await Navigation.PushAsync(new ProductDetail(product));
        }
    }

    private async void OnEditProductClicked(object sender, EventArgs e)
    {
        var product = (sender as Button)?.BindingContext as Product;
        if (product != null)
        {
            await Navigation.PushAsync(new EditProductPage(product));
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
                Products.Remove(product);
            }
        }
    }
}
