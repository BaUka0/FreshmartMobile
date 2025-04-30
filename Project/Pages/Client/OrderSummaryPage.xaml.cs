using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Project.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using PointF = Syncfusion.Drawing.PointF;

namespace Project.Pages.Client;

public partial class OrderSummaryPage : ContentPage
{
    private ObservableCollection<Product> OrderItems;

    public OrderSummaryPage(ObservableCollection<Product> items)
    {
        InitializeComponent();
        OrderItems = items;
        SummaryCollectionView.ItemsSource = OrderItems;

        int total = 0;
        foreach (var item in OrderItems)
        {
            if (int.TryParse(item.Price.Replace("₸", "").Trim(), out int price))
                total += price * item.Quantity;
        }

        TotalLabel.Text = $"Общая стоимость: {total} ₸";

        DeliveryPicker.SelectedIndexChanged += OnDeliveryChanged;
        PaymentPicker.SelectedIndexChanged += OnPaymentChanged;
    }

    private void OnDeliveryChanged(object sender, EventArgs e)
    {
        AddressSection.IsVisible = DeliveryPicker.SelectedItem?.ToString() == "Курьер";
    }

    private void OnPaymentChanged(object sender, EventArgs e)
    {
        CardSection.IsVisible = PaymentPicker.SelectedItem?.ToString() == "Карта";
    }
    private async void OnConfirmOrderClicked(object sender, EventArgs e)
    {
        var document = new PdfDocument();
        var page = document.Pages.Add();
        var graphics = page.Graphics;
        float y = 10;

        var fontStream = typeof(App).Assembly.GetManifestResourceStream("Project.Resources.Fonts.arial.ttf");
        var font = new PdfTrueTypeFont(fontStream, 12);

        graphics.DrawString("Чек заказа", new PdfTrueTypeFont(fontStream, 18),
            PdfBrushes.DarkBlue, new PointF(0, y));
        y += 30;

        graphics.DrawString($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}", font,
            PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString("Товар", font, PdfBrushes.Black, new PointF(0, y));
        graphics.DrawString("Кол-во", font, PdfBrushes.Black, new PointF(200, y));
        graphics.DrawString("Цена", font, PdfBrushes.Black, new PointF(300, y));
        graphics.DrawString("Сумма", font, PdfBrushes.Black, new PointF(400, y));
        y += 20;

        int totalPrice = 0;

        foreach (var item in OrderItems)
        {
            if (int.TryParse(item.Price.Replace("₸", "").Trim(), out int price))
            {
                int itemTotal = price * item.Quantity;
                totalPrice += itemTotal;

                graphics.DrawString(item.Name, font, PdfBrushes.Black, new PointF(0, y));
                graphics.DrawString(item.Quantity.ToString(), font, PdfBrushes.Black, new PointF(200, y));
                graphics.DrawString($"{price} ₸", font, PdfBrushes.Black, new PointF(300, y));
                graphics.DrawString($"{itemTotal} ₸", font, PdfBrushes.Black, new PointF(400, y));
                y += 20;
            }
        }

        y += 10;
        graphics.DrawString($"Итого: {totalPrice} ₸", new PdfTrueTypeFont(fontStream, 14, PdfFontStyle.Bold),
            PdfBrushes.Black, new PointF(0, y));
        y += 30;

        string delivery = DeliveryPicker.SelectedItem?.ToString() ?? "не выбран";
        string payment = PaymentPicker.SelectedItem?.ToString() ?? "не выбран";
        graphics.DrawString($"Доставка: {delivery}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;
        graphics.DrawString($"Оплата: {payment}", font, PdfBrushes.Black, new PointF(0, y));

        var fileName = $"order_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        using (var stream = File.Create(filePath))
            document.Save(stream);
        document.Close(true);

        await Launcher.Default.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });

        await Navigation.PopToRootAsync();
    }
}