using com.tweetapp.Models.Context;
using MongoDB.Driver;

namespace com.tweetapp.Middleware
{
    public static class ConfigureMongoDB
    {
        public static void ConfigureMongoDatabase(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            IConfiguration? configuration1 = services.BuildServiceProvider().GetService<IConfiguration>();
            IConfiguration configuration = configuration1;
            var settings = GetMongoDBSettings(configuration);
            services.AddSingleton<ITweetAppDatabaseSettings>(settings);
            var mongoClient = new MongoClient(settings.ConnectionString);
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton(mongoClient.GetDatabase(settings.DatabaseName));

            //AddMongoDbServices<TweetService, TweetModel>(settings.TweetCollectionName);
            //AddMongoDbServices<UserService, UserModel>(settings.UserCollectionName);

            //void AddMongoDbServices<TService, TModel>(string collectionName)
            //{
            //    services.AddSingleton(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<TModel>(collectionName));
            //    services.AddSingleton(typeof(TService));
            //}
        }

        public static TweetAppDatabaseSettings GetMongoDBSettings(IConfiguration configuration) =>
                    configuration.GetSection(nameof(TweetAppDatabaseSettings)).Get<TweetAppDatabaseSettings>();

        //static IMongoDatabase CreateMongoDatabase(TweetAppDatabaseSettings settings)
        //{
        //    var client = new MongoClient(settings.ConnectionString);
        //    return client.GetDatabase(settings.DatabaseName);
        //}
    }

}
