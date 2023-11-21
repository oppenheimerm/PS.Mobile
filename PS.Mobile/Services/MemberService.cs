using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Services.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Json;

namespace PS.Mobile.Services
{
    public class MemberService : BaseNetworkService, IMemberService
    {
        private readonly IAuthService AuthService;

        public MemberService(IAuthService authService, HttpClient httpClient) : base(httpClient)
        {
            AuthService = authService;
        }

        public async Task<(List<StationLite> Stations, bool success)> GetStationsAsync()
        {
            var authenticated = await AuthService.IsUserAuthenticated();
            if (authenticated.Success)
            {
                HttpResponseMessage httpResponseMessage = null;
                var baseAddress = new Uri(Helpers.Constants.baseUrl);
                using (var handler = new HttpClientHandler { UseCookies = false })
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, Helpers.Constants.GetStations);
                    message.Headers.Add("Cookie", $"refreshToken={authenticated.RefreshToken}");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticated.jwtToken);
                    httpResponseMessage = await client.SendAsync(message);
                    //httpResponseMessage.EnsureSuccessStatusCode();

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        List<StationLite> Stations = JsonConvert.DeserializeObject<List<StationLite>>(content);
                        return (Stations, true);

                    }
                    else
                    {
                        return (new List<StationLite>(), false);
                    }
                }
            }
            else
            {
                return (new List<StationLite>(), false);
            }
        }
    }
}