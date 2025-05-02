using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Project.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using PointF = Syncfusion.Drawing.PointF;
using Project.Services;

namespace Project.Pages.Client;

public partial class OrderSummaryPage : ContentPage
{
    private DatabaseService _databaseService;
    private AuthService _authService;
    private ObservableCollection<Product> OrderItems;

    public OrderSummaryPage(ObservableCollection<Product> items, DatabaseService database, AuthService authService)
    {
        InitializeComponent();
        _databaseService = database;
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
        _authService = authService;
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
        // Создаем новый заказ
        string address = null;
        if (AddressSection.IsVisible)
        {
            // Объединяем данные из двух полей ввода, проверяя, что они не пустые
            if (!string.IsNullOrEmpty(CityEntry.Text) && !string.IsNullOrEmpty(StreetEntry.Text))
            {
                address = $"{CityEntry.Text}, {StreetEntry.Text}";
            }
            else
            {
                await DisplayAlert("Ошибка", "Пожалуйста, заполните адрес доставки", "OK");
                return;
            }
        }

        var order = new Order
        {
            UserId = _authService.GetCurrentUserId(),
            DeliveryMethod = DeliveryPicker.SelectedItem?.ToString() ?? "Самовывоз",
            PaymentMethod = PaymentPicker.SelectedItem?.ToString() ?? "Наличные",
            Address = address,
            OrderDate = DateTime.Now,
            OrderStatus = "Обработка"
        };

        // Вычисляем общую стоимость
        int totalPrice = 0;
        foreach (var item in OrderItems)
        {
            if (int.TryParse(item.Price.Replace("₸", "").Trim(), out int price))
                totalPrice += price * item.Quantity;
        }
        order.TotalPrice = $"{totalPrice} ₸";

        // Добавляем элементы заказа
        order.Items = new List<OrderItem>();
        foreach (var item in OrderItems)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.Id,
                ProductName = item.Name,
                ProductPrice = item.Price,
                Quantity = item.Quantity,
                ProductImageData = item.ImageData // Добавляем передачу изображения
            });
        }


        // Сохраняем заказ в базу данных
        try
        {
            await _databaseService.CreateOrderAsync(order);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось сохранить заказ: {ex.Message}", "OK");
            return;
        }

        try
        {
            // Генерируем и показываем PDF-чек
            await GenerateOrderReceiptPdf(order);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось создать PDF-чек: {ex.Message}", "OK");
        }

        await Navigation.PopToRootAsync();
    }

    private async Task GenerateOrderReceiptPdf(Order order)
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
        graphics.DrawString($"Заказ №{order.Id}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Дата заказа
        graphics.DrawString($"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Статус заказа
        graphics.DrawString($"Статус: {order.OrderStatus}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Способ доставки
        graphics.DrawString($"Способ доставки: {order.DeliveryMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Способ оплаты
        graphics.DrawString($"Способ оплаты: {order.PaymentMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        // Адрес доставки (если есть)
        if (!string.IsNullOrEmpty(order.Address))
        {
            graphics.DrawString($"Адрес доставки: {order.Address}", font, PdfBrushes.Black, new PointF(0, y));
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

        // Итоговая стоимость
        y += 10;
        graphics.DrawString($"Итого: {order.TotalPrice}", boldFont, PdfBrushes.Black, new PointF(0, y));

        // Сохранение и открытие PDF
        var fileName = $"order_{order.Id}_{order.OrderDate:yyyyMMdd_HHmmss}.pdf";
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
