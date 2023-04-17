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
    public class TweetService : ITweetService
    {
        private readonly ITweetDataManager _tweetDataManager;
        public TweetService(ITweetDataManager tweetDataManager)
        {
            _tweetDataManager = tweetDataManager;
        }
        public async Task<Response> AddTweet(string username, TweetRequest tweetObj)
        {
            return await _tweetDataManager.AddTweet(username,tweetObj);
        }

        public async Task<List<TweetModel>> AllTweets()
        {
            return await _tweetDataManager.AllTweets();
        }

        public async Task<Response> DeleteTweet(string username, string id)
        {
            return await _tweetDataManager.DeleteTweet(username,id);
        }

        public async Task<List<TweetModel>> GetAllTweetByUsername(string username)
        {
            return await _tweetDataManager.GetAllTweetsByUsername(username);
        }

        public async Task<Response> LikeTweet(string username, string id,bool like)
        {
            return await _tweetDataManager.LikeTweet(username,id,like);
        }

        public async Task<Response> ReplyTweet(string username, string id,ReplyRequest replyRequest)
        {
            return await _tweetDataManager.ReplyTweet(username, id,replyRequest);
        }

        public async Task<Response> UpdateTweet(string username, string id,TweetRequest tweetObj)
        {
            return await _tweetDataManager.UpdateTweet(username,id,tweetObj);
        }
    }
}
