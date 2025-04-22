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
            if(_database != null)
                return;

            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db");
            Debug.WriteLine($"Database path: {dbPath}");

            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<User>();
        }

        public async Task<List<User>> GetUsersAsync() => await _database.Table<User>().ToListAsync();
        public async Task<User> GetUserAsync(int id) => await _database.Table<User>().Where(i => i.Id == id).FirstOrDefaultAsync();
        public async Task<User> GetUserByCredentialsAsync(string username, string password) => await _database.Table<User>().FirstOrDefaultAsync(u => u.username == username && u.password == password);
        public async Task<int> AddUserAsync(User user) => await _database.InsertAsync(user);
        public async Task<int> ChangeUserRoleAsync(int id, string newRole)
        {
            var user = await GetUserAsync(id);
            if (user != null)
            {
                user.role = newRole;
                return await _database.UpdateAsync(user);
            }
            return 0;
        }
    }
}
