using Project.Models;
using Project.Services;

namespace Project.Pages;

public partial class ProductDetail : ContentPage
{
    private Product _product;
    private DatabaseService _databaseService;

    public ProductDetail(Product product, DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _product = product;

        BindingContext = _product;

        ProductNameLabel.Text = _product.Name;
        ProductDescriptionLabel.Text = _product.Description;
        ProductPriceLabel.Text = _product.Price;
    }

    // Обработчик кнопки отправки отзыва
    private async void OnSubmitReviewClicked(object sender, EventArgs e)
    {
        string reviewText = ReviewEditor.Text?.Trim();

        if (string.IsNullOrEmpty(reviewText))
        {
            await DisplayAlert("Ошибка", "Пожалуйста, введите текст отзыва.", "OK");
            return;
        }

        // Здесь ты можешь сохранить отзыв в базу данных или в API
        // Пока просто показываем сообщение
        await DisplayAlert("Отзыв отправлен", "Спасибо за ваш отзыв!", "OK");

        // Очищаем поле
        ReviewEditor.Text = string.Empty;
    }
}