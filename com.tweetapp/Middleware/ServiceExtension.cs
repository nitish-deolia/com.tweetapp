using com.tweetapp.Configuration;

namespace com.tweetapp.Middleware
{
    public static class ServiceExtension
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );
            });
        }

        public static void ConfigureAppSettings(this IServiceCollection services)
        {
            IConfiguration? configuration1 = services.BuildServiceProvider().GetService<IConfiguration>();
            IConfiguration configuration = configuration1;
            services.Configure<AppsettingsConfig>(option => configuration.GetSection("AppSettings").Bind(option));
        }
    }
}
