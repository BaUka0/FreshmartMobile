using Project.Models;
using Project.Services;

namespace Project.Pages.Seller;

public partial class AddProductPage : ContentPage
{
    private string _imagePath;
    private DatabaseService _databaseService;
    public List<Category> Categories { get; set; }
    

    public AddProductPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        LoadCategories();
        CategoryPicker.ItemsSource = Categories;
    }

    private void LoadCategories()
    {
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
    }

    private async void OnUploadImageClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Выберите изображение",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            _imagePath = result.FullPath;
            ProductImage.Source = ImageSource.FromFile(_imagePath);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text;
        string description = DescriptionEditor.Text;
        string price = PriceEntry.Text;
        var category = CategoryPicker.SelectedItem as Category;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description)
            || string.IsNullOrWhiteSpace(price) || category == null)
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "OK");
            return;
        }


        byte[] fileBytes = null;
        if (!string.IsNullOrEmpty(_imagePath))
        {
            fileBytes = File.ReadAllBytes(_imagePath);
        }

        var newProduct = new Product
        {
            Name = name,
            Description = description,
            Price = price + " ₸",
            Category = category.Name,
            ImageData = fileBytes
        };


        await _databaseService.CreateProductAsync(newProduct);

        await DisplayAlert("Успешно", "Продукт добавлен", "OK");
        await Navigation.PopAsync();
    }

}