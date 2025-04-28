using Project.Models; // Чтобы видеть класс Product

namespace Project.Pages;

public partial class ProductDetail : ContentPage
{
    private Product _product;

    public ProductDetail(Product product)
    {
        InitializeComponent();
        _product = product;

        ProductNameLabel.Text = _product.Name;
        ProductDescriptionLabel.Text = _product.Description;
        ProductPriceLabel.Text = _product.Price;
        ProductImage.Source = _product.Image ?? "defaultProduct.png";
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
