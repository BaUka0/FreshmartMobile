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

            // ��������� ������ �� ���������� ������
            FilterProducts(searchText);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // ��������� ���������� ������ � �������� �������
            FilterProducts(e.NewTextValue);
        }

        private void FilterProducts(string searchText)
        {
            // ���������� ������� (�������� �� ��� �������� ������)
            var allProducts = GetAllProducts();
            var filteredProducts = allProducts.Where(p => p.Name.ToLower().Contains(searchText.ToLower())).ToList();

            // ��������� ������ �����������
            SearchResults.Clear();
            foreach (var product in filteredProducts)
            {
                SearchResults.Add(product);
            }
        }

        private List<Product> GetAllProducts()
        {
            // ������ � ���������� �������. �������� �� �������� ������ �� ������ ���������
            return new List<Product>
            {
                new Product { Name = "����� 1", Description = "�������� ������ 1", Price = "$10", ImageData = new byte[] { } },
                new Product { Name = "����� 2", Description = "�������� ������ 2", Price = "$20", ImageData = new byte[] { } },
                // �������� �������� ������
            };
        }
    }
}
