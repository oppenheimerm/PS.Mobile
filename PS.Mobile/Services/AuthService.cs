using Newtonsoft.Json;
using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Helpers;
using PS.Mobile.Models;
using PS.Mobile.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
//using System.Text.Json;

namespace PS.Mobile.Services
{
    public class AuthService : BaseNetworkService, IAuthService
    {

        public AuthService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var serializedData = await SecureStorage.Default.GetAsync(Constants.AuthUserCredentials);
            return !string.IsNullOrWhiteSpace(serializedData);
        }

        public async Task<(bool success, string errorMessage)> LoginAsync(AuthenticateRequest request)
        {

            var response = await HttpClient.PostAsJsonAsync(Constants.AuthenticateUserUrl, request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(content);
                if (authResponse.StatusCode == 200 || !string.IsNullOrEmpty(authResponse.JwtToken))
                {
                    var refreshRequest = await RefreshTokensForUserAsync(authResponse.JwtToken);
                    if (refreshRequest.success)
                    {
                        return (true, string.Empty);
                    }
                    else
                    {
                        return (false, $"Could not login, please contact customer suppost");
                    }

                }
                else
                {
                    return (false, authResponse.Message);
                }
            }
            else
            {
                return (false, $"Could not login, please contact customer suppost");
            }
        }

        public async Task<HeaderData> GetUserRequestTokens()
        {           
            var TokenData = new HeaderData
            {
                Expires = DateTime.Parse(await SecureStorage.GetAsync(Constants.RefreshTokenExpiry)),
                Token = await SecureStorage.GetAsync(Constants.RefreshTokenValue)
            };

            return TokenData;
        }

        public async Task<AuthenticateResponse> GetUserDataAsync()
        {
            var rsp = await SecureStorage.GetAsync(Constants.AuthUserCredentials);
            AuthenticateResponse credentials = JsonConvert.DeserializeObject<AuthenticateResponse>(rsp);
            return credentials;
        }

        public async Task<StationsResponse> GetPetrolStations()
        {
            StationsResponse stations;
            var response = await HttpClient.GetAsync(Constants.GetStations);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                /*stations = JsonSerializer.Deserialize<StationsResponse>(content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });*/
                stations = JsonConvert.DeserializeObject<StationsResponse>(content);
                if (stations.StatusCode == 200)
                {
                    return stations;
                }
                else
                {
                    // TODO try to login if 401
                    //  Return empty station response for now
                    return new StationsResponse();
                }
            }
            else
            {
                //  TODO Additional login
                return null;
            }
        }

        public void Logout()
        {
            SecureStorage.Default.Remove(Constants.AuthUserCredentials);
            SecureStorage.Default.Remove(Constants.RefreshTokenValue);
            SecureStorage.Default.Remove(Constants.RefreshTokenExpiry);
        } 

        public async Task<(bool success, string errorMessage)> RefreshTokensForUserAsync(string jwtToken)
        {

            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await HttpClient.PostAsJsonAsync(Constants.RefreshToken, string.Empty);
            if (response.IsSuccessStatusCode)
            {

                // Store the cookies from the response
                //var headers = response.Headers;//.GetValues("refreshToken");
                IEnumerable<string> cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

                //  TODO - Preform check in cases when this parse function fails / handle it
                var TokenDate = GetRefreshHeader(cookies);

                var content = await response.Content.ReadAsStringAsync();


                AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(content);
                if (authResponse.StatusCode == 200 || !string.IsNullOrEmpty(authResponse.JwtToken))
                {
                    //  Store token in secure storage
                    var serializedUserData = JsonConvert.SerializeObject(authResponse);
                    // check for refresh token
                    await SecureStorage.Default.SetAsync(Constants.AuthUserCredentials, serializedUserData);
                    await SecureStorage.Default.SetAsync(Constants.RefreshTokenValue, TokenDate.Token);
                    await SecureStorage.Default.SetAsync(Constants.RefreshTokenExpiry, TokenDate.Expires.ToString());
                    return (true, string.Empty);

                    //  TODO Retry authenticating user
                }
                else
                {
                    return (false, authResponse.Message);
                }
            }
            else
            {
                return (false, $"Could not login, please contact customer suppost");
            }
        }


        private HeaderData GetRefreshHeader(IEnumerable<string> httpOnlyString)
        {
            //  refreshToken=4uRZ2lORFcpGNeuZhTvUWiC0cW7yPi2jhTutKjlmvadypNJhWo3AY4tUETncao5%2FAh2S%2BSPo5rYE%2B7CgMEzPEw%3D%3D; expires=Tue, 14 Nov 2023 17:54:02 GMT; path=/; httponly
            var token = from value in httpOnlyString
                        where value.Contains("refreshToken")
                        select value;

            var tokenPre = token.FirstOrDefault();
            if (tokenPre != null) {
                var tokenPost = tokenPre.Split("=");

                var HeaderData = new HeaderData
                {
                    Expires = GetRefreshTokenExpiry(tokenPost[2].ToString()),
                    Token = parseTokenString(tokenPost[1].ToString())
                };
                return HeaderData;
            }
            else
            {
                return new HeaderData();
            }

        }

        private DateTime GetRefreshTokenExpiry(string date)
        {
            //[2] = Tue, 14 Nov 2023 18:44:26 GMT; path
            var preParse = date.Split(";");
            var PostParse1 = preParse[0];
            var PostParse2 = DateTime.Parse(PostParse1);

            return PostParse2;
        }

        private string parseTokenString(string val)
        {
            //[1] = jGghDPiCT1T7%2FIB6zzoeTrnUMS1qoAaEfP4oyI2nO9tZtJ%2F0FrVDKL6qsWGiq4LgD9sx5HeeruritN0ocQtD9A%3D%3D; expires
            var preParse = val;
            var postParse = val.Split(";");
            return postParse[0].ToString();
        }

    }
}