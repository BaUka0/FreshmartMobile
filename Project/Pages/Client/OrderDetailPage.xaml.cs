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

        // Заполняем информацию о заказе
        OrderNumberLabel.Text = $"Заказ №{_order.Id}";
        OrderDateLabel.Text = $"Дата: {_order.OrderDate:dd.MM.yyyy HH:mm}";
        OrderStatusLabel.Text = $"Статус: {_order.OrderStatus}";
        DeliveryMethodLabel.Text = $"Способ доставки: {_order.DeliveryMethod}";
        PaymentMethodLabel.Text = $"Способ оплаты: {_order.PaymentMethod}";

        if (!string.IsNullOrEmpty(_order.Address))
        {
            AddressLabel.Text = $"Адрес доставки: {_order.Address}";
            AddressLabel.IsVisible = true;
        }

        // Отображаем товары заказа
        OrderItemsCollectionView.ItemsSource = _order.Items;

        // Отображаем итоговую стоимость
        TotalPriceLabel.Text = $"Итого: {_order.TotalPrice}";
    }

    private async void OnDownloadReceiptClicked(object sender, EventArgs e)
    {
        try
        {
            await GenerateReceiptPdf();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось создать PDF: {ex.Message}", "OK");
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

        // Заголовок
        graphics.DrawString("Чек заказа", titleFont, PdfBrushes.DarkBlue, new PointF(0, y));
        y += 30;

        // Номер заказа
        graphics.DrawString($"Заказ №{_order.Id}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Дата заказа
        graphics.DrawString($"Дата: {_order.OrderDate:dd.MM.yyyy HH:mm}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Статус заказа
        graphics.DrawString($"Статус: {_order.OrderStatus}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Способ доставки
        graphics.DrawString($"Способ доставки: {_order.DeliveryMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Способ оплаты
        graphics.DrawString($"Способ оплаты: {_order.PaymentMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Адрес доставки (если есть)
        if (!string.IsNullOrEmpty(_order.Address))
        {
            graphics.DrawString($"Адрес доставки: {_order.Address}", font, PdfBrushes.Black, new PointF(0, y));
            y += 20;
        }

        // Заголовок таблицы товаров
        graphics.DrawString("Список товаров:", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Заголовки столбцов
        graphics.DrawString("Товар", font, PdfBrushes.Black, new PointF(0, y));
        graphics.DrawString("Кол-во", font, PdfBrushes.Black, new PointF(200, y));
        graphics.DrawString("Цена", font, PdfBrushes.Black, new PointF(300, y));
        graphics.DrawString("Сумма", font, PdfBrushes.Black, new PointF(400, y));
        y += 20;

        // Строки с товарами
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

        // Итоговая стоимость
        y += 10;
        graphics.DrawString($"Итого: {_order.TotalPrice}", boldFont, PdfBrushes.Black, new PointF(0, y));

        // Сохранение и открытие PDF
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
