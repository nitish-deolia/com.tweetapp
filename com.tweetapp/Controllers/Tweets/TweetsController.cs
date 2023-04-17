using com.tweetapp.BusinessLayer.Interfaces;
using com.tweetapp.Models;
using com.tweetapp.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.Controllers.Tweets
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly ITweetService _tweetService;
        private readonly IUserService _userService;
        public TweetsController(ITweetService tweetService,IUserService userService)
        {
            this._tweetService = tweetService;
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> RegisterUser([FromBody] RegisterRequest registerRequest)
        {
            var result = new RegisterResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    result = await this._userService.RegisterUser(registerRequest);
                    return new OkObjectResult(result); 
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                //logger exception
                return StatusCode(500,ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public async Task<ActionResult<LoginResponse>> Login(string username,string password)
        {
            LoginResponse loginResponse;
            try
            {
                loginResponse = await this._userService.Login(username,password);
                if(loginResponse == null)
                {
                    return BadRequest();
                }
                else
                {
                    return new OkObjectResult(loginResponse);
                }
            }
            catch (Exception ex)
            {
                //logger exception
                return Unauthorized();
            }
        }

        [HttpGet("{{username}}/forget")]
        public async Task<ActionResult<ForgotResponse>> ForgotPassword(string username,string newPaswword,string confirmNewPassword)
        {
            ForgotResponse forgotResponse = new();
            try
            {
                forgotResponse = await this._userService.ForgotPassword(username,newPaswword,confirmNewPassword);
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return forgotResponse;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<TweetModel>>> AllTweets()
        {
            List<TweetModel> tweetModelList = new List<TweetModel>();
            try
            {
                tweetModelList = await this._tweetService.AllTweets();
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return tweetModelList;
        }

        [HttpGet("users/all")]
        public async Task<ActionResult<List<UserModel>>> AllUsers()
        {
            List<UserModel> userModelList = new List<UserModel>();
            try
            {
                userModelList = await this._userService.AllUsers();
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return userModelList;
        }

        [HttpGet("user/search/{{username}}")]
        public async Task<ActionResult<List<UserModel>>> SearchUser(string username)
        {
            List<UserModel> userModelList = new();  
            try
            {
                userModelList = await this._userService.SearchUser(username);
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return userModelList;
        }

        [HttpGet("{{username}}")]
        public async Task<ActionResult<List<TweetModel>>> GetAllTweetByUsername(string username)
        {
            List<TweetModel> tweetModelList = new List<TweetModel>();
            try
            {
                tweetModelList = await this._tweetService.GetAllTweetByUsername(username);
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return tweetModelList;
        }

        [HttpPost("{{username}}/add")]
        public async Task<ActionResult<Response>> AddTweet(string username,[FromBody] TweetRequest tweetObj)
        {
            Response addResponse = new();
            try
            {
                if(ModelState.IsValid)
                {
                    addResponse = await this._tweetService.AddTweet(username,tweetObj);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return addResponse;
        }

        [HttpPut("{{username}}/update/{{id}}")]
        public async Task<ActionResult<Response>> UpdateTweet(string username,string id, [FromBody] TweetRequest tweetObj)
        {
            Response response = new();
            try
            {
                response = await this._tweetService.UpdateTweet(username, id, tweetObj);
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return response;
        }

        [HttpDelete("{{username}}/delete/{{id}}")]
        public async Task<ActionResult<Response>> DeleteTweet(string username,string id)
        {
            Response response = new();
            try
            {
                response = await this._tweetService.DeleteTweet(username, id);
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return response;
        }

        [HttpPut("{{username}}/like/{{id}}")]
        public async Task<ActionResult<Response>> LikeTweet(string username,string id,bool like)
        {
            Response response = new();
            try
            {
                response = await this._tweetService.LikeTweet(username, id,like);
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return response;
        }

        [HttpPost("{{username}}/reply/{{id}}")]
        public async Task<ActionResult<Response>> ReplyTweet(string username, string id, [FromBody] ReplyRequest replyRequest)
        {
            Response response = new();
            try
            {
                response = await this._tweetService.ReplyTweet(username, id,replyRequest);
            }
            catch (Exception ex)
            {
                //logger exception
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("refreshtoken")]
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var tokenResponse = await _userService.VerifyAndGenerateToken(tokenRequest);
                    return new OkObjectResult(tokenResponse);
                }else return BadRequest();
            }catch(Exception ex)
            {
                //logger exception
                return StatusCode(500,ex.Message);
            }
        }
    }
}
