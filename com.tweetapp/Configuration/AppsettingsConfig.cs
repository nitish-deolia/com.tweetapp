namespace com.tweetapp.Configuration
{
    public class AppsettingsConfig
    {
        public string Secret { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set;}
        public TimeSpan ExpiryTime { get; set; }
    }
}
