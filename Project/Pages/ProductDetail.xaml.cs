using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;

namespace Project.Pages;

public partial class ProductDetail : ContentPage
{
    private Product _product;
    private DatabaseService _databaseService;
    private AuthService _authService;

    public ProductDetail(Product product, DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;
        _product = product;

        BindingContext = _product;

        ProductNameLabel.Text = _product.Name;
        ProductDescriptionLabel.Text = _product.Description;
        ProductPriceLabel.Text = _product.Price;

        var userRole = _authService.GetCurrentUserRole();
        var isClient = userRole == "client";

        ActionButtonsSection.IsVisible = isClient;
        AddReviewSection.IsVisible = isClient;

        if (isClient)
        {
            InitializeClientButtons();
        }

        LoadReviews();
    }

    private async void InitializeClientButtons()
    {
        var userId = _authService.GetCurrentUserId();

        var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, _product.Id);
        FavoriteButton.Source = isFavorite ? "favourite_green.png" : "favourite_grey.png";

        var cartItems = await _databaseService.GetCartItemsAsync(userId);
        var isInCart = cartItems.Any(ci => ci.ProductId == _product.Id);
        CartButton.Source = isInCart ? "basket_green.png" : "basket_grey.png";
    }

    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        var userId = _authService.GetCurrentUserId();
        var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, _product.Id);

        if (isFavorite)
        {
            await _databaseService.RemoveFavoriteProductAsync(userId, _product.Id);
            FavoriteButton.Source = "favourite_grey.png";
            await DisplayAlert("Таңдаулылар", "Тауар таңдаулылардан жойылды", "Ок");
        }
        else
        {
            await _databaseService.AddFavoriteProductAsync(userId, _product.Id);
            FavoriteButton.Source = "favourite_green.png";
            await DisplayAlert("Таңдаулылар", "Тауар таңдаулыларыңызға қосылды", "Ок");
        }
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        var userId = _authService.GetCurrentUserId();

        var cartItems = await _databaseService.GetCartItemsAsync(userId);
        var isInCart = cartItems.Any(ci => ci.ProductId == _product.Id);

        if (isInCart)
        {
            await DisplayAlert("Себет", "Тауар қазірдің өзінде себетке салынған!", "Ок");
        }
        else
        {
            await _databaseService.AddToCartAsync(userId, _product.Id, 1);
            CartButton.Source = "basket_green.png";
            await DisplayAlert("Қосылды", "Тауар себетке қосылды!", "Ок");
        }
    }

    private async void LoadReviews()
    {
        var reviews = await _databaseService.GetProductReviewsAsync(_product.Id);

        ReviewsContainer.Children.Clear();

        if (reviews.Count == 0)
        {
            ReviewsContainer.Children.Add(new Label
            {
                Text = "Әзірге пікірлер жоқ. Бірінші болыңыз!",
                TextColor = Colors.Gray,
                FontSize = 16,
                Margin = new Thickness(0, 5, 0, 10)
            });
        }
        else
        {
            foreach (var review in reviews)
            {
                var reviewFrame = new Frame
                {
                    CornerRadius = 10,
                    BorderColor = Colors.LightGray,
                    BackgroundColor = Colors.White,
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var reviewLayout = new VerticalStackLayout();

                var userNameLabel = new Label
                {
                    Text = review.UserName ?? "Қолданушы",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    TextColor = Colors.Black
                };

                var dateLabel = new Label
                {
                    Text = review.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                    FontSize = 12,
                    TextColor = Colors.Gray,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var contentLabel = new Label
                {
                    Text = review.Text,
                    FontSize = 14,
                    TextColor = Colors.Black
                };

                reviewLayout.Children.Add(userNameLabel);
                reviewLayout.Children.Add(dateLabel);
                reviewLayout.Children.Add(contentLabel);

                reviewFrame.Content = reviewLayout;
                ReviewsContainer.Children.Add(reviewFrame);
            }
        }
    }
    private async void OnSubmitReviewClicked(object sender, EventArgs e)
    {
        string reviewText = ReviewEditor.Text?.Trim();

        if (string.IsNullOrEmpty(reviewText))
        {
            await DisplayAlert("Қате", "Пікір мәтінін енгізіңіз.", "OK");
            return;
        }

        if (_authService.CurrentUser == null || _authService.GetCurrentUserRole() != "client")
        {
            await DisplayAlert("Қате", "Тек клиенттер пікір қалдыра алады.", "OK");
            return;
        }

        var review = new Review
        {
            ProductId = _product.Id,
            UserId = _authService.GetCurrentUserId(),
            Text = reviewText
        };

        await _databaseService.AddReviewAsync(review);
        await DisplayAlert("Пікір жіберілді", "Пікіріңізге рахмет!", "OK");

        ReviewEditor.Text = string.Empty;

        LoadReviews();
    }
}
