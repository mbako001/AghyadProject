using AghyadProject.Models;

namespace AghyadProject.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<string>> Register(User user, string email, string password);
        Task<ServiceResponse<string>> Login(string username , string password);
        Task<bool> UserExists(string username);
        Task<ServiceResponse<string>> ForgotPassword(string email);
        Task<ServiceResponse<string>> ResetPassowrd(string Token , string password);
    }
}
