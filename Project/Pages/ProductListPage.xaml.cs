using Project.Models;

namespace Project.Pages;

public partial class ProductListPage : ContentPage
{
    public Category SelectedCategory { get; set; }
    public List<Product> Products { get; set; }
    public ProductListPage(Category category)
	{
		InitializeComponent();
        // ��������� ��������� ���������
        SelectedCategory = category;
        CategoryNameLabel.Text = SelectedCategory.Name; // ������������� �������� ��������� �� �����

        // �������� ��������� ��� ��������� ��������� (������)
        LoadProducts();
    }
    private void LoadProducts()
    {
        // ������ �������� ��������� � ����������� �� ���������
        Products = new List<Product>
        {
            new Product { Name = "������� 1", Description = "�������� 1", Price = "$10.00", Image = "product1.png" },
            new Product { Name = "������� 2", Description = "�������� 2", Price = "$20.00", Image = "product2.png" },
            // �������� ������ ���������
        };

        // �������� ������ � CollectionView
        ProductsCollectionView.ItemsSource = Products;
    }
}