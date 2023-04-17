using com.tweetapp.Models;
using com.tweetapp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.BusinessLayer.Interfaces
{
    public interface ITweetService
    {
        public Task<List<TweetModel>> AllTweets();
        public Task<List<TweetModel>> GetAllTweetByUsername(string username);
        public Task<Response> AddTweet(string username,TweetRequest tweetObj);
        public Task<Response> UpdateTweet(string username, string id,TweetRequest tweetObj);
        public Task<Response> DeleteTweet(string username, string id);
        public Task<Response> LikeTweet(string username, string id,bool like);
        public Task<Response> ReplyTweet(string username, string id,ReplyRequest replyRequest);
      
    }
}
