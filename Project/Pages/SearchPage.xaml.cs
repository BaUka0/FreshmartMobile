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
                new Category { Name = "Все", Icon = "all_icon.png" },
                new Category { Name = "Напитки", Icon = "drink_icon.png" },
                new Category { Name = "Фрукты", Icon = "fruit_icon.png" },
                new Category { Name = "Овощи", Icon = "vegetables_icon.png" },
                new Category { Name = "Мясо", Icon = "meat_icon.png" },
                new Category { Name = "Гастрономия", Icon = "gastronomy.png" },
                new Category { Name = "Молочные продукты", Icon = "dairy_icon.png" },
                new Category { Name = "Бакалея", Icon = "grocery.png" },
                new Category { Name = "Хлеб и выпечка", Icon = "bread_icon.png" },
                new Category { Name = "Чай, кофе, какао", Icon = "tea.png" },
                new Category { Name = "Конфеты и сладости", Icon = "candies_icon.png" },
                new Category { Name = "Собственное производство", Icon = "cake.png" },
                new Category { Name = "Консервы", Icon = "canned.png" },
                new Category { Name = "Замороженные продукты", Icon = "frozen_icon.png" },
                new Category { Name = "Другие товары", Icon = "others.png" },
            };

            // Создаем кнопки для категорий
            foreach (var category in _categories)
            {
                var frame = new Frame
                {
                    Padding = new Thickness(12, 8),
                    CornerRadius = 20,
                    BorderColor = Colors.LightGray,
                    BackgroundColor = category.Name == "Все" ? Color.FromArgb("#E8F5E9") : Colors.White,
                    HasShadow = false
                };

                var categoryButton = new Button
                {
                    Text = category.Name,
                    FontSize = 14,
                    BackgroundColor = Colors.Transparent,
                    BorderWidth = 0,
                    TextColor = category.Name == "Все" ? Color.FromArgb("#388E3C") : Colors.Black,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0)
                };

                // Добавляем обработчик клика
                categoryButton.Clicked += (sender, e) =>
                {
                    OnCategorySelected(category.Name);
                };

                frame.Content = categoryButton;

                // Добавляем данные категории в свойство BindingContext фрейма
                frame.BindingContext = category;

                // Добавляем на экран
                categoriesContainer.Children.Add(frame);
            }

            // По умолчанию выбираем "Все"
            _selectedCategories.Add("Все");
        }

        private void OnCategorySelected(string categoryName)
        {
            if (categoryName == "Все")
            {
                // Если выбрали "Все", сбрасываем все остальные выбранные категории
                _selectedCategories.Clear();
                _selectedCategories.Add("Все");

                // Обновляем визуальное состояние кнопок
                foreach (var child in categoriesContainer.Children)
                {
                    if (child is Frame frame)
                    {
                        var category = frame.BindingContext as Category;
                        frame.BackgroundColor = category.Name == "Все" ? Color.FromArgb("#E8F5E9") : Colors.White;

                        if (frame.Content is Button button)
                        {
                            button.TextColor = category.Name == "Все" ? Color.FromArgb("#388E3C") : Colors.Black;
                        }
                    }
                }
            }
            else
            {
                // Если выбрали конкретную категорию

                // Удаляем "Все" из выбранных категорий, если она там есть
                if (_selectedCategories.Contains("Все"))
                {
                    _selectedCategories.Remove("Все");

                    // Сбрасываем визуальное состояние кнопки "Все"
                    foreach (var child in categoriesContainer.Children)
                    {
                        if (child is Frame frame && frame.BindingContext is Category category && category.Name == "Все")
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

                // Инвертируем состояние выбранной категории
                if (_selectedCategories.Contains(categoryName))
                {
                    _selectedCategories.Remove(categoryName);
                }
                else
                {
                    _selectedCategories.Add(categoryName);
                }

                // Если не осталось выбранных категорий, выбираем "Все"
                if (_selectedCategories.Count == 0)
                {
                    _selectedCategories.Add("Все");

                    // Обновляем визуальное состояние кнопки "Все"
                    foreach (var child in categoriesContainer.Children)
                    {
                        if (child is Frame frame && frame.BindingContext is Category category && category.Name == "Все")
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

                // Обновляем визуальное состояние выбранных/не выбранных категорий
                foreach (var child in categoriesContainer.Children)
                {
                    if (child is Frame frame && frame.BindingContext is Category category && category.Name != "Все")
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

            // Обновляем результаты поиска с учетом выбранных категорий
            ApplyFilters();
        }

        private async void LoadProducts()
        {
            if (_databaseService != null)
            {
                // Загружаем все продукты из базы данных
                _allProducts = await _databaseService.GetProductsAsync();

                // Применяем фильтры к загруженным продуктам
                ApplyFilters();
            }
            else
            {
                // Если сервис не предоставлен, используем демо-данные
                _allProducts = new List<Product>
                {
                    new Product { Name = "Молоко", Description = "Свежее молоко 2.5%", Price = "450 ₸", Category = "Молочные продукты", ImageData = new byte[] { } },
                    new Product { Name = "Хлеб", Description = "Белый хлеб", Price = "180 ₸", Category = "Хлеб и выпечка", ImageData = new byte[] { } },
                    new Product { Name = "Яблоки", Description = "Свежие яблоки", Price = "550 ₸", Category = "Фрукты", ImageData = new byte[] { } },
                    new Product { Name = "Кока-кола", Description = "Газированный напиток 1.5л", Price = "650 ₸", Category = "Напитки", ImageData = new byte[] { } },
                    new Product { Name = "Сыр", Description = "Сыр Российский 45%", Price = "1800 ₸", Category = "Молочные продукты", ImageData = new byte[] { } },
                };

                // Применяем фильтры к демо-данным
                ApplyFilters();
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _currentSearchText = e.NewTextValue?.Trim() ?? string.Empty;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            // Проверяем, что источник данных не null
            if (_allProducts == null)
            {
                // Если продукты не загружены, обновляем интерфейс с пустыми результатами
                SearchResults.Clear();
                resultsCountLabel.Text = "Найдено товаров: 0";
                return;
            }

            var filteredProducts = _allProducts;

            // Фильтрация по поисковому запросу
            if (!string.IsNullOrEmpty(_currentSearchText))
            {
                filteredProducts = filteredProducts
                    .Where(p =>
                        (p.Name?.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.Description?.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase) == true))
                    .ToList();
            }

            // Фильтрация по категориям
            if (!_selectedCategories.Contains("Все"))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Category != null && _selectedCategories.Contains(p.Category))
                    .ToList();
            }

            // Обновляем результаты поиска
            SearchResults.Clear();

            if (_authService != null && _databaseService != null)
            {
                // Получаем данные пользователя для отображения избранного и корзины
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

                                // Проверяем избранное
                                var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, product.Id);
                                product.FavoriteIcon = isFavorite ? "favourite_green.png" : "favourite_grey.png";

                                // Проверяем корзину
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
                            // Обновляем счетчик результатов
                            resultsCountLabel.Text = $"Найдено товаров: {SearchResults.Count}";
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при применении фильтров: {ex.Message}");
                        MainThread.BeginInvokeOnMainThread(() => {
                            resultsCountLabel.Text = "Произошла ошибка при поиске";
                        });
                    }
                });
            }
            else
            {
                // Если сервисы не предоставлены, просто добавляем продукты без обработки
                foreach (var product in filteredProducts)
                {
                    SearchResults.Add(product);
                }

                // Обновляем счетчик результатов
                resultsCountLabel.Text = $"Найдено товаров: {SearchResults.Count}";
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
                    await DisplayAlert("Избранное", "Товар удален из избранного", "Ок");
                }
                else
                {
                    await _databaseService.AddFavoriteProductAsync(userId, product.Id);
                    product.FavoriteIcon = "favourite_green.png";
                    await DisplayAlert("Избранное", "Товар добавлен в избранное", "Ок");
                }

                // Обновляем UI
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
                    await DisplayAlert("Корзина", "Товар уже в корзине", "Ок");
                }
                else
                {
                    await _databaseService.AddToCartAsync(userId, product.Id, 1);
                    product.CartIcon = "basket_green.png";
                    await DisplayAlert("Корзина", "Товар добавлен!", "Ок");
                }

                // Обновляем UI
                var index = SearchResults.IndexOf(product);
                if (index != -1)
                {
                    SearchResults[index] = product;
                }
            }
        }
    }
}
