using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDatabaseService _databaseService;
        private User _currentUser;

        public User CurrentUser => _currentUser;

        public AuthService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _databaseService.GetUserByCredentialsAsync(username, password);

                if (user == null)
                {
                    user = await _databaseService.GetUserByEmailAsync(username, password);
                }

                if (user != null)
                {
                    _currentUser = user;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log error but don't expose details to user
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return false;
                }

                // Password strength validation (minimum 6 characters)
                if (password.Length < 6)
                {
                    return false;
                }

                // Check if username already exists
                if (await _databaseService.UsernameExistsAsync(username))
                {
                    return false; // Username already exists
                }

                // Check if email already exists
                if (await _databaseService.EmailExistsAsync(email))
                {
                    return false; // Email already exists
                }

                var newUser = new User
                {
                    username = username,
                    password = password, // Will be hashed in AddUserAsync
                    email = email,
                    role = "client"
                };
                await _databaseService.AddUserAsync(newUser);
                return true;
            }
            catch (Exception ex)
            {
                // Log error but don't expose details to user
                System.Diagnostics.Debug.WriteLine($"Registration error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            if (_currentUser == null)
            {
                return false;
            }

            // Verify old password using the database service method  
            var verifiedUser = await _databaseService.GetUserByCredentialsAsync(_currentUser.username, oldPassword);
            if (verifiedUser == null)
            {
                return false;
            }

            // Update password using the dedicated method that handles hashing
            await _databaseService.UpdateUserPasswordAsync(_currentUser, newPassword);

            return true;
        }
        public async Task<bool> UpdateProfileImageAsync(byte[] imageData)
        {
            if (_currentUser == null)
            {
                return false;
            }

            _currentUser.ProfileImage = imageData;
            await _databaseService.UpdateUserAsync(_currentUser);

            return true;
        }

        public async Task<bool> UpdateProfileAsync(string username, string email)
        {
            if (_currentUser == null)
            {
                return false;
            }

            _currentUser.username = username;
            _currentUser.email = email;
            await _databaseService.UpdateUserAsync(_currentUser);

            return true;
        }
        public string GetCurrentUserRole()
        {
            if (_currentUser != null)
            {
                return _currentUser.role;
            }
            return "guest";
        } 
        public int GetCurrentUserId()
        {
            if (_currentUser != null)
            {
                return _currentUser.Id;
            }
            return 0;
        }
        public void Logout() => _currentUser = null;
    }
}
