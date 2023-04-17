namespace com.tweetapp.Models.Context
{
    public class TweetAppDatabaseSettings : ITweetAppDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UserCollectionName { get; set; }
        public string TweetCollectionName { get; set; }
        public string RefreshTokenCollectionName { get; set; }
    }

    public interface ITweetAppDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string UserCollectionName { get; set; }
        string TweetCollectionName { get; set; }
        string RefreshTokenCollectionName { get; set; }
    }
}
