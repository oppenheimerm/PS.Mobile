using PS.Core.Models.ApiRequestResponse;
using System.Net.Http.Headers;
using PS.Mobile.Helpers;
using PS.Mobile.Services.Interfaces;
using Newtonsoft.Json;

namespace PS.Mobile.Services
{
    public class MemberService : BaseNetworkService, IMemberService
    {
        private readonly IAuthService AuthService;

        public MemberService(IAuthService authService, HttpClient httpClient) : base(httpClient)
        {
            AuthService = authService;
        }

        public async Task<(List<StationLite>Stations, bool success)> GetStationsAsync()
        {
            var tokens = await AuthService.GetUserDataAsync();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.JwtToken);

            var response = await HttpClient.GetAsync(Constants.GetStations);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                List<StationLite> Stations = JsonConvert.DeserializeObject<List<StationLite>>(content);
                return (Stations, true);
            }
            else
            {
                return (new List<StationLite>(), false);
            }
        }


    }
}