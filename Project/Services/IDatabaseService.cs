using Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Services
{
    public interface IDatabaseService : IDisposable
    {
        Task InitAsync();
        
        // User operations
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync(int id);
        Task<User> GetUserByCredentialsAsync(string username, string password);
        Task<User> GetUserByEmailAsync(string email, string password);
        Task<int> AddUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
        Task<int> UpdateUserPasswordAsync(User user, string newPassword);
        Task<int> DeleteUserAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        
        // Product operations
        Task<int> CreateProductAsync(Product product);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductAsync(Product product);
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task<List<Product>> GetProductsByCategoryAsync(string category);
        Task<List<Product>> GetPopularProductsAsync(int limit = 10);
        
        // Order operations
        Task<int> CreateOrderAsync(Order order);
        Task<List<Order>> GetUserOrdersAsync(int userId);
        Task<Order> GetOrderAsync(int orderId);
        Task<int> UpdateOrderStatusAsync(int orderId, string status);
        
        // Cart operations
        Task AddToCartAsync(int userId, int productId, int quantity = 1);
        Task<List<CartItem>> GetCartItemsAsync(int userId);
        Task RemoveCartItemAsync(int cartItemId);
        Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity);
        Task ClearCartAsync(int userId);
        
        // Favorite operations
        Task<int> AddFavoriteProductAsync(int userId, int productId);
        Task<int> RemoveFavoriteProductAsync(int userId, int productId);
        Task<bool> IsProductFavoriteAsync(int userId, int productId);
        Task<List<Product>> GetFavoriteProductsAsync(int userId);
        
        // Review operations
        Task<int> AddReviewAsync(Review review);
        Task<List<Review>> GetProductReviewsAsync(int productId);
        Task<List<Review>> GetUserReviewsAsync(int userId);
        
        // Payment card operations
        Task<List<PaymentCard>> GetPaymentCardsAsync(int userId);
        Task<PaymentCard> GetDefaultPaymentCardAsync(int userId);
        Task<int> AddPaymentCardAsync(PaymentCard card);
        Task<int> UpdatePaymentCardAsync(PaymentCard card);
        Task<int> DeletePaymentCardAsync(int cardId);
        
        // Seller application operations
        Task<List<SellerApplication>> GetSellerApplicationsAsync();
        Task<int> UpdateSellerApplicationAsync(SellerApplication application);
        Task<List<SellerApplication>> GetSellerApplicationsWithUsersAsync(string statusFilter = null);
        Task<List<SellerApplication>> GetPendingSellerApplicationsWithUsersAsync();
        Task<int> CreateSellerApplicationAsync(SellerApplication application);
    }
}