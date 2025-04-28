using Project.Models;
namespace Project.Pages.Client;

public partial class FavouritePage : ContentPage
{
    public FavouritePage()
    {
        InitializeComponent();
        LoadTestProducts();
    }

    private void LoadTestProducts()
    {
        var products = new List<Product>
        {
            new Product { Name = "������", Image = "defaultProduct.png", Price = "100" },
            new Product { Name = "�����", Image = "defaultProduct.png", Price = "100" },
            new Product { Name = "��������", Image = "defaultProduct.png" , Price = "100" }
        };

        FavouritesCollectionView.ItemsSource = products;
    }

    private void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            button.Source = button.Source.ToString().Contains("green")
                ? "favourite_grey.png"
                : "favourite_green.png";
        }
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            button.Source = "basket_green.png";
            await DisplayAlert("���������", "����� �������� � �������!", "��");
            button.Source = "basket_grey.png";
        }
    }
}
