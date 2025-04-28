using Project.Models; // ����� ������ ����� Product

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

    // ���������� ������ �������� ������
    private async void OnSubmitReviewClicked(object sender, EventArgs e)
    {
        string reviewText = ReviewEditor.Text?.Trim();

        if (string.IsNullOrEmpty(reviewText))
        {
            await DisplayAlert("������", "����������, ������� ����� ������.", "OK");
            return;
        }

        // ����� �� ������ ��������� ����� � ���� ������ ��� � API
        // ���� ������ ���������� ���������
        await DisplayAlert("����� ���������", "������� �� ��� �����!", "OK");

        // ������� ����
        ReviewEditor.Text = string.Empty;
    }
}
