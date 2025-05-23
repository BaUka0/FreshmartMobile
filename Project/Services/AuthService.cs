﻿using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class AuthService
    {
        private readonly DatabaseService _databaseService;
        private User _currentUser;

        public User CurrentUser => _currentUser;

        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> LoginAsync(string username, string password)
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
        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _databaseService.GetUserByCredentialsAsync(username, password);

            if (existingUser == null)
            {
                existingUser = await _databaseService.GetUserByEmailAsync(email, password);

                if (existingUser != null)
                {
                    return false;
                }
                var newUser = new User
                {
                    username = username,
                    password = password,
                    email = email,
                    role = "client"
                };
                await _databaseService.AddUserAsync(newUser);
                return true;
            }

            return false;
        }
        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            if (_currentUser == null)
            {
                return false;
            }

            if (_currentUser.password != oldPassword)
            {
                return false;
            }

            _currentUser.password = newPassword;
            await _databaseService.UpdateUserAsync(_currentUser);

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
