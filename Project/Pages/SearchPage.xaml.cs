using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Project.Models;
using Project.Services;

namespace Project.Pages
{
    public partial class SearchPage : ContentPage
    {
        private DatabaseService _databaseService;
        private AuthService _authService;
        private List<Product> _allProducts;
        private string _currentSearchText = string.Empty;
        private List<string> _selectedCategories = new List<string>();
        private List<Category> _categories;

        public ObservableCollection<Product> SearchResults { get; set; }

        public SearchPage(string searchText, DatabaseService databaseService = null, AuthService authService = null)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _authService = authService;
            _currentSearchText = searchText ?? string.Empty;

            SearchResults = new ObservableCollection<Product>();
            BindingContext = this;

            searchBar.Text = _currentSearchText;

            InitializeCategories();
            LoadProducts();
        }

        private void InitializeCategories()
        {
            _categories = new List<Category>
                {
                    new Category { Name = "Барлығы", Icon = "all_icon.png" },
                    new Category { Name = "Сусындар", Icon = "drink_icon.png" },
                    new Category { Name = "Жемістер", Icon = "fruit_icon.png" },
                    new Category { Name = "Көкөністер", Icon = "vegetables_icon.png" },
                    new Category { Name = "Ет өнімдері", Icon = "meat_icon.png" },
                    new Category { Name = "Гастрономия", Icon = "gastronomy.png" },
                    new Category { Name = "Сүт өнімдері", Icon = "dairy_icon.png" },
                    new Category { Name = "Бакалея", Icon = "grocery.png" },
                    new Category { Name = "Нан және тоқаштар", Icon = "bread_icon.png" },
                    new Category { Name = "Шай, кофе, какао", Icon = "tea.png" },
                    new Category { Name = "Кәмпиттер мен тәттілер", Icon = "candies_icon.png" },
                    new Category { Name = "Өндірістік өнімдер", Icon = "cake.png" },
                    new Category { Name = "Консервілер", Icon = "canned.png" },
                    new Category { Name = "Мұздатылған өнімдер", Icon = "frozen_icon.png" },
                    new Category { Name = "Басқа тауарлар", Icon = "others.png" },
                };


            foreach (var category in _categories)
            {
                var frame = new Frame
                {
                    Padding = new Thickness(12, 8),
                    CornerRadius = 20,
                    BorderColor = Colors.LightGray,
                    BackgroundColor = category.Name == "<Барлығы>" ? Color.FromArgb("#E8F5E9") : Colors.White,
                    HasShadow = false
                };

                var categoryButton = new Button
                {
                    Text = category.Name,
                    FontSize = 14,
                    BackgroundColor = Colors.Transparent,
                    BorderWidth = 0,
                    TextColor = category.Name == "Барлығы" ? Color.FromArgb("#388E3C") : Colors.Black,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0)
                };

                categoryButton.Clicked += (sender, e) =>
                {
                    OnCategorySelected(category.Name);
                };

                frame.Content = categoryButton;

                frame.BindingContext = category;

                categoriesContainer.Children.Add(frame);
            }
            _selectedCategories.Add("Все");
        }

        private void OnCategorySelected(string categoryName)
        {
            if (categoryName == "Барлығы")
            {
                _selectedCategories.Clear();
                _selectedCategories.Add("Барлығы");

                foreach (var child in categoriesContainer.Children)
                {
                    if (child is Frame frame)
                    {
                        var category = frame.BindingContext as Category;
                        frame.BackgroundColor = category.Name == "Барлығы" ? Color.FromArgb("#E8F5E9") : Colors.White;

                        if (frame.Content is Button button)
                        {
                            button.TextColor = category.Name == "Барлығы" ? Color.FromArgb("#388E3C") : Colors.Black;
                        }
                    }
                }
            }
            else
            {
                if (_selectedCategories.Contains("Барлығы"))
                {
                    _selectedCategories.Remove("Барлығы");

                    foreach (var child in categoriesContainer.Children)
                    {
                        if (child is Frame frame && frame.BindingContext is Category category && category.Name == "Барлығы")
                        {
                            frame.BackgroundColor = Colors.White;
                            if (frame.Content is Button button)
                            {
                                button.TextColor = Colors.Black;
                            }
                            break;
                        }
                    }
                }

                if (_selectedCategories.Contains(categoryName))
                {
                    _selectedCategories.Remove(categoryName);
                }
                else
                {
                    _selectedCategories.Add(categoryName);
                }

                if (_selectedCategories.Count == 0)
                {
                    _selectedCategories.Add("Барлығы");

                    foreach (var child in categoriesContainer.Children)
                    {
                        if (child is Frame frame && frame.BindingContext is Category category && category.Name == "Барлығы")
                        {
                            frame.BackgroundColor = Color.FromArgb("#E8F5E9");
                            if (frame.Content is Button button)
                            {
                                button.TextColor = Color.FromArgb("#388E3C");
                            }
                            break;
                        }
                    }
                }

                foreach (var child in categoriesContainer.Children)
                {
                    if (child is Frame frame && frame.BindingContext is Category category && category.Name != "Барлығы")
                    {
                        bool isSelected = _selectedCategories.Contains(category.Name);
                        frame.BackgroundColor = isSelected ? Color.FromArgb("#E8F5E9") : Colors.White;

                        if (frame.Content is Button button)
                        {
                            button.TextColor = isSelected ? Color.FromArgb("#388E3C") : Colors.Black;
                        }
                    }
                }
            }

            ApplyFilters();
        }

        private async void LoadProducts()
        {
            _allProducts = await _databaseService.GetProductsAsync();
            ApplyFilters();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _currentSearchText = e.NewTextValue?.Trim() ?? string.Empty;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allProducts == null)
            {
                SearchResults.Clear();
                resultsCountLabel.Text = "Табылған тауарлар: 0";
                return;
            }

            var filteredProducts = _allProducts;

            if (!string.IsNullOrEmpty(_currentSearchText))
            {
                filteredProducts = filteredProducts
                    .Where(p =>
                        (p.Name?.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Description?.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase) == true))
                    .ToList();
            }
            if (!_selectedCategories.Contains("Барлығы"))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Category != null && _selectedCategories.Contains(p.Category))
                    .ToList();
            }

            SearchResults.Clear();

            if (_authService != null && _databaseService != null)
            {
                var userId = _authService.GetCurrentUserId();
                var userRole = _authService.GetCurrentUserRole();

                Task.Run(async () => {
                    try
                    {
                        var cartItems = await _databaseService.GetCartItemsAsync(userId);
                        var cartProductIds = cartItems.Select(ci => ci.ProductId).ToHashSet();

                        foreach (var product in filteredProducts)
                        {
                            if (userRole.Equals("client", StringComparison.OrdinalIgnoreCase))
                            {
                                product.IsFavoriteButtonVisible = true;
                                product.IsCartButtonVisible = true;

                                var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, product.Id);
                                product.FavoriteIcon = isFavorite ? "favourite_green.png" : "favourite_grey.png";

                                product.CartIcon = cartProductIds.Contains(product.Id) ? "basket_green.png" : "basket_grey.png";
                            }
                            else
                            {
                                product.IsFavoriteButtonVisible = false;
                                product.IsCartButtonVisible = false;
                            }

                            MainThread.BeginInvokeOnMainThread(() => {
                                SearchResults.Add(product);
                            });
                        }

                        MainThread.BeginInvokeOnMainThread(() => {
                            resultsCountLabel.Text = $"Табылған тауарлар: {SearchResults.Count}";
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Фильтр қолдану қатесі: {ex.Message}");
                        MainThread.BeginInvokeOnMainThread(() => {
                            resultsCountLabel.Text = "Іздеу кезінде қате орын алды";
                        });
                    }
                });
            }
            else
            {
                foreach (var product in filteredProducts)
                {
                    SearchResults.Add(product);
                }

                resultsCountLabel.Text = $"Табылған тауарлар: {SearchResults.Count}";
            }
        }


        private async void OnProductTapped(object sender, TappedEventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is Product product && _databaseService != null && _authService != null)
            {
                await Navigation.PushAsync(new ProductDetail(product, _databaseService, _authService));
            }
        }

        private async void OnLikeClicked(object sender, EventArgs e)
        {
            if (_databaseService == null || _authService == null) return;

            if (sender is ImageButton button && button.BindingContext is Product product)
            {
                var userId = _authService.GetCurrentUserId();
                var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, product.Id);

                if (isFavorite)
                {
                    await _databaseService.RemoveFavoriteProductAsync(userId, product.Id);
                    product.FavoriteIcon = "favourite_grey.png";
                    await DisplayAlert("Таңдаулылар", "Тауар таңдаулылардан жойылды", "Ок");
                }
                else
                {
                    await _databaseService.AddFavoriteProductAsync(userId, product.Id);
                    product.FavoriteIcon = "favourite_green.png";
                    await DisplayAlert("Таңдаулылар", "Тауар таңдаулыларыңызға қосылды", "Ок");
                }

                var index = SearchResults.IndexOf(product);
                if (index != -1)
                {
                    SearchResults[index] = product;
                }
            }
        }

        private async void OnAddToCartClicked(object sender, EventArgs e)
        {
            if (_databaseService == null || _authService == null) return;

            if (sender is ImageButton button && button.BindingContext is Product product)
            {
                var userId = _authService.GetCurrentUserId();
                var cartItems = await _databaseService.GetCartItemsAsync(userId);
                var isInCart = cartItems.Any(ci => ci.ProductId == product.Id);

                if (isInCart)
                {
                    await DisplayAlert("Себет", "Тауар қазірдің өзінде себетке салынған!", "Ок");
                }
                else
                {
                    await _databaseService.AddToCartAsync(userId, product.Id, 1);
                    product.CartIcon = "basket_green.png";
                    await DisplayAlert("Себет", "Тауар себетке қосылды!", "Ок");
                }

                var index = SearchResults.IndexOf(product);
                if (index != -1)
                {
                    SearchResults[index] = product;
                }
            }
        }
    }
}
