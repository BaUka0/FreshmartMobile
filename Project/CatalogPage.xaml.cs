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
            new Category { Name = "�������", Icon = "drink_icon.png" },
            new Category { Name = "������", Icon = "fruit_icon.png" },
            new Category { Name = "�����", Icon = "vegetables_icon.png" },
            new Category { Name = "����", Icon = "meat_icon.png" },
            new Category { Name = "�������� ��������", Icon = "dairy_icon.png" },
            new Category { Name = "���� � �������", Icon = "bread_icon.png" },
            new Category { Name = "������� � ��������", Icon = "candies_icon.png" },
            new Category { Name = "������������ ��������", Icon = "frozen_icon.png" }
        };

        // ������� ������� ��� ��������� ������� �� ���������
        CategoryCommand = new Command<Category>(OnCategorySelected);

        BindingContext = this;
    }

    private async void OnCategorySelected(Category selectedCategory)
    {
        if (selectedCategory == null) return;

        // ������� �� �������� ProductList � ��������� ����������
        await Navigation.PushAsync(new ProductList(selectedCategory));
    }
    private async void OnCategoryTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        if (frame != null)
        {
            // �������� ���������� � ����������
            await frame.ScaleTo(0.9, 100, Easing.CubicIn);
            await frame.ScaleTo(1, 100, Easing.CubicOut);
        }
    }
}

