using Project.Models;
using System.Collections.ObjectModel;

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
        string delivery = DeliveryPicker.SelectedItem?.ToString() ?? "не выбран";
        string payment = PaymentPicker.SelectedItem?.ToString() ?? "не выбран";

        await DisplayAlert("Заказ подтвержден",
            $"Вы выбрали:\nДоставка: {delivery}\nОплата: {payment}",
            "ОК");

        await Navigation.PopToRootAsync();
    }
}