using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;

namespace Project.Pages.Client;

public partial class UserReviewsPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;
    public ObservableCollection<UserReviewItem> UserReviews { get; set; }

    public UserReviewsPage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;
        UserReviews = new ObservableCollection<UserReviewItem>();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserReviews();
    }

    private async Task LoadUserReviews()
    {
        UserReviews.Clear();
        int userId = _authService.GetCurrentUserId();

        var reviews = await _databaseService.GetUserReviewsAsync(userId);

        foreach (var review in reviews)
        {
            var product = await _databaseService.GetProductAsync(review.ProductId);
            if (product != null)
            {
                UserReviews.Add(new UserReviewItem
                {
                    ReviewId = review.Id,
                    ProductId = review.ProductId,
                    ProductName = product.Name,
                    ProductImage = product.ImageData,
                    ReviewText = review.Text,
                    CreatedAt = review.CreatedAt
                });
            }
        }
    }

    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is int productId)
        {
            var product = await _databaseService.GetProductAsync(productId);
            if (product != null)
            {
                await Navigation.PushAsync(new ProductDetail(product, _databaseService, _authService));
            }
        }
    }
}