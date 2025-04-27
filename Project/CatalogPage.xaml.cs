namespace Project;
public class Category
{
    public string Name { get; set; }
    public string Icon { get; set; }
}

public partial class CatalogPage : ContentPage
{
    public List<Category> Categories { get; set; }

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

        BindingContext = this;
    }
}
