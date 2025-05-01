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


    }
}