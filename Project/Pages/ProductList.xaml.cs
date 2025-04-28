using Project.Models;
using Project.Pages;
namespace Project;

public partial class ProductList : ContentPage
{
    public Category SelectedCategory { get; set; }
    public List<Product> Products { get; set; }

    public ProductList(Category category)
    {
        InitializeComponent();

        SelectedCategory = category;
        CategoryNameLabel.Text = SelectedCategory.Name;

        LoadProducts();
    }

    private void LoadProducts()
    {
        Products = new List<Product>
        {
            new Product { Name = "������� 1", Description = "�������� 1", Price = "$10.00", Image = "default_product.png" },
            new Product { Name = "������� 2", Description = "�������� 2", Price = "$20.00", Image = "default_product.png" },
            // �������� ��� ������
        };

        ProductsCollectionView.ItemsSource = Products;
    }

    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Product tappedProduct)
        {
            await Navigation.PushAsync(new ProductDetail(tappedProduct));
        }
    }

    private void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            if (button.Source is FileImageSource imageSource)
            {
                // ��������� ������� ��������
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
            // �����, ��������, �������� ������ ������� �� ������� ������� �� �������
            button.Source = "basket_green.png";

            await DisplayAlert("���������", "����� �������� � �������!", "��");

            // ����� ����������� ������� ������� ����� �������
            button.Source = "basket_grey.png";
        }
    }

}
