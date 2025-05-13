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
        new Category { Name = "Сусындар", Icon = "drink_icon.png" },
        new Category { Name = "Жемістер", Icon = "fruit_icon.png" },
        new Category { Name = "Көкөністер", Icon = "vegetables_icon.png" },
        new Category { Name = "Ет өнімдері", Icon = "meat_icon.png" },
        new Category { Name = "Аспаздық өнімдер", Icon = "gastronomy.png" },
        new Category { Name = "Сүт өнімдері", Icon = "dairy_icon.png" },
        new Category { Name = "Бакалея", Icon = "grocery.png" },
        new Category { Name = "Нан және тоқаштар", Icon = "bread_icon.png" },
        new Category { Name = "Шай, кофе, какао", Icon = "tea.png" },
        new Category { Name = "Кәмпиттер мен тәттілер", Icon = "candies_icon.png" },
        new Category { Name = "Өз өндірісіміз", Icon = "cake.png" },
        new Category { Name = "Консервілер", Icon = "canned.png" },
        new Category { Name = "Мұздатылған өнімдер", Icon = "frozen_icon.png" },
        new Category { Name = "Басқа тауарлар", Icon = "others.png" },
    };
}


    private async void OnUploadImageClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Суретті таңдаңыз",
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
            await DisplayAlert("Қате", "Барлық өрістерді толтырыңыз", "OK");
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

        await DisplayAlert("Сәтті", "Тауар қосылды", "OK");
        await Navigation.PopAsync();
    }

}