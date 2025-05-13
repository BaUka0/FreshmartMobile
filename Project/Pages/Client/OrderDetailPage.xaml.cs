using Project.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using PointF = Syncfusion.Drawing.PointF;

namespace Project.Pages.Client;

public partial class OrderDetailPage : ContentPage
{
    private readonly Order _order;

    public OrderDetailPage(Order order)
    {
        InitializeComponent();
        _order = order;

        OrderNumberLabel.Text = $"Тапсырыс №{_order.Id}";
        OrderDateLabel.Text = $"Күн: {_order.OrderDate:dd.MM.yyyy HH:mm}";
        OrderStatusLabel.Text = $"Статус: {_order.OrderStatus}";
        DeliveryMethodLabel.Text = $"Жеткізу әдісі: {_order.DeliveryMethod}";
        PaymentMethodLabel.Text = $"Төлем әдісі: {_order.PaymentMethod}";

        if (!string.IsNullOrEmpty(_order.Address))
        {
            AddressLabel.Text = $"Жеткізу мекенжайы: {_order.Address}";
            AddressLabel.IsVisible = true;
        }

        OrderItemsCollectionView.ItemsSource = _order.Items;

        TotalPriceLabel.Text = $"Барлығы: {_order.TotalPrice}";
    }

    private async void OnDownloadReceiptClicked(object sender, EventArgs e)
    {
        try
        {
            await GenerateReceiptPdf();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Қате", $"PDF жасау сәтсіз аяқталды: {ex.Message}", "OK");
        }
    }

    private async Task GenerateReceiptPdf()
    {
        var document = new PdfDocument();
        var page = document.Pages.Add();
        var graphics = page.Graphics;
        float y = 10;

        var fontStream = typeof(App).Assembly.GetManifestResourceStream("Project.Resources.Fonts.arial.ttf");
        var titleFont = new PdfTrueTypeFont(fontStream, 18);
        var font = new PdfTrueTypeFont(fontStream, 12);
        var boldFont = new PdfTrueTypeFont(fontStream, 14, PdfFontStyle.Bold);

        graphics.DrawString("Тапсырыс чегі", titleFont, PdfBrushes.DarkBlue, new PointF(0, y));
        y += 30;

        graphics.DrawString($"Тапсырыс №{_order.Id}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Күн: {_order.OrderDate:dd.MM.yyyy HH:mm}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Статус: {_order.OrderStatus}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Жеткізу әдісі: {_order.DeliveryMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Төлем әдісі: {_order.PaymentMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        if (!string.IsNullOrEmpty(_order.Address))
        {
            graphics.DrawString($"Жеткізу мекенжайы: {_order.Address}", font, PdfBrushes.Black, new PointF(0, y));
            y += 20;
        }

        graphics.DrawString("Тауарлар реті бойынша:", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString("Тауар", font, PdfBrushes.Black, new PointF(0, y));
        graphics.DrawString("Саны", font, PdfBrushes.Black, new PointF(200, y));
        graphics.DrawString("Бағасы", font, PdfBrushes.Black, new PointF(300, y));
        graphics.DrawString("Сомма", font, PdfBrushes.Black, new PointF(400, y));
        y += 20;

        foreach (var item in _order.Items)
        {
            if (int.TryParse(item.ProductPrice.Replace("₸", "").Trim(), out int price))
            {
                int itemTotal = price * item.Quantity;

                graphics.DrawString(item.ProductName, font, PdfBrushes.Black, new PointF(0, y));
                graphics.DrawString(item.Quantity.ToString(), font, PdfBrushes.Black, new PointF(200, y));
                graphics.DrawString($"{price} ₸", font, PdfBrushes.Black, new PointF(300, y));
                graphics.DrawString($"{itemTotal} ₸", font, PdfBrushes.Black, new PointF(400, y));
                y += 20;
            }
        }

        y += 10;
        graphics.DrawString($"Барлығы: {_order.TotalPrice}", boldFont, PdfBrushes.Black, new PointF(0, y));

        var fileName = $"order_{_order.Id}_{_order.OrderDate:yyyyMMdd}.pdf";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        using (var stream = File.Create(filePath))
            document.Save(stream);
        document.Close(true);

        await Launcher.Default.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }

}
