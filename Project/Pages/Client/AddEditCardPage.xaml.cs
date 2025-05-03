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

            Title = _isEditing ? "Редактировать карту" : "Добавить карту";

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
                await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля", "OK");
                return;
            }

            // Базовая валидация номера карты
            if (CardNumberEntry.Text.Length < 16 || CardNumberEntry.Text.Length > 19)
            {
                await DisplayAlert("Ошибка", "Неверный формат номера карты", "OK");
                return;
            }

            // Базовая валидация срока действия
            if (!ExpiryDateEntry.Text.Contains("/") || ExpiryDateEntry.Text.Length != 5)
            {
                await DisplayAlert("Ошибка", "Неверный формат срока действия карты (MM/YY)", "OK");
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
                    await DisplayAlert("Успех", "Карта успешно обновлена", "OK");
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
                    await DisplayAlert("Успех", "Новая карта добавлена", "OK");
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось сохранить карту: {ex.Message}", "OK");
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
                "Подтверждение удаления",
                "Вы уверены, что хотите удалить эту карту?",
                "Да, удалить",
                "Отмена");

            if (confirm)
            {
                try
                {
                    await _databaseService.DeletePaymentCardAsync(_editingCard.Id);
                    await DisplayAlert("Успех", "Карта успешно удалена", "OK");
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Не удалось удалить карту: {ex.Message}", "OK");
                }
            }
        }
    }
}
