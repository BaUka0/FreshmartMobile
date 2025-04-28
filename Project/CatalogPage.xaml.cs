using Project.Pages;
using System.Windows.Input;

namespace Project;

public class Category
{
    public string Name { get; set; }
    public string Icon { get; set; }
}

public partial class CatalogPage : ContentPage
{
    public List<Category> Categories { get; set; }
    public ICommand CategoryCommand { get; set; }

    public CatalogPage()
    {
        InitializeComponent();

        Categories = new List<Category>
        {
            new Category { Name = "Напитки", Icon = "drink_icon.png" },
            new Category { Name = "Фрукты", Icon = "fruit_icon.png" },
            new Category { Name = "Овощи", Icon = "vegetables_icon.png" },
            new Category { Name = "Мясо", Icon = "meat_icon.png" },
            new Category { Name = "Молочные продукты", Icon = "dairy_icon.png" },
            new Category { Name = "Хлеб и выпечка", Icon = "bread_icon.png" },
            new Category { Name = "Конфеты и сладости", Icon = "candies_icon.png" },
            new Category { Name = "Замороженные продукты", Icon = "frozen_icon.png" }
        };

        // Создаем команду для обработки нажатия на категорию
        CategoryCommand = new Command<Category>(OnCategorySelected);

        BindingContext = this;
    }

    private async void OnCategorySelected(Category selectedCategory)
    {
        if (selectedCategory == null) return;

        // Переход на страницу ProductList с выбранной категорией
        await Navigation.PushAsync(new ProductList(selectedCategory));
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

