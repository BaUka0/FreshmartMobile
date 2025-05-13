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
    private List<PaymentCard> _userCards;
    private PaymentCard _selectedCard;

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

        TotalLabel.Text = $"Жалпы құны: {total} ₸";

        DeliveryPicker.SelectedIndexChanged += OnDeliveryChanged;
        PaymentPicker.SelectedIndexChanged += OnPaymentChanged;
        _authService = authService;
        LoadUserCardsAsync();
    }
    private async Task LoadUserCardsAsync()
    {
        var userId = _authService.GetCurrentUserId();
        _userCards = await _databaseService.GetPaymentCardsAsync(userId);

        if (_userCards != null && _userCards.Count > 0)
        {
            List<string> paymentOptions = new List<string> { "Қолма-қол ақшамен", "Карта" };

            foreach (var card in _userCards)
            {
                paymentOptions.Add($"Карта {card.MaskedCardNumber}");
            }

            PaymentPicker.ItemsSource = paymentOptions;
        }
    }
    private void OnDeliveryChanged(object sender, EventArgs e)
    {
        AddressSection.IsVisible = DeliveryPicker.SelectedItem?.ToString() == "Курьер";
    }

    private void OnPaymentChanged(object sender, EventArgs e)
    {
        string selectedPayment = PaymentPicker.SelectedItem?.ToString();

        if (selectedPayment == "Карта")
        {
            CardSection.IsVisible = true;
            _selectedCard = null;
        }
        else if (selectedPayment != null && selectedPayment.StartsWith("Карта "))
        {
            string maskedNumber = selectedPayment.Substring(6);
            _selectedCard = _userCards.FirstOrDefault(c => c.MaskedCardNumber == maskedNumber);
            CardSection.IsVisible = false;
        }
        else
        {
            CardSection.IsVisible = false;
            _selectedCard = null;
        }
    }

    private async void OnConfirmOrderClicked(object sender, EventArgs e)
    {
        string address = null;
        if (AddressSection.IsVisible)
        {
            if (!string.IsNullOrEmpty(CityEntry.Text) && !string.IsNullOrEmpty(StreetEntry.Text))
            {
                address = $"{CityEntry.Text}, {StreetEntry.Text}";
            }
            else
            {
                await DisplayAlert("Қате", "Жеткізу мекенжайын толтырыңыз", "OK");
                return;
            }
        }

        string paymentMethod = PaymentPicker.SelectedItem?.ToString() ?? "Наличные";

        if (paymentMethod == "Карта" &&
            !string.IsNullOrWhiteSpace(CardNumberEntry.Text) &&
            !string.IsNullOrWhiteSpace(ExpiryEntry.Text))
        {
            bool saveCard = await DisplayAlert("Картаны сақтау керек пе?",
                "Бұл картаны болашақ сатып алулар үшін сақтағыңыз келе ме?", "Иә", "Жоқ");

            if (saveCard)
            {
                PaymentCard newCard = new PaymentCard
                {
                    UserId = _authService.GetCurrentUserId(),
                    CardNumber = CardNumberEntry.Text,
                    ExpiryDate = ExpiryEntry.Text,
                    CardHolderName = "Карта иесі",
                    IsDefault = false
                };

                await _databaseService.AddPaymentCardAsync(newCard);
            }
        }

        var order = new Order
        {
            UserId = _authService.GetCurrentUserId(),
            DeliveryMethod = DeliveryPicker.SelectedItem?.ToString() ?? "Ала кету",
            PaymentMethod = paymentMethod,
            Address = address,
            OrderDate = DateTime.Now,
            OrderStatus = "Өңдеу"
        };

        int totalPrice = 0;
        foreach (var item in OrderItems)
        {
            if (int.TryParse(item.Price.Replace("₸", "").Trim(), out int price))
                totalPrice += price * item.Quantity;
        }
        order.TotalPrice = $"{totalPrice} ₸";

        order.Items = new List<OrderItem>();
        foreach (var item in OrderItems)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.Id,
                ProductName = item.Name,
                ProductPrice = item.Price,
                Quantity = item.Quantity,
                ProductImageData = item.ImageData
            });
        }


        try
        {
            await _databaseService.CreateOrderAsync(order);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Қате", $"Тапсырыс сақталмады: {ex.Message}", "OK");
            return;
        }

        try
        {
            await GenerateOrderReceiptPdf(order);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"PDF жасау сәтсіз аяқталды: {ex.Message}", "OK");
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

        graphics.DrawString("Тапсырыс чегі", titleFont, PdfBrushes.DarkBlue, new PointF(0, y));
        y += 30;

        graphics.DrawString($"Тапсырыс №{order.Id}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Күн: {order.OrderDate:dd.MM.yyyy HH:mm}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Статус: {order.OrderStatus}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Жеткізу әдісі: {order.DeliveryMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        graphics.DrawString($"Төлем әдісі: {order.PaymentMethod}", font, PdfBrushes.Black, new PointF(0, y));
        y += 20;

        if (!string.IsNullOrEmpty(order.Address))
        {
            graphics.DrawString($"Жеткізу мекенжайы: {order.Address}", font, PdfBrushes.Black, new PointF(0, y));
            y += 20;
        }

        graphics.DrawString("Тауарлар реті бойынша:", font, PdfBrushes.Black, new PointF(0, y));
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
        graphics.DrawString($"Барлығы: {order.TotalPrice}", boldFont, PdfBrushes.Black, new PointF(0, y));

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
