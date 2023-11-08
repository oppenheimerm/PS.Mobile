using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Models;

namespace PS.Mobile.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> IsUserAuthenticated();
        Task<(bool success, string errorMessage)> LoginAsync(AuthenticateRequest request);
        void Logout();
        Task<(bool success, string errorMessage)> RefreshTokensForUserAsync(string jwtToken);
        Task<HeaderData> GetUserRequestTokens();
        Task<AuthenticateResponse> GetUserDataAsync();
    }
}