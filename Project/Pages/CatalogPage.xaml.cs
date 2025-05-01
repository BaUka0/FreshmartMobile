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
            new Category { Name = "�������", Icon = "drink_icon.png" },
            new Category { Name = "������", Icon = "fruit_icon.png" },
            new Category { Name = "�����", Icon = "vegetables_icon.png" },
            new Category { Name = "����", Icon = "meat_icon.png" },
            new Category { Name = "�����������", Icon = "gastronomy.png" },
            new Category { Name = "�������� ��������", Icon = "dairy_icon.png" },
            new Category { Name = "�������", Icon = "grocery.png" },
            new Category { Name = "���� � �������", Icon = "bread_icon.png" },
            new Category { Name = "���, ����, �����", Icon = "tea.png" },
            new Category { Name = "������� � ��������", Icon = "candies_icon.png" },
            new Category { Name = "����������� ������������", Icon = "cake.png" },
            new Category { Name = "��������", Icon = "canned.png" },
            new Category { Name = "������������ ��������", Icon = "frozen_icon.png" },
            new Category { Name = "������ ������", Icon = "others.png" },
        };

        // ������� ������� ��� ��������� ������� �� ���������
        CategoryCommand = new Command<Category>(OnCategorySelected);

        BindingContext = this;
    }

    private async void OnCategorySelected(Category selectedCategory)
    {
        if (selectedCategory == null) return;

        // ������� �� �������� ProductList � ��������� ����������
        await Navigation.PushAsync(new ProductListPage(selectedCategory, _databaseService, _authService));
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