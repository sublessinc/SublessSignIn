namespace Subless.Models
{
    public class AuthSettings
    {
        public string Region { get; set; }
        public string PoolId { get; set; }
        public string AppClientId { get; set; }
        public string IssuerUrl { get; set; }
        public string JwtKeySetUrl { get; set; }
        public string CognitoUrl { get; set; }
        public string Domain { get; set; }
        public string IdentityServerLicenseKey { get; set; }
        public string SessionStoreConnString { get; set; }
        public string CancellationSurvey { get; set; }
    }
}
