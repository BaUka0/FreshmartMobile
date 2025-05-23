﻿using System.Collections.ObjectModel;
using Project.Models;
using Project.Services;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using PointF = Syncfusion.Drawing.PointF;

namespace Project.Pages.Client;

public partial class OrderHistoryPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;
    private ObservableCollection<Order> _orders;

    public OrderHistoryPage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;
        _orders = new ObservableCollection<Order>();
        OrdersCollectionView.ItemsSource = _orders;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var userId = _authService.GetCurrentUserId();
            var orders = await _databaseService.GetUserOrdersAsync(userId);

            _orders.Clear();
            foreach (var order in orders)
            {
                _orders.Add(order);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Қате", $"Тапсырыс журналы жүктелмеді: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnOrderDetailClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Order order)
        {
            await Navigation.PushAsync(new OrderDetailPage(order));
        }
    }

    private async void OnDownloadReceiptClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Order order)
        {
            try
            {
                await GenerateOrderReceiptPdf(order);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Қате", $"PDF жасау сәтсіз аяқталды: {ex.Message}", "OK");
            }
        }
    }

    private async Task GenerateOrderReceiptPdf(Order order)
    {
        var document = new PdfDocument();
        var page = document.Pages.Add();
        var graphics = page.Graphics;
        float y = 10;

        var fontStream = typeof(App).Assembly.GetManifestResourceStream("Project.Resources.Fonts.arial.ttf");
        var font = new PdfTrueTypeFont(fontStream, 12);

        graphics.DrawString("Тапсырыс чегі", new PdfTrueTypeFont(fontStream, 18),
            PdfBrushes.DarkBlue, new PointF(0, y));
        y += 30;

        graphics.DrawString($"Тапсырыс №{order.Id}", font,
            PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Күн: {order.OrderDate:dd.MM.yyyy HH:mm}", font,
            PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Жеткізу әдісі: {order.DeliveryMethod}", font,
            PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Төлем әдісі: {order.PaymentMethod}", font,
            PdfBrushes.Black, new PointF(0, y));
        y += 20;

        if (!string.IsNullOrEmpty(order.Address))
        {
            graphics.DrawString($"Жеткізу мекенжайы: {order.Address}", font,
                PdfBrushes.Black, new PointF(0, y));
            y += 20;
        }

        graphics.DrawString("Тауарлар реті бойынша:", font,
            PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString("Тауар", font, PdfBrushes.Black, new PointF(0, y));
        graphics.DrawString("Саны", font, PdfBrushes.Black, new PointF(200, y));
        graphics.DrawString("Бағасы", font, PdfBrushes.Black, new PointF(300, y));
        graphics.DrawString("Сомма", font, PdfBrushes.Black, new PointF(400, y));
        y += 20;

        foreach (var item in order.Items)
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
        graphics.DrawString($"Барлығы: {order.TotalPrice}", new PdfTrueTypeFont(fontStream, 14, PdfFontStyle.Bold),
            PdfBrushes.Black, new PointF(0, y));

        var fileName = $"order_{order.Id}_{order.OrderDate:yyyyMMdd}.pdf";
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
