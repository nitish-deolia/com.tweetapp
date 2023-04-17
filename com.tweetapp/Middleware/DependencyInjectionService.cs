using com.tweetapp.BusinessLayer.Interfaces;
using com.tweetapp.BusinessLayer.Services;
using com.tweetapp.DataManager.Interfaces;
using com.tweetapp.DataManager.Repository;
using com.tweetapp.Models.Context;

namespace com.tweetapp.Middleware
{
    public static class DependencyInjectionService
    {
        public static void ConfigureDependencyInjection(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //DI
            services.AddScoped<ITweetDataManager, TweetDataManager>();
            services.AddScoped<IUserDataManager,UserDataManager>();

            services.AddScoped<ITweetService,TweetService>();
            services.AddScoped<IUserService,UserService>();

            services.AddSingleton<IMongoDbContext, MongoDbContext>();
        }
    }
}
