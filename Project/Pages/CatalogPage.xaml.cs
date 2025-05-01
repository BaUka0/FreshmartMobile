using Project.Services;
using System.Windows.Input;

namespace Project.Pages;

public class Category
{
    public string Name { get; set; }
    public string Icon { get; set; }
}

public partial class CatalogPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;
    public List<Category> Categories { get; set; }
    public ICommand CategoryCommand { get; set; }

    public CatalogPage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;

        Categories = new List<Category>
        {
            new Category { Name = "Напитки", Icon = "drink_icon.png" },
            new Category { Name = "Фрукты", Icon = "fruit_icon.png" },
            new Category { Name = "Овощи", Icon = "vegetables_icon.png" },
            new Category { Name = "Мясо", Icon = "meat_icon.png" },
            new Category { Name = "Гастрономия", Icon = "gastronomy.png" },
            new Category { Name = "Молочные продукты", Icon = "dairy_icon.png" },
            new Category { Name = "Бакалея", Icon = "grocery.png" },
            new Category { Name = "Хлеб и выпечка", Icon = "bread_icon.png" },
            new Category { Name = "Чай, кофе, какао", Icon = "tea.png" },
            new Category { Name = "Конфеты и сладости", Icon = "candies_icon.png" },
            new Category { Name = "Собственное производство", Icon = "cake.png" },
            new Category { Name = "Консервы", Icon = "canned.png" },
            new Category { Name = "Замороженные продукты", Icon = "frozen_icon.png" },
            new Category { Name = "Другие товары", Icon = "others.png" },
        };

        // Создаем команду для обработки нажатия на категорию
        CategoryCommand = new Command<Category>(OnCategorySelected);

        BindingContext = this;
    }

    private async void OnCategorySelected(Category selectedCategory)
    {
        if (selectedCategory == null) return;

        // Переход на страницу ProductList с выбранной категорией
        await Navigation.PushAsync(new ProductListPage(selectedCategory, _databaseService, _authService));
    }
    private async void OnCategoryTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        if (frame != null)
        {
            // Анимация уменьшения и увеличения
            await frame.ScaleTo(0.9, 100, Easing.CubicIn);
            await frame.ScaleTo(1, 100, Easing.CubicOut);
        }
    }
}