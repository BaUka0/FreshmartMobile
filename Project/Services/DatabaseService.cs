using Project.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class DatabaseService : IDisposable
    {
        private SQLiteAsyncConnection _database;
        private bool _disposed = false;

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

            await _database.CreateTableAsync<Review>();

            await _database.CreateTableAsync<Order>();
            await _database.CreateTableAsync<OrderItem>();
            await _database.CreateTableAsync<PaymentCard>();
        }

        // Helper method for password hashing
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Helper method to verify password
        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        public async Task<List<User>> GetUsersAsync() => await _database.Table<User>().ToListAsync();
        public async Task<User> GetUserAsync(int id) => await _database.Table<User>().Where(i => i.Id == id).FirstOrDefaultAsync();
        public async Task<User> GetUserByCredentialsAsync(string username, string password) 
        {
            var user = await _database.Table<User>().FirstOrDefaultAsync(u => u.username == username);
            if (user != null && VerifyPassword(password, user.password))
            {
                return user;
            }
            return null;
        }
        public async Task<User> GetUserByEmailAsync(string email, string password) 
        {
            var user = await _database.Table<User>().FirstOrDefaultAsync(u => u.email == email);
            if (user != null && VerifyPassword(password, user.password))
            {
                return user;
            }
            return null;
        }
        public async Task<int> AddUserAsync(User user) 
        {
            user.password = HashPassword(user.password);
            return await _database.InsertAsync(user);
        }
        public async Task<int> UpdateUserAsync(User user) => await _database.UpdateAsync(user);
        public async Task<int> DeleteUserAsync(User user) => await _database.DeleteAsync(user);

        // Method for updating user password (with hashing)
        public async Task<int> UpdateUserPasswordAsync(User user, string newPassword)
        {
            user.password = HashPassword(newPassword);
            return await _database.UpdateAsync(user);
        }

        // Helper method to check if username exists (for registration)
        public async Task<bool> UsernameExistsAsync(string username)
        {
            var user = await _database.Table<User>().FirstOrDefaultAsync(u => u.username == username);
            return user != null;
        }

        // Helper method to check if email exists (for registration)
        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _database.Table<User>().FirstOrDefaultAsync(u => u.email == email);
            return user != null;
        }


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
        public async Task<List<Product>> GetPopularProductsAsync(int limit = 10)
        {
            var allFavorites = await _database.Table<FavoriteProduct>().ToListAsync();

            var popularProductIds = allFavorites
                .GroupBy(f => f.ProductId)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(limit)
                .ToList();

            var popularProducts = new List<Product>();

            foreach (var item in popularProductIds)
            {
                var product = await GetProductAsync(item.ProductId);
                if (product != null)
                {
                    popularProducts.Add(product);
                }
            }

            if (popularProducts.Count == 0)
            {
                popularProducts = await _database.Table<Product>()
                    .Take(limit)
                    .ToListAsync();
            }

            return popularProducts;
        }


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

        //отзывы
        public async Task<int> AddReviewAsync(Review review)
        {
            review.CreatedAt = DateTime.Now;
            return await _database.InsertAsync(review);
        }
        public async Task<List<Review>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _database.Table<Review>()
                                        .Where(r => r.ProductId == productId)
                                        .OrderByDescending(r => r.CreatedAt)
                                        .ToListAsync();

            foreach (var review in reviews)
            {
                var user = await GetUserAsync(review.UserId);
                if (user != null)
                {
                    review.UserName = user.username;
                }
            }

            return reviews;
        }
        public async Task<List<Review>> GetUserReviewsAsync(int userId)
        {
            var reviews = await _database.Table<Review>()
                                        .Where(r => r.UserId == userId)
                                        .OrderByDescending(r => r.CreatedAt)
                                        .ToListAsync();

            foreach (var review in reviews)
            {
                var product = await GetProductAsync(review.ProductId);
                if (product != null)
                {
                    review.UserName = product.Name;
                }
            }

            return reviews;
        }

        //Заказы
        public async Task<int> CreateOrderAsync(Order order)
        {
            try
            {
                // Use transaction to ensure data consistency
                return await _database.RunInTransactionAsync(db =>
                {
                    order.OrderDate = DateTime.Now;
                    order.OrderStatus = "Обработка";
                    var orderId = db.Insert(order);

                    if (order.Items != null && order.Items.Count > 0)
                    {
                        foreach (var item in order.Items)
                        {
                            item.OrderId = order.Id;
                            db.Insert(item);
                        }
                    }

                    return orderId;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating order: {ex.Message}");
                throw; // Re-throw to let caller handle
            }
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            var orders = await _database.Table<Order>()
                                      .Where(o => o.UserId == userId)
                                      .OrderByDescending(o => o.OrderDate)
                                      .ToListAsync();

            foreach (var order in orders)
            {
                order.Items = await _database.Table<OrderItem>()
                                           .Where(oi => oi.OrderId == order.Id)
                                           .ToListAsync();

                foreach (var item in order.Items)
                {
                    var product = await GetProductAsync(item.ProductId);
                    if (product != null)
                    {
                        item.ProductImageData = product.ImageData;
                    }
                }
            }

            return orders;
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            var order = await _database.Table<Order>()
                                     .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                order.Items = await _database.Table<OrderItem>()
                                           .Where(oi => oi.OrderId == order.Id)
                                           .ToListAsync();

                foreach (var item in order.Items)
                {
                    var product = await GetProductAsync(item.ProductId);
                    if (product != null)
                    {
                        item.ProductImageData = product.ImageData;
                    }
                }
            }

            return order;
        }

        public async Task<int> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _database.Table<Order>()
                                     .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                order.OrderStatus = status;
                return await _database.UpdateAsync(order);
            }

            return 0;
        }


        // Оплата
        public async Task<List<PaymentCard>> GetPaymentCardsAsync(int userId)
        {
            return await _database.Table<PaymentCard>()
                                .Where(c => c.UserId == userId)
                                .ToListAsync();
        }

        public async Task<PaymentCard> GetDefaultPaymentCardAsync(int userId)
        {
            return await _database.Table<PaymentCard>()
                                .Where(c => c.UserId == userId && c.IsDefault)
                                .FirstOrDefaultAsync();
        }

        public async Task<int> AddPaymentCardAsync(PaymentCard card)
        {
            try
            {
                if (card.IsDefault)
                {
                    // Use transaction to ensure consistency when setting default card
                    return await _database.RunInTransactionAsync(db =>
                    {
                        // Remove default flag from other cards
                        var userCards = db.Table<PaymentCard>().Where(c => c.UserId == card.UserId).ToList();
                        foreach (var userCard in userCards)
                        {
                            if (userCard.IsDefault)
                            {
                                userCard.IsDefault = false;
                                db.Update(userCard);
                            }
                        }

                        // Insert new card
                        return db.Insert(card);
                    });
                }

                return await _database.InsertAsync(card);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding payment card: {ex.Message}");
                throw;
            }
        }

        public async Task<int> UpdatePaymentCardAsync(PaymentCard card)
        {
            try
            {
                if (card.IsDefault)
                {
                    // Use transaction to ensure consistency when setting default card
                    return await _database.RunInTransactionAsync(db =>
                    {
                        // Remove default flag from other cards
                        var userCards = db.Table<PaymentCard>().Where(c => c.UserId == card.UserId).ToList();
                        foreach (var userCard in userCards)
                        {
                            if (userCard.Id != card.Id && userCard.IsDefault)
                            {
                                userCard.IsDefault = false;
                                db.Update(userCard);
                            }
                        }

                        // Update the card
                        return db.Update(card);
                    });
                }

                return await _database.UpdateAsync(card);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating payment card: {ex.Message}");
                throw;
            }
        }

        public async Task<int> DeletePaymentCardAsync(int cardId)
        {
            var card = await _database.Table<PaymentCard>()
                                    .FirstOrDefaultAsync(c => c.Id == cardId);
            if (card != null)
            {
                return await _database.DeleteAsync(card);
            }
            return 0;
        }

        // IDisposable implementation for proper resource cleanup
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _database?.CloseAsync()?.Wait();
                }
                _disposed = true;
            }
        }
    }
}