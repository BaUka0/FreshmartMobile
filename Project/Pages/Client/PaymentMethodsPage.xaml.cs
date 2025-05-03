using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;

namespace Project.Pages.Client
{
    public partial class PaymentMethodsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        public ObservableCollection<PaymentCard> PaymentCards { get; set; } = new();

        public PaymentMethodsPage(DatabaseService databaseService, AuthService authService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _authService = authService;

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPaymentCards();
        }

        private async Task LoadPaymentCards()
        {
            PaymentCards.Clear();
            var userId = _authService.GetCurrentUserId();
            var cards = await _databaseService.GetPaymentCardsAsync(userId);

            foreach (var card in cards)
            {
                PaymentCards.Add(card);
            }
        }

        private async void OnAddCardClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditCardPage(_databaseService, _authService));
        }

        private async void OnEditCardClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int cardId)
            {
                var userId = _authService.GetCurrentUserId();
                var cards = await _databaseService.GetPaymentCardsAsync(userId);
                var selectedCard = cards.FirstOrDefault(c => c.Id == cardId);

                if (selectedCard != null)
                {
                    await Navigation.PushAsync(new AddEditCardPage(_databaseService, _authService, selectedCard));
                }
            }
        }

        private async void OnCardSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is PaymentCard selectedCard)
            {
                // ������� ���������
                ((CollectionView)sender).SelectedItem = null;

                string action = await DisplayActionSheet("�������� � ������", "������", null,
                    "������� ��������", "�������������", "�������");

                switch (action)
                {
                    case "������� ��������":
                        selectedCard.IsDefault = true;
                        await _databaseService.UpdatePaymentCardAsync(selectedCard);
                        await LoadPaymentCards();
                        await DisplayAlert("�����", "����� ����������� ��� ��������", "OK");
                        break;
                    case "�������������":
                        await Navigation.PushAsync(new AddEditCardPage(_databaseService, _authService, selectedCard));
                        break;
                    case "�������":
                        bool confirm = await DisplayAlert("�������������",
                            "�� �������, ��� ������ ������� ��� �����?", "��", "���");
                        if (confirm)
                        {
                            await _databaseService.DeletePaymentCardAsync(selectedCard.Id);
                            await LoadPaymentCards();
                            await DisplayAlert("�����", "����� ������� �������", "OK");
                        }
                        break;
                }
            }
        }
    }
}
