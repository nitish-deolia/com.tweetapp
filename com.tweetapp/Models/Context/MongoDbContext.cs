using com.tweetapp.Models.Entities;
using MongoDB.Driver;

namespace com.tweetapp.Models.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly ITweetAppDatabaseSettings _dbSettings;
        public MongoDbContext(IMongoDatabase database,ITweetAppDatabaseSettings tweetAppDatabaseSettings)
        {
            _database = database;
            _dbSettings = tweetAppDatabaseSettings;
        }

        public IMongoCollection<User> Users<User>() => _database.GetCollection<User>(_dbSettings.UserCollectionName);
        public IMongoCollection<Tweet> Tweets<Tweet>() => _database.GetCollection<Tweet>(_dbSettings.TweetCollectionName);
        public IMongoCollection<RefreshToken> RefreshTokens<RefreshToken>() => _database.GetCollection<RefreshToken>(_dbSettings.RefreshTokenCollectionName);
    }

    public interface IMongoDbContext
    {
        IMongoCollection<User> Users<User>();
        IMongoCollection<Tweet> Tweets<Tweet>();
        IMongoCollection<RefreshToken> RefreshTokens<RefreshToken>();
    }
}
