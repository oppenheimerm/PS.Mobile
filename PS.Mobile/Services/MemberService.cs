using PS.Core.Models.ApiRequestResponse;
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

        public async Task<(GetNearestStationsResponse Stations, bool success)> GetStationsAsync()
        {
            var authenticated = await AuthService.IsUserAuthenticated();
            if (authenticated.Success)
            {
                HttpResponseMessage httpResponseMessage = null;
                var baseAddress = new Uri(Helpers.Constants.baseUrl);
                using (var handler = new HttpClientHandler { UseCookies = false })
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    // Parameters needed for request:
                    //          Lat / Long
                    //          Country
                    //          DistanceUnit { Miles(0), Kilometers(1)}
                    var message = new HttpRequestMessage(HttpMethod.Get, $"{Helpers.Constants.GetStations}?fromLat={51.5237747}&fromLongt={-0.065196}&countryId={1}&units={1}");
                    message.Headers.Add("Cookie", $"refreshToken={authenticated.RefreshToken}");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticated.jwtToken);
                    httpResponseMessage = await client.SendAsync(message);
                    //httpResponseMessage.EnsureSuccessStatusCode();

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        try
                        {
                            var content = await httpResponseMessage.Content.ReadAsStringAsync();
                            PS.Core.Models.ApiRequestResponse.GetNearestStationsResponse stations = JsonConvert.DeserializeObject<GetNearestStationsResponse>(content);
                            return (stations, true);
                        }catch(Exception ex)
                        {
                            return (new GetNearestStationsResponse(), false);
                        }

                    }
                    else
                    {
                        return (new GetNearestStationsResponse(), false);
                    }
                }
            }
            else
            {
                return (new GetNearestStationsResponse(), false);
            }
        }
    }
}