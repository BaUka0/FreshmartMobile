using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Project.Pages;

public partial class HomePage : ContentPage, INotifyPropertyChanged
{
    private DatabaseService _databaseService;
    private AuthService _authService;

    private ObservableCollection<string> _carouselItems;
    public ObservableCollection<string> CarouselItems
    {
        get => _carouselItems;
        set => SetProperty(ref _carouselItems, value);
    }

    private ObservableCollection<Product> _popularProducts;
    public ObservableCollection<Product> PopularProducts
    {
        get => _popularProducts;
        set => SetProperty(ref _popularProducts, value);
    }

    private ObservableCollection<Product> _searchSuggestions;
    public ObservableCollection<Product> SearchSuggestions
    {
        get => _searchSuggestions;
        set => SetProperty(ref _searchSuggestions, value);
    }

    private double _suggestionsHeight = 0;
    public double SuggestionsHeight
    {
        get => _suggestionsHeight;
        set => SetProperty(ref _suggestionsHeight, value);
    }

    // Задержка поиска для улучшения производительности
    private CancellationTokenSource _throttleCts;
    // Флаг для отслеживания состояния поиска
    private bool _isSearchActive = false;

    public HomePage(DatabaseService databaseService, AuthService authService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _authService = authService;

        CarouselItems = new ObservableCollection<string>
        {
            "carusel_one.png",
            "carusel_two.png",
            "carusel_three.png"
        };

        PopularProducts = new ObservableCollection<Product>();
        SearchSuggestions = new ObservableCollection<Product>();

        BindingContext = this;

        LoadMockProducts();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadMockProducts();
        _isSearchActive = false;
    }

    private async void LoadMockProducts()
    {
        PopularProducts.Clear();

        var popularProducts = await _databaseService.GetPopularProductsAsync(6);

        var userId = _authService.GetCurrentUserId();
        var userRole = _authService.GetCurrentUserRole();

        var cartItems = await _databaseService.GetCartItemsAsync(userId);
        var cartProductIds = cartItems.Select(ci => ci.ProductId).ToHashSet();

        foreach (var product in popularProducts)
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

            PopularProducts.Add(product);
        }
    }

    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Product tappedProduct)
        {
            await Navigation.PushAsync(new ProductDetail(tappedProduct, _databaseService, _authService));
        }
    }

    private async void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Product product)
        {
            var userId = _authService.GetCurrentUserId();
            var isFavorite = await _databaseService.IsProductFavoriteAsync(userId, product.Id);

            if (isFavorite)
            {
                await _databaseService.RemoveFavoriteProductAsync(userId, product.Id);
                product.FavoriteIcon = "favourite_grey.png";
                await DisplayAlert("Таңдаулы", "Тауар таңдаулылардан жойылды", "Ок");
            }
            else
            {
                await _databaseService.AddFavoriteProductAsync(userId, product.Id);
                product.FavoriteIcon = "favourite_green.png";
                await DisplayAlert("Таңдаулы", "Тауар таңдаулыларыңызға қосылды", "Ок");
            }

            RefreshProducts();
        }
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Product product)
        {
            var userId = _authService.GetCurrentUserId();
            var cartItems = await _databaseService.GetCartItemsAsync(userId);
            var isInCart = cartItems.Any(ci => ci.ProductId == product.Id);

            if (isInCart)
            {
                await DisplayAlert("Себет", "Тауар қазірдің өзінде себетке салынған", "Ок");
            }
            else
            {
                await _databaseService.AddToCartAsync(userId, product.Id, 1);
                product.CartIcon = "basket_green.png";
                await DisplayAlert("Себет", "Тауар қосылды!", "Ок");
            }

            RefreshProducts();
        }
    }

    private void RefreshProducts()
    {
        PopularProducts = new ObservableCollection<Product>(PopularProducts);
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.Trim();

        // Если в процессе фокусировки, пропускаем обработку
        if (_isSearchActive)
            return;

        // Отмена предыдущего поиска, если он в процессе
        _throttleCts?.Cancel();
        _throttleCts = new CancellationTokenSource();

        if (string.IsNullOrEmpty(searchText))
        {
            suggestionsFrame.IsVisible = false;
            return;
        }

        try
        {
            // Добавляем задержку перед поиском для уменьшения количества запросов
            await Task.Delay(300, _throttleCts.Token);

            // Получаем список всех продуктов и фильтруем их
            var allProducts = await _databaseService.GetProductsAsync();
            var filteredProducts = allProducts
                .Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();

            SearchSuggestions.Clear();
            foreach (var product in filteredProducts)
            {
                SearchSuggestions.Add(product);
            }

            // Рассчитываем высоту списка подсказок
            // Высота одного элемента примерно 40 единиц (с учетом отступов)
            int itemHeight = 40;
            int maxItems = 5; // Максимальное количество элементов

            // Вычисляем высоту в зависимости от количества элементов
            SuggestionsHeight = Math.Min(SearchSuggestions.Count, maxItems) * itemHeight;

            // Минимальная высота, если есть хотя бы один элемент
            if (SearchSuggestions.Count > 0 && SuggestionsHeight < itemHeight)
                SuggestionsHeight = itemHeight;

            // Показываем результаты только если есть совпадения и текстовое поле не пустое
            suggestionsFrame.IsVisible = SearchSuggestions.Count > 0 && !string.IsNullOrEmpty(searchText);
        }
        catch (TaskCanceledException)
        {
            // Поиск был отменен, ничего не делаем
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Іздеу кезіндегі қате: {ex.Message}");
        }
    }

    private void OnSearchBarFocused(object sender, FocusEventArgs e)
    {
        // Просто показываем подсказки, если уже набран текст
        if (!string.IsNullOrEmpty(searchBar.Text))
        {
            suggestionsFrame.IsVisible = SearchSuggestions.Count > 0;
        }
    }

    private async void OnSearchBarSearchButtonPressed(object sender, EventArgs e)
    {
        try
        {
            // Устанавливаем флаг активного поиска
            _isSearchActive = true;

            // Получаем текст поиска
            string searchText = searchBar.Text?.Trim() ?? string.Empty;

            // Скрываем подсказки
            suggestionsFrame.IsVisible = false;

            // Очищаем поле поиска
            searchBar.Text = string.Empty;

            // Проверяем, что сервисы не null перед передачей
            if (_databaseService != null && _authService != null)
            {
                // Переходим на страницу поиска с параметрами
                await Navigation.PushAsync(new SearchPage(searchText, _databaseService, _authService));
            }
            else
            {
                // В случае отсутствия сервисов показываем сообщение
                await DisplayAlert("Қате", "Іздеуіңізді аяқтау мүмкін емес. Тағы жасауды сәл кейінірек көріңізді өтінеміз.", "ОК");
            }
        }
        catch (Exception ex)
        {
            // Обрабатываем возможные исключения
            Console.WriteLine($"Іздеу бетіне өту кезінде қате: {ex.Message}");
            await DisplayAlert("Қате", "Іздеу кезінде қате орын алды.", "ОК");
        }
        finally
        {
            // Сбрасываем флаг в любом случае
            _isSearchActive = false;
        }
    }

    private void OnSearchBarUnfocused(object sender, FocusEventArgs e)
    {
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(200), () =>
        {
            suggestionsFrame.IsVisible = false;
        });
    }

    private async void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
            {
                // Устанавливаем флаг активного поиска
                _isSearchActive = true;

                // Сбрасываем выделение
                suggestionsCollection.SelectedItem = null;

                // Скрываем подсказки
                suggestionsFrame.IsVisible = false;

                // Очищаем поисковую строку
                searchBar.Text = string.Empty;

                // Проверка, что сервисы не null
                if (_databaseService != null && _authService != null)
                {
                    // Переходим на страницу поиска с выбранным текстом
                    await Navigation.PushAsync(new SearchPage(selectedProduct.Name, _databaseService, _authService));
                }
                else
                {
                    // Показываем сообщение об ошибке
                    await DisplayAlert("Қате", "Іздеуді ашу мүмкін емес. Әрекетті кейінірек қайталаңыз.", "ОК");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Анықтаманы таңдау кезіндегі қате: {ex.Message}");
            await DisplayAlert("Қате", "Элементті таңдау кезінде қате орын алды.", "ОК");
        }
        finally
        {
            // Сбрасываем флаг в любом случае
            _isSearchActive = false;
        }
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion
}
