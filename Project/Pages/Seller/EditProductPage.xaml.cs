using Project.Models;
using Project.Services;

namespace Project.Pages.Seller;

public partial class EditProductPage : ContentPage
{
    private Product _product;
    private DatabaseService _databaseService;
    private string _imagePath;
    private List<Category> Categories;

    public EditProductPage(Product product, DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _product = product;
        _imagePath = product.Image;
        LoadCategories();
        FillProductFields();
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

        CategoryPicker.ItemsSource = Categories;
    }

    private void FillProductFields()
    {
        NameEntry.Text = _product.Name;
        DescriptionEditor.Text = _product.Description;
        PriceEntry.Text = _product.Price.Replace("₸", "").Trim();

        // Отображаем уже сохраненное фото из байтов, если есть
        if (_product.ImageData != null && _product.ImageData.Length > 0)
        {
            ProductImage.Source = ImageSource.FromStream(() => new MemoryStream(_product.ImageData));
        }
        else if (!string.IsNullOrEmpty(_product.Image))
        {
            ProductImage.Source = ImageSource.FromFile(_product.Image);
        }
        else
        {
            ProductImage.Source = null;
        }

        var selectedCategory = Categories.FirstOrDefault(c => c.Name == _product.Category);
        if (selectedCategory != null)
        {
            CategoryPicker.SelectedItem = selectedCategory;
        }
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
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(DescriptionEditor.Text) ||
            string.IsNullOrWhiteSpace(PriceEntry.Text) ||
            CategoryPicker.SelectedItem is not Category selectedCategory)
        {
            await DisplayAlert("Қате", "Барлық өрістерді толтырыңыз", "OK");
            return;
        }

        _product.Name = NameEntry.Text;
        _product.Description = DescriptionEditor.Text;
        _product.Price = PriceEntry.Text + " ₸";
        _product.Category = selectedCategory.Name;
        _product.Image = _imagePath;


        if (!string.IsNullOrEmpty(_imagePath))
        {
            _product.ImageData = File.ReadAllBytes(_imagePath);
        }

        // Вызываем метод для обновления товара в базе
        await _databaseService.UpdateProductAsync(_product);

        await DisplayAlert("Сәтті", "Тауар жаңартылды", "OK");
        await Navigation.PopAsync();
    }
}