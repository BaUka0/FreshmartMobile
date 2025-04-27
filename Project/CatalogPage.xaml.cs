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
            new Category { Name = "�������", Icon = "drink_icon.png" },
            new Category { Name = "������", Icon = "fruit_icon.png" },
            new Category { Name = "�����", Icon = "vegetables_icon.png" },
            new Category { Name = "����", Icon = "meat_icon.png" },
            new Category { Name = "�������� ��������", Icon = "dairy_icon.png" },
            new Category { Name = "���� � �������", Icon = "bread_icon.png" },
            new Category { Name = "������� � ��������", Icon = "candies_icon.png" },
            new Category { Name = "������������ ��������", Icon = "frozen_icon.png" }
        };

        BindingContext = this;
    }
}
