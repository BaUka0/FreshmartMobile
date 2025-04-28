using Project.Models;

namespace Project.Pages;

public partial class ProductListPage : ContentPage
{
    public Category SelectedCategory { get; set; }
    public List<Product> Products { get; set; }
    public ProductListPage(Category category)
	{
		InitializeComponent();
        // Сохраняем выбранную категорию
        SelectedCategory = category;
        CategoryNameLabel.Text = SelectedCategory.Name; // Устанавливаем название категории на экран

        // Загрузка продуктов для выбранной категории (пример)
        LoadProducts();
    }
    private void LoadProducts()
    {
        // Пример загрузки продуктов в зависимости от категории
        Products = new List<Product>
        {
            new Product { Name = "Продукт 1", Description = "Описание 1", Price = "$10.00", Image = "product1.png" },
            new Product { Name = "Продукт 2", Description = "Описание 2", Price = "$20.00", Image = "product2.png" },
            // Добавьте больше продуктов
        };

        // Привязка данных к CollectionView
        ProductsCollectionView.ItemsSource = Products;
    }
}