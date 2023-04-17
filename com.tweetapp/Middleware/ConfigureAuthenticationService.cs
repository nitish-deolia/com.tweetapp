using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace com.tweetapp.Middleware
{
    public static class ConfigureAuthenticationService
    {
        public static void ConfigureAuthentication(this IServiceCollection services) {

            IConfiguration? configuration1 = services.BuildServiceProvider().GetService<IConfiguration>();
            IConfiguration configuration = configuration1;
            var key = Encoding.ASCII.GetBytes(configuration.GetSection(key: "AppSettings:Secret").Value);
            var tokenValidationParameter = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, //dev
                ValidateAudience = false, //dev
                ValidateLifetime = true,
                ValidIssuer = configuration.GetSection("AppSettings:JwtIssuer").Value,
                ValidAudience = configuration.GetSection("AppSettings:JwtAudience").Value,
            };

            services.AddSingleton(tokenValidationParameter);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameter;
            });
        }

    }
}
