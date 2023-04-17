using com.tweetapp.Configuration;
using com.tweetapp.DataManager.Interfaces;
using com.tweetapp.Models;
using com.tweetapp.Models.Context;
using com.tweetapp.Models.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace com.tweetapp.DataManager.Repository
{
    public class TweetDataManager : ITweetDataManager
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IOptions<AppsettingsConfig> _appSettings;

        public TweetDataManager(IMongoDbContext dbContext, IOptions<AppsettingsConfig> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings;
        }

        public async Task<List<TweetModel>> AllTweets()
        {
            List<TweetModel> tweetList = new();
            var filter = Builders<Tweet>.Filter.Eq(tweet => tweet.IsActive, true);
            var projection = Builders<Tweet>.Projection.Expression(tweet => new TweetModel
            {
                Id = tweet.Id,
                TweetMessage = tweet.TweetMessage,
                Author_Id = tweet.Author_Id,
                Created_At= tweet.Created_At,
                Tags = tweet.Tags,
                Likes = tweet.Likes,
                ReplyList = tweet.ReplyList.Select(reply => new ReplyModel
                            {
                                Id = reply.Id.ToString(),
                                ReplyMessage= reply.ReplyMessage,
                                Author_Id = reply.Author_Id,
                                Likes= reply.Likes,
                                Created_At = reply.Created_At,
                                Tags = reply.Tags,
                            }).ToList(),
                UpdateHistory = tweet.UpdateHistory.Select(updateHistory => new TweetUpdateModel
                            {
                                Id = updateHistory.Id.ToString(),
                                Message = updateHistory.Message,
                                UpdatedOn = updateHistory.UpdatedOn,
                            }).ToList(),
                UpdateUntil = tweet.UpdateUntil,
                IsActive = tweet.IsActive
            });

            tweetList = await _dbContext.Tweets<Tweet>().Aggregate().Match(filter).Project(projection).ToListAsync();
            return tweetList;
        }

        public async Task<Response> AddTweet(string username,TweetRequest tweetObj)
        {
            var userFilter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.LoginId, username),Builders<User>.Filter.Eq(x => x.IsActive,true));
            var user = _dbContext.Users<User>().Find(userFilter);
            var charCheck = tweetObj.TweetMessage.Length > 144;
            var tagCheck = String.Join("", tweetObj.Tags.ToArray()).Length > 144;
            if(!charCheck && !tagCheck)
            {
                Tweet tweet = new()
                {
                    TweetMessage = tweetObj.TweetMessage,
                    Author_Id = username,
                    Tags = tweetObj.Tags,
                    Created_At = DateTime.Now,
                    Likes = 0,
                    ReplyList = new List<Reply>(),
                    UpdateHistory = new List<TweetUpdate>()
                    {
                        new TweetUpdate()
                        {
                            Id = ObjectId.GenerateNewId(),
                            Message = tweetObj.TweetMessage,
                            UpdatedOn = DateTime.Now
                        }
                    },
                    UpdateUntil = DateTime.Now.AddMinutes(30),
                    IsActive = true
                };

                await _dbContext.Tweets<Tweet>().InsertOneAsync(tweet);
                var updateTweet = Builders<User>.Update.Push(user => user.Tweets,tweet.Id.ToString());
                var resultObject = await _dbContext.Users<User>().UpdateOneAsync(userFilter, updateTweet);
                if(resultObject != null)
                {
                    return new Response { Success= true };
                }else return new Response { Success = false , Errors = new List<string>() { "Something went wrong.Please Try again Later."} };
            }
            else
            {
                return new Response { Success = false, Errors= new List<string>() { "Character limit for Tweet for Tag" } };
            }
        }

        public async Task<List<TweetModel>> GetAllTweetsByUsername(string username)
        {
            var tweetResult = new List<TweetModel>();
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.LoginId, username), Builders<User>.Filter.Eq(x => x.IsActive, true));
            var tweetIdList = (await _dbContext.Users<User>().Find(filter).FirstOrDefaultAsync()).Tweets?.ToList();
            var projection = Builders<Tweet>.Projection.Expression(tweet => new TweetModel
            {
                Id = tweet.Id,
                TweetMessage = tweet.TweetMessage,
                Author_Id = tweet.Author_Id,
                Created_At = tweet.Created_At,
                Tags = tweet.Tags,
                Likes = tweet.Likes,
                ReplyList = tweet.ReplyList.Select(reply => new ReplyModel
                {
                    Id = reply.Id.ToString(),
                    ReplyMessage = reply.ReplyMessage,
                    Author_Id = reply.Author_Id,
                    Likes = reply.Likes,
                    Created_At = reply.Created_At,
                    Tags = reply.Tags,
                }).ToList(),
                UpdateHistory = tweet.UpdateHistory.Select(updateHistory => new TweetUpdateModel
                {
                    Id = updateHistory.Id.ToString(),
                    Message = updateHistory.Message,
                    UpdatedOn = updateHistory.UpdatedOn,
                }).ToList(),
                UpdateUntil = tweet.UpdateUntil,
                IsActive = tweet.IsActive
            });

            var tweetFilter = Builders<Tweet>.Filter.And(Builders<Tweet>.Filter.Eq(x => x.IsActive, true), Builders<Tweet>.Filter.In(x => x.Id, tweetIdList));
            tweetResult = await _dbContext.Tweets<Tweet>().Find(tweetFilter).Project(projection).ToListAsync();
            return tweetResult;
        }

        public async Task<Response> UpdateTweet(string username,string id,TweetRequest tweetObj)
        {
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.LoginId, username), Builders<User>.Filter.Eq(x => x.IsActive, true));
            var tweetsIds = (await _dbContext.Users<User>().Find(filter).FirstOrDefaultAsync()).Tweets?.ToList();
            var tweetFilter = Builders<Tweet>.Filter.And(Builders<Tweet>.Filter.Eq(x => x.Id, id),
                                                         Builders<Tweet>.Filter.Eq(x => x.Author_Id,username),
                                                         Builders<Tweet>.Filter.Eq(x => x.IsActive, true));
            var tweet = await _dbContext.Tweets<Tweet>().Aggregate().Match(tweetFilter).FirstOrDefaultAsync();
            if(tweet.UpdateUntil > DateTime.UtcNow && tweetsIds.Contains(id))
            {
                var update = Builders<Tweet>.Update.Set(x => x.TweetMessage, tweetObj.TweetMessage)
                                                    .Set(x => x.Tags, tweetObj.Tags)
                                                    .Set(x => x.Created_At, DateTime.UtcNow)
                                                    .Push(x => x.UpdateHistory, new TweetUpdate
                                                    {
                                                        Id = ObjectId.GenerateNewId(),
                                                        Message = tweetObj.TweetMessage,
                                                        UpdatedOn = DateTime.UtcNow
                                                    });
                var result = await _dbContext.Tweets<Tweet>().UpdateOneAsync(tweetFilter, update);
                if (result != null) return new Response { Success = true };
                else return new Response { Success = false, Errors = new List<string>() { "Something went wrong." } };
            }
            else
            {
                return new Response { Success = false, Errors = new List<string>() { "Cannot Update tweet." } };
            }
        }

        public async Task<Response> DeleteTweet(string username,string id)
        {
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.LoginId, username),Builders<User>.Filter.Eq(x => x.IsActive,true));
            var tweetIds = (await _dbContext.Users<User>().Find(filter).FirstOrDefaultAsync()).Tweets?.ToList();
            if(tweetIds != null && tweetIds.Contains(id))
            {
                var pullTweet = Builders<User>.Update.Pull(x => x.Tweets, id);
                var resultRemovefromUser = await _dbContext.Users<User>().UpdateOneAsync(filter, pullTweet);

                var tweetFilter = Builders<Tweet>.Filter.And(Builders<Tweet>.Filter.Eq(x => x.Id, id),
                                  Builders<Tweet>.Filter.Eq(x => x.Author_Id,username),
                                  Builders<Tweet>.Filter.Eq(x => x.IsActive ,true));

                await _dbContext.Tweets<Tweet>().DeleteOneAsync(tweetFilter);
                return new Response { Success = true };
            }else return new Response { Success = false , Errors = new List<string>() { "Tweet Doesn't exist."} };
        }

        public async Task<Response> LikeTweet(string username,string id,bool like)
        {
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.LoginId, username), Builders<User>.Filter.Eq(x => x.IsActive, true));
            var tweetIds = (await _dbContext.Users<User>().Find(filter).FirstOrDefaultAsync()).Tweets?.ToList();
            if (tweetIds != null && tweetIds.Contains(id))
            {
                var tweetFilter = Builders<Tweet>.Filter.And(Builders<Tweet>.Filter.Eq(x => x.Id, id),
                                  Builders<Tweet>.Filter.Eq(x => x.Author_Id, username),
                                  Builders<Tweet>.Filter.Eq(x => x.IsActive, true));

                UpdateDefinition<Tweet> update;
                if(like)
                {
                    update = Builders<Tweet>.Update.Inc(x => x.Likes, 1);
                }
                else
                {
                    update = Builders<Tweet>.Update.Inc(x => x.Likes, -1);
                }

                await _dbContext.Tweets<Tweet>().UpdateOneAsync(tweetFilter,update);
                return new Response { Success = true };
            }
            else return new Response { Success = false, Errors = new List<string>() { "Tweet Doesn't exist." } };
        }

        public async Task<Response> ReplyTweet(string username,string id,ReplyRequest replyRequest)
        {
            var filter = Builders<User>.Filter.And(Builders<User>.Filter.Eq(x => x.LoginId, username), Builders<User>.Filter.Eq(x => x.IsActive, true));
            var tweetIds = (await _dbContext.Users<User>().Aggregate().Match(filter).FirstOrDefaultAsync()).Tweets?.ToList();
            if(tweetIds != null && tweetIds.Contains(id))
            {
                var charCheck = replyRequest.ReplyMessage.Length > 144;
                var tagCheck = String.Join("", replyRequest.Tags.ToArray()).Length > 144;
                if (!charCheck && !tagCheck)
                {
                    var tweetFilter = Builders<Tweet>.Filter.And(Builders<Tweet>.Filter.Eq(x => x.Id, id),
                                 Builders<Tweet>.Filter.Eq(x => x.Author_Id, username),
                                 Builders<Tweet>.Filter.Eq(x => x.IsActive, true));

                    Reply reply = new Reply
                    {
                        Id = ObjectId.GenerateNewId(),
                        ReplyMessage = replyRequest.ReplyMessage,
                        Author_Id = username,
                        Likes = 0,
                        Created_At = DateTime.UtcNow,
                        Tags = replyRequest.Tags
                    };

                    var update = Builders<Tweet>.Update.Push(x => x.ReplyList, reply);
                    var resultObject = await _dbContext.Tweets<Tweet>().UpdateOneAsync(tweetFilter, update);
                    if (resultObject != null)
                    {
                        return new Response { Success = true };
                    }
                    else return new Response { Success = false, Errors = new List<string>() { "Something went wrong.Please Try again Later." } };
                }
                else
                {
                    return new Response { Success = false, Errors = new List<string>() { "Character limit for Tweet for Tag" } };
                }
            }
            else
            {
                return new Response { Success = false, Errors = new List<string>() { "Tweet Not Found." } };
            }
        }
    }
}
