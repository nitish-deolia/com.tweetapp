using com.tweetapp.Models;
using com.tweetapp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.BusinessLayer.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResponse> RegisterUser(RegisterRequest registerRequest);
        Task<LoginResponse> Login(string username, string password);
        Task<ForgotResponse> ForgotPassword(string username,string newPassword,string confirmNewPassword);
        Task<List<UserModel>> AllUsers();
        Task<List<UserModel>> SearchUser(string username);
        Task<LoginResponse> VerifyAndGenerateToken(TokenRequest tokenRequest);
    }
}
