using Project.Models;
using Project.Services;

namespace Project.Pages.Client
{
    public partial class AddEditCardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly PaymentCard _editingCard;
        private bool _isEditing;

        public bool IsEditing => _isEditing;

        public AddEditCardPage(DatabaseService databaseService, AuthService authService, PaymentCard card = null)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _authService = authService;
            _editingCard = card;
            _isEditing = card != null;

            Title = _isEditing ? "Картаны өңдеу" : "Картаны қосу";

            // Кнопка удаления видна только при редактировании
            DeleteButton.IsVisible = _isEditing;

            if (_isEditing)
            {
                CardNumberEntry.Text = card.CardNumber;
                CardHolderEntry.Text = card.CardHolderName;
                ExpiryDateEntry.Text = card.ExpiryDate;
                DefaultCardSwitch.IsToggled = card.IsDefault;
            }

            BindingContext = this;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CardNumberEntry.Text) ||
                string.IsNullOrWhiteSpace(CardHolderEntry.Text) ||
                string.IsNullOrWhiteSpace(ExpiryDateEntry.Text))
            {
                await DisplayAlert("Қате", "Барлық өрістерді толтырыңыз", "OK");
                return;
            }

            // Базовая валидация номера карты
            if (CardNumberEntry.Text.Length < 16 || CardNumberEntry.Text.Length > 19)
            {
                await DisplayAlert("Қате", "Жарамсыз карта нөмірінің пішімі", "OK");
                return;
            }

            // Базовая валидация срока действия
            if (!ExpiryDateEntry.Text.Contains("/") || ExpiryDateEntry.Text.Length != 5)
            {
                await DisplayAlert("Қате", "Жарамсыз картаның жарамдылық мерзімі пішімі (MM/YY)", "OK");
                return;
            }

            try
            {
                PaymentCard card;

                if (_isEditing)
                {
                    card = _editingCard;
                    card.CardNumber = CardNumberEntry.Text;
                    card.CardHolderName = CardHolderEntry.Text;
                    card.ExpiryDate = ExpiryDateEntry.Text;
                    card.IsDefault = DefaultCardSwitch.IsToggled;

                    await _databaseService.UpdatePaymentCardAsync(card);
                    await DisplayAlert("Сәтті", "Карта сәтті жаңартылды.", "OK");
                }
                else
                {
                    card = new PaymentCard
                    {
                        UserId = _authService.GetCurrentUserId(),
                        CardNumber = CardNumberEntry.Text,
                        CardHolderName = CardHolderEntry.Text,
                        ExpiryDate = ExpiryDateEntry.Text,
                        IsDefault = DefaultCardSwitch.IsToggled
                    };

                    await _databaseService.AddPaymentCardAsync(card);
                    await DisplayAlert("Сәтті", "Жаңа карта қосылды", "OK");
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Қате", $"Картаны сақтау мүмкін болмады: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (!_isEditing || _editingCard == null)
                return;

            bool confirm = await DisplayAlert(
                "Жоюды растау",
                "Бұл картаны шынымен жойғыңыз келе ме?",
                "Иә, жою",
                "Болдырмау");

            if (confirm)
            {
                try
                {
                    await _databaseService.DeletePaymentCardAsync(_editingCard.Id);
                    await DisplayAlert("Сәтті", "Карта сәтті жойылды.", "OK");
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Қате", $"Картаны жою мүмкін болмады: {ex.Message}", "OK");
                }
            }
        }
    }
}
