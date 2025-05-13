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
            new Category { Name = "Сусындар", Icon = "drink_icon.png" },
            new Category { Name = "Жемістер", Icon = "fruit_icon.png" },
            new Category { Name = "Көкөністер", Icon = "vegetables_icon.png" },
            new Category { Name = "Ет өнімдері\r\n", Icon = "meat_icon.png" },
            new Category { Name = "Гастрономия", Icon = "gastronomy.png" },
            new Category { Name = "Сүт өнімдері\r\n", Icon = "dairy_icon.png" },
            new Category { Name = "Бакалея", Icon = "grocery.png" },
            new Category { Name = "Нан және тоқаштар\r\n", Icon = "bread_icon.png" },
            new Category { Name = "Шай, кофе, какао", Icon = "tea.png" },
            new Category { Name = "Кәмпиттер мен тәттілер\r\n", Icon = "candies_icon.png" },
            new Category { Name = "\tӨндірістік өнімдер\r\n", Icon = "cake.png" },
            new Category { Name = "Консервілер", Icon = "canned.png" },
            new Category { Name = "\tМұздатылған өнімдер\r\n", Icon = "frozen_icon.png" },
            new Category { Name = "\tБасқа тауарлар", Icon = "others.png" },
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