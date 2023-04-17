using com.tweetapp.Models;
using com.tweetapp.Models.Entities;

namespace com.tweetapp.DataManager.Interfaces
{
    public interface IUserDataManager
    {
        Task<RegisterResponse> RegisterUser(RegisterRequest registerRequest);
        Task<LoginResponse> Login(string username,string password);
        Task<ForgotResponse> ForgotPassword(string username, string newPassword, string confirmNewPassword);
        Task<List<UserModel>> AllUsers();
        Task<List<UserModel>> SearchUser(string username);
        Task<LoginResponse> VerifyAndGenerateToken(TokenRequest tokenRequest);
    }
}
