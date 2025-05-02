using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Project.Models;

namespace Project.Pages
{
    public partial class SearchPage : ContentPage
    {
        public ObservableCollection<Product> SearchResults { get; set; }

        public SearchPage(string searchText)
        {
            InitializeComponent();
            SearchResults = new ObservableCollection<Product>();
            BindingContext = this;

            // Фильтруем товары по введенному тексту
            FilterProducts(searchText);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // Обновляем результаты поиска в реальном времени
            FilterProducts(e.NewTextValue);
        }

        private void FilterProducts(string searchText)
        {
            // Фильтрация товаров (замените на ваш источник данных)
            var allProducts = GetAllProducts();
            var filteredProducts = allProducts.Where(p => p.Name.ToLower().Contains(searchText.ToLower())).ToList();

            // Обновляем список результатов
            SearchResults.Clear();
            foreach (var product in filteredProducts)
            {
                SearchResults.Add(product);
            }
        }

        private List<Product> GetAllProducts()
        {
            // Пример с фиктивными данными. Замените на реальные данные из вашего источника
            return new List<Product>
            {
                new Product { Name = "Товар 1", Description = "Описание товара 1", Price = "$10", ImageData = new byte[] { } },
                new Product { Name = "Товар 2", Description = "Описание товара 2", Price = "$20", ImageData = new byte[] { } },
                // Добавьте реальные товары
            };
        }
    }
}
