namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class AdvancedConfiguration
    {
        public string IssuerUri { get; set; }
        public bool CorsAllowAnyOrigin { get; set; }

        public string[] CorsAllowOrigins { get; set; }
    }
}
