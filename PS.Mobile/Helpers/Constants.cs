
namespace PS.Mobile.Helpers
{
    public static class Constants
    {
        public const string baseUrl = "https://psusersapi.azurewebsites.net";
        #region Urls
        public const string AuthenticateUserUrl = "/api/users/authenticate";
        public const string RefreshToken = "/api/Users/refresh-token";
        public const string GetStations = "/api/Stations";
        #endregion
        public const string AuthUserCredentials = "AuthenticatedUserCredentials";
        public const string LoginTimeStamp = "LoginTimeStampDatTime";
        /// <summary>
        /// For Token Header object
        /// </summary>
        public const string RefreshTokenHeaders = "RefreshTokenHeaders";

        //public const string IsBusyRequestMessage = "Busy";
        
    }
}