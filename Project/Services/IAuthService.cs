using Project.Models;
using System.Threading.Tasks;

namespace Project.Services
{
    public interface IAuthService
    {
        User CurrentUser { get; }
        Task<bool> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password);
        Task<bool> ChangePasswordAsync(string oldPassword, string newPassword);
        Task<bool> UpdateProfileImageAsync(byte[] imageData);
        Task<bool> UpdateProfileAsync(string username, string email);
        string GetCurrentUserRole();
        int GetCurrentUserId();
        void Logout();
    }
}