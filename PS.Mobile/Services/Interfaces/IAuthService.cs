using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Models;

namespace PS.Mobile.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string jwtToken, string RefreshToken, string ErrorMessage)> IsUserAuthenticated();
        Task<(bool success, string errorMessage)> LoginAsync(PS.Mobile.Models.AuthenticateRequest request);
        void Logout();

        /// <summary>
        /// Must be called for all request requiring authentication.  Automatically handles logging in with
        /// refesh tokens if available
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <param name="LoginTimeStamp"></param>
        /// <returns></returns>        
        Task<(bool success, string jwtToken, string refreshToken, string errorMessage)> RefreshTokensForUserAsync(AuthenticateResponse request, HeaderData headers);
        Task<(HeaderData headers, AuthenticateResponse userData)> GetUserReFreshTokensAsync();
        Task<AuthenticateResponse> GetUserDataAsync();

    }
}