using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace com.tweetapp.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LoginId { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public List<string>? Tweets { get; set; }
    }

    public class TweetModel
    {
        public string Id { get; set; }
        public string TweetMessage { get; set; }
        public string Author_Id { get; set; }
        public DateTime Created_At { get; set; }
        public List<string> Tags { get; set; }
        public int Likes { get; set; }
        public List<ReplyModel> ReplyList { get; set; }
        public List<TweetUpdateModel> UpdateHistory { get; set; }
        public DateTime UpdateUntil { get; set; }
        public bool IsActive { get; set; }
    }

    public class ReplyModel
    {
        public string Id { get; set; }
        public string ReplyMessage { get; set; }
        public string Author_Id { get; set; }
        public int Likes { get; set; }
        public DateTime Created_At { get; set; }
        public List<string> Tags { get; set; }
    }

    public class TweetUpdateModel
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    #region Login,Register,Forgot
    public class LoginResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Errors { get; set; }
    }

    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get;set; }
        [Required] 
        public string Email { get; set; }
        [Required] 
        public string LoginId { get; set; }
        [Required]
        public string Password { get;set; }
        [Required] 
        public string ConfirmPassword { get; set; }
        [Required] 
        public string PhoneNumber { get;set; }


    }

    public class RegisterResponse
    {
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
    }

    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }

    public class ForgotResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
    #endregion

    public class Response
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }

    public class TweetRequest
    {
        [Required]
        public string TweetMessage { get; set; }
        public List<string> Tags { get; set; }
    }

    public class ReplyRequest
    {
        [Required]
        public string ReplyMessage { get; set; }
        public List<string> Tags { get; set; }
    }

}
