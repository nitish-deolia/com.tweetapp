using com.tweetapp.Configuration;
using com.tweetapp.DataManager.Interfaces;
using com.tweetapp.Models;
using com.tweetapp.Models.Context;
using com.tweetapp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Converters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace com.tweetapp.DataManager.Repository
{
    public class UserDataManager : IUserDataManager
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IOptions<AppsettingsConfig> _appSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public UserDataManager(IMongoDbContext mongoDbContext, IOptions<AppsettingsConfig> appSettings, TokenValidationParameters tokenValidationParameters) {
            _dbContext = mongoDbContext;
            _appSettings = appSettings;
            _tokenValidationParameters = tokenValidationParameters;
        }
        public async Task<RegisterResponse> RegisterUser(RegisterRequest registerRequest)
        {
            var users = _dbContext.Users<User>();
            User user;
            if (users != null)
            {
                var filter = Builders<User>.Filter.Eq(user => user.LoginId, registerRequest.LoginId) | Builders<User>.Filter.Eq(user => user.Email, registerRequest.Email);
                var user_exist = await users.Find(filter).FirstOrDefaultAsync();
                if (user_exist != null)
                {
                    return new RegisterResponse { Result = false, Errors = new List<string>() { "Invalid User Or Email Entered." } };
                }
                else
                {
                    if (registerRequest.Password == registerRequest.ConfirmPassword)
                    {
                        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
                        user = new()
                        {
                            FirstName = registerRequest.FirstName,
                            LastName = registerRequest.LastName,
                            Email = registerRequest.Email,
                            LoginId = registerRequest.LoginId,
                            PasswordHash = passwordHash,
                            PhoneNumber = registerRequest.PhoneNumber,
                            IsActive = true,
                            Tweets = new List<string>()
                        };
                        await users.InsertOneAsync(user);
                        return new RegisterResponse { Result = true };
                    }
                    else
                    {
                        return new RegisterResponse { Result = false, Errors = new List<string>() { "Password does not match." } };
                    }
                }
            }
            else
            {
                return new RegisterResponse { Result = false, Errors = new List<string>() { "Server Error" } };
            }
        }
        public async Task<LoginResponse> Login(string username, string password)
        {
            var users = _dbContext.Users<User>();
            var filter = Builders<User>.Filter.Eq(user => user.LoginId, username) & Builders<User>.Filter.Eq(x => x.IsActive, true);
            User user = await users.Find(filter).FirstOrDefaultAsync();
            if (user == null)
            {
                return new LoginResponse { Errors = new List<string> { "Invalid Request." } };
            }
            else
            {
                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return new LoginResponse { Errors = new List<string> { "Invalid Request." } };
                }
                else
                {
                    return await GenerateToken(user);
                }
            }
        }
        public async Task<List<UserModel>> AllUsers()
        {
            var userlist = new List<UserModel>();
            var filter = Builders<User>.Filter.Eq(x => x.IsActive,true);
            var Projection = Builders<User>.Projection.Expression(user => new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                LoginId = user.LoginId,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            });
            userlist = await _dbContext.Users<User>().Find(filter).Project(Projection).ToListAsync();

            return userlist;
        }
        public async Task<ForgotResponse> ForgotPassword(string username, string newPassword, string confirmNewPassword)
        {
            ForgotResponse result = new();
            var users = _dbContext.Users<User>();
            if(users != null)
            {
                var filter = Builders<User>.Filter.Eq(x=> x.LoginId,username) & Builders<User>.Filter.Eq(x => x.IsActive, true);
                var user_exist = users.Find(filter).FirstOrDefaultAsync();
                if(user_exist != null)
                {
                    if(newPassword == confirmNewPassword)
                    {
                        string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        var passUpdate = Builders<User>.Update.Set(x => x.PasswordHash, passwordHash);
                        await users.UpdateOneAsync(filter,passUpdate);
                        return result = new ForgotResponse { Success = true };
                    }
                    else
                    {
                        return result = new ForgotResponse { Success = false, Errors = new List<string>() { "Pasword does not match" } };
                    }
                }
                else
                {
                    return result = new ForgotResponse { Success = false, Errors = new List<string>() { "Invalid user" } };
                }
            }
            else
            {
                return result = new ForgotResponse { Success = false, Errors = new List<string>() { "Server Error" } };
            }
        }
        public async Task<List<UserModel>> SearchUser(string username) { 
            var userlist = new List<UserModel>();
            var filter = Builders<User>.Filter.Regex(x => x.LoginId, new BsonRegularExpression(Regex.Escape(username), "i")) & Builders<User>.Filter.Eq(x =>x.IsActive,true);
            var Projection = Builders<User>.Projection.Expression(user => new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                LoginId = user.LoginId,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            });
            userlist = await _dbContext.Users<User>().Find(filter).Project(Projection).ToListAsync();
            return userlist;
        }
        #region token logic
        public async Task<LoginResponse> GenerateToken(User user)
        {
            LoginResponse loginResponse;
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Value.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id",user.Id),
                        new Claim(ClaimTypes.Name,user.LoginId),
                        new Claim(JwtRegisteredClaimNames.Email,user.Email),
                        new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString())
                    }),
                    IssuedAt = DateTime.Now,
                    Expires = DateTime.UtcNow.Add(_appSettings.Value.ExpiryTime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)
                };

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = jwtTokenHandler.WriteToken(token);

                var refreshToken = new RefreshToken
                {
                    Token = RefreshTokenGenerator(32),//generate refresh token
                    AddedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false,
                    IsUsed = false,
                    UserId = user.LoginId,
                    JwtId = token.Id
                };

                var refreshTokenCollection = _dbContext.RefreshTokens<RefreshToken>();
                await refreshTokenCollection.InsertOneAsync(refreshToken);

                loginResponse = new LoginResponse
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.LoginId,
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token
                };
            }catch
            {
                throw;
            }

            return loginResponse;
        }
        private string RefreshTokenGenerator(int length)
        {
            var random = new byte[length];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        public async Task<LoginResponse> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false) return new LoginResponse { Errors = new List<string> { "Invalid Tokens." } };

                    var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                    var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                    var refreshTokens = _dbContext.RefreshTokens<RefreshToken>();
                    var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, tokenRequest.RefreshToken);
                    var refreshToken = await refreshTokens.Find(filter).FirstOrDefaultAsync();
                    if(refreshToken == null || refreshToken.IsUsed || refreshToken.IsRevoked || expiryDate > DateTime.UtcNow || refreshToken.JwtId != jti)
                    {
                        return new LoginResponse { Errors = new List<string> { "Invalid Tokens." } };
                    }

                    var update = Builders<RefreshToken>.Update.Set(x => x.IsUsed, true);
                    await refreshTokens.UpdateOneAsync(filter,update);

                    var users = _dbContext.Users<User>();
                    var filterUser = Builders<User>.Filter.Eq(x => x.LoginId, refreshToken.UserId);
                    var user = await users.Find(filterUser).FirstOrDefaultAsync();

                    return await GenerateToken(user);

                    static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
                    {
                        var dateTimeVal = new DateTime(year:1970,month:1,day:1,hour:0,minute:0,second:0,millisecond:0,DateTimeKind.Utc);
                        dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
                        return dateTimeVal;
                    }
                }
                else
                {
                    return new LoginResponse { Errors = new List<string> { "Invalid Tokens." } };
                }
            }catch
            {
                throw;
            }
        }
        #endregion
    }
}
