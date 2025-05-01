using Project.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public async Task InitAsync()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db");
            Debug.WriteLine($"Database path: {dbPath}");

            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<SellerApplication>();

            await _database.CreateTableAsync<Product>();
            await _database.CreateTableAsync<FavoriteProduct>();
            await _database.CreateTableAsync<CartItem>();
        }

        public async Task<List<User>> GetUsersAsync() => await _database.Table<User>().ToListAsync();
        public async Task<User> GetUserAsync(int id) => await _database.Table<User>().Where(i => i.Id == id).FirstOrDefaultAsync();
        public async Task<User> GetUserByCredentialsAsync(string username, string password) => await _database.Table<User>().FirstOrDefaultAsync(u => u.username == username && u.password == password);
        public async Task<User> GetUserByEmailAsync(string email, string password) => await _database.Table<User>().FirstOrDefaultAsync(u => u.email == email && u.password == password);
        public async Task<int> AddUserAsync(User user) => await _database.InsertAsync(user);
        public async Task<int> UpdateUserAsync(User user) => await _database.UpdateAsync(user);
        public async Task<int> DeleteUserAsync(User user) => await _database.DeleteAsync(user);


        //заявки
        public async Task<List<SellerApplication>> GetSellerApplicationsAsync() => await _database.Table<SellerApplication>().ToListAsync();
        public async Task<int> UpdateSellerApplicationAsync(SellerApplication application) => await _database.UpdateAsync(application);
        public async Task<List<SellerApplication>> GetSellerApplicationsWithUsersAsync(string statusFilter = null)
        {
            List<SellerApplication> applications;

            if (string.IsNullOrEmpty(statusFilter))
            {
                applications = await _database.Table<SellerApplication>().ToListAsync();
            }
            else
            {
                applications = await _database.Table<SellerApplication>()
                                                .Where(app => app.Status == statusFilter)
                                                .ToListAsync();
            }
            foreach (var app in applications)
            {
                app.User = await GetUserAsync(app.UserId);
            }
            return applications.Where(app => app.User != null).ToList();
        }
        public Task<List<SellerApplication>> GetPendingSellerApplicationsWithUsersAsync() => GetSellerApplicationsWithUsersAsync("Pending");
        public Task<int> CreateSellerApplicationAsync(SellerApplication application) => _database.InsertAsync(application);


        // Методы для работы с товаром
        public Task<int> CreateProductAsync(Product product) => _database.InsertAsync(product);
        public Task<int> UpdateProductAsync(Product product) => _database.UpdateAsync(product);
        public Task<int> DeleteProductAsync(Product product) => _database.DeleteAsync(product);
        public Task<List<Product>> GetProductsAsync() => _database.Table<Product>().ToListAsync();
        public Task<Product> GetProductAsync(int id) => _database.Table<Product>().FirstOrDefaultAsync(p => p.Id == id);
        public Task<List<Product>> GetProductsByCategoryAsync(string category) => _database.Table<Product>().Where(p => p.Category == category).ToListAsync();

        // Избранное
        public async Task<int> AddFavoriteProductAsync(int userId, int productId)
        {
            var favorite = new FavoriteProduct { UserId = userId, ProductId = productId };
            return await _database.InsertAsync(favorite);
        }

        public async Task<int> RemoveFavoriteProductAsync(int userId, int productId)
        {
            var favorite = await _database.Table<FavoriteProduct>()
                                           .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
            if (favorite != null)
            {
                return await _database.DeleteAsync(favorite);
            }
            return 0;
        }

        public async Task<bool> IsProductFavoriteAsync(int userId, int productId)
        {
            var favorite = await _database.Table<FavoriteProduct>()
                                           .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
            return favorite != null;
        }
        public async Task<List<Product>> GetFavoriteProductsAsync(int userId)
        {
            var favoriteProducts = new List<Product>();

            var favoriteProductIds = await _database.Table<FavoriteProduct>()
                                                    .Where(f => f.UserId == userId)
                                                    .ToListAsync();

            foreach (var favorite in favoriteProductIds)
            {
                var product = await GetProductAsync(favorite.ProductId);
                if (product != null)
                {
                    favoriteProducts.Add(product);
                }
            }

            return favoriteProducts;
        }

        // корзина
        public async Task AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            var existingCartItem = await _database.Table<CartItem>()
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                await _database.UpdateAsync(existingCartItem);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _database.InsertAsync(newCartItem);
            }
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            return await _database.Table<CartItem>()
                                  .Where(ci => ci.UserId == userId)
                                  .ToListAsync();
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var item = await _database.Table<CartItem>().FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            if (item != null)
                await _database.DeleteAsync(item);
        }

        public async Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity)
        {
            var item = await _database.Table<CartItem>()
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (item != null && newQuantity > 0)
            {
                item.Quantity = newQuantity;
                await _database.UpdateAsync(item);
            }
        }
        public async Task ClearCartAsync(int userId)
        {
            var items = await _database.Table<CartItem>()
                                       .Where(ci => ci.UserId == userId)
                                       .ToListAsync();
            foreach (var item in items)
            {
                await _database.DeleteAsync(item);
            }
        }


    }
}