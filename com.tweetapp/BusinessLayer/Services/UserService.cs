using com.tweetapp.BusinessLayer.Interfaces;
using com.tweetapp.DataManager.Interfaces;
using com.tweetapp.Models;
using com.tweetapp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDataManager _userDataManager;
        public UserService(IUserDataManager userDataManager)
        {
            _userDataManager = userDataManager;
        }
        public async Task<List<UserModel>> AllUsers()
        {
            return await _userDataManager.AllUsers();
        }

        public async Task<ForgotResponse> ForgotPassword(string username, string newPassword, string confirmNewPassword)
        {
            return await _userDataManager.ForgotPassword(username, newPassword, confirmNewPassword);
        }

        public async Task<LoginResponse> Login(string username,string password)
        {
            return await _userDataManager.Login(username,password);
        }

        public async Task<RegisterResponse> RegisterUser(RegisterRequest registerRequest)
        {
            return await _userDataManager.RegisterUser(registerRequest);
        }

        public async Task<List<UserModel>> SearchUser(string username)
        {
            return await _userDataManager.SearchUser(username);
        }

        public async Task<LoginResponse> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            return await _userDataManager.VerifyAndGenerateToken(tokenRequest);
        }
    }
}
