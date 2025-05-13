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
            new Category { Name = "Ет өнімдері", Icon = "meat_icon.png" },
            new Category { Name = "Гастрономия", Icon = "gastronomy.png" },
            new Category { Name = "Сүт өнімдері", Icon = "dairy_icon.png" },
            new Category { Name = "Бакалея", Icon = "grocery.png" },
            new Category { Name = "Нан және тоқаштар", Icon = "bread_icon.png" },
            new Category { Name = "Шай, кофе, какао", Icon = "tea.png" },
            new Category { Name = "Кәмпиттер мен тәттілер", Icon = "candies_icon.png" },
            new Category { Name = "Өндірістік өнімдер", Icon = "cake.png" },
            new Category { Name = "Консервілер", Icon = "canned.png" },
            new Category { Name = "Мұздатылған өнімдер", Icon = "frozen_icon.png" },
            new Category { Name = "Басқа тауарлар", Icon = "others.png" },
        };

        CategoryCommand = new Command<Category>(OnCategorySelected);

        BindingContext = this;
    }

    private async void OnCategorySelected(Category selectedCategory)
    {
        if (selectedCategory == null) return;

        await Navigation.PushAsync(new ProductListPage(selectedCategory, _databaseService, _authService));
    }
    private async void OnCategoryTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        if (frame != null)
        {
            await frame.ScaleTo(0.9, 100, Easing.CubicIn);
            await frame.ScaleTo(1, 100, Easing.CubicOut);
        }
    }
}