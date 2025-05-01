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