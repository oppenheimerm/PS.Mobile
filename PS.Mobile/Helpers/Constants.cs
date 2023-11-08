
namespace PS.Mobile.Helpers
{
    public static class Constants
    {
        public const string baseUrl = "https://psusersapi.azurewebsites.net/";
        #region Urls
        public const string AuthenticateUserUrl = baseUrl + "api/users/authenticate";
        public const string RefreshToken = baseUrl + "/api/Users/refresh-token";
        public const string GetStations = baseUrl + "/api/Stations";
        #endregion
        public const string AuthUserCredentials = "AuthenticatedUserCredentials";
        public const string RefreshTokenValue = "RefreshToken";
        public const string RefreshTokenExpiry = "RefreshTokenExpiry";
    }
}