using com.tweetapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace com.tweetapp.DataManager.Interfaces
{
    public interface ITweetDataManager
    {
        Task<List<TweetModel>> AllTweets();
        Task<Response> AddTweet(string username, TweetRequest tweetObj);
        Task<List<TweetModel>> GetAllTweetsByUsername(string username);

        Task<Response> UpdateTweet(string username,string id, TweetRequest tweetObj);
        Task<Response> DeleteTweet(string username,string id);
        Task<Response> LikeTweet(string username, string id,bool like);
        public Task<Response> ReplyTweet(string username, string id, ReplyRequest replyRequest);
    }
}
