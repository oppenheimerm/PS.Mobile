using Newtonsoft.Json;
using PS.Core.Models.ApiRequestResponse;
using PS.Mobile.Helpers;
using PS.Mobile.Models;
using PS.Mobile.Services.Interfaces;
using PS.Mobile.Services.Networking;
using System.Net.Http.Headers;
using System.Net.Http.Json;



namespace PS.Mobile.Services
{
    public class AuthService : BaseNetworkService, IAuthService
    {

        public AuthService(HttpClient httpClient) : base(httpClient)
        {
        }


        public async Task<(bool success, string errorMessage)> LoginAsync(PS.Mobile.Models.AuthenticateRequest request)
        {
            ///  REMOVE
            /*var devLogin = new PS.Mobile.Models.AuthenticateRequest()
            {
                EmailAddress = "joesephbrothag@gmail.com",
                Password = "==45&9!RrTtt="
            };*/


            //  Login tokne is only valid for 20 minutes from now
            var loginTimeStamp = DateTime.Now;
            var response = await HttpClient.PostAsJsonAsync(Constants.AuthenticateUserUrl, request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(content);
                IEnumerable<string> cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                var tokenDate = GetRefreshHeader(cookies);

                //var content = await httpResponseMessage.Content.ReadAsStringAsync();


                if (authResponse.StatusCode == 200 || !string.IsNullOrEmpty(authResponse.JwtToken))
                {

                    //  SaveData
                    var saveDataRequest = await SaveDataRequestAsync(authResponse, tokenDate, loginTimeStamp);

                    // we need to save the refresh token

                    if (saveDataRequest.Success)
                    {
                        return (true, string.Empty);
                    }
                    else
                    {
                        //  This should be successful, as we have received a 200 response and not a 
                        //  a 401, so there is some other issue
                        return (false, "Unable to complete this request.  Please contact customer support for assistance");
                    }

                }
                else if (authResponse.StatusCode == 401)
                {
                    //  USername or password was inccorrect
                    return (false, "Either username or password was incorrect.");
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

        public async Task<(bool Success, string jwtToken, string RefreshToken, string ErrorMessage)> IsUserAuthenticated()
        {

            //  Authenticate user should have data in secure storage
           var userData = await SecureStorage.Default.GetAsync(Constants.AuthUserCredentials);
            if (userData == null)
                return (false, string.Empty, string.Empty, "Please Login");

            //  Check the time stamp from when the user logged in
            var timeStamp = DateTime.Parse(await SecureStorage.Default.GetAsync(Constants.LoginTimeStamp));
            var timeNow = DateTime.Now;
            var timeSpan = timeNow - timeStamp;

            //  TODO
            // Check if refreshToken has also expired

            // If time span is >= 18 mins, we need to do a refresh token request
            if (timeSpan.TotalMinutes > 15) /* actual 20 */
            {
                //AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(content);
                // Get user stored data
                AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(userData);
                var headers = await SecureStorage.Default.GetAsync(Constants.RefreshTokenHeaders);
                HeaderData headerData = JsonConvert.DeserializeObject<HeaderData>(headers);
                var refreshTokenRequest = await RefreshTokensForUserAsync(authResponse, headerData);
                if(refreshTokenRequest.success)
                {
                    return (true, refreshTokenRequest.jwtToken, refreshTokenRequest.refreshToken, string.Empty); ;
                }
                return (false, string.Empty, string.Empty, refreshTokenRequest.errorMessage);
            }
            else
            {
                //  We're ok, just return stored data
                var storedData = await GetUserReFreshTokensAsync();
                return (true, storedData.userData.JwtToken, storedData.headers.Token, string.Empty);
            }

        }
                

        public async Task<(HeaderData headers, AuthenticateResponse userData)> GetUserReFreshTokensAsync()
        {
            var request = await SecureStorage.Default.GetAsync(Constants.RefreshTokenHeaders);
            var headers = JsonConvert.DeserializeObject<HeaderData>(request);
            var requestUsrData = await SecureStorage.Default.GetAsync(Constants.AuthUserCredentials);
            AuthenticateResponse credentials = JsonConvert.DeserializeObject<AuthenticateResponse>(requestUsrData);

            return (headers, credentials);
        }

        public async Task<AuthenticateResponse> GetUserDataAsync()
        {
            var rsp = await SecureStorage.GetAsync(Constants.AuthUserCredentials);
            AuthenticateResponse credentials = JsonConvert.DeserializeObject<AuthenticateResponse>(rsp);
            return credentials;
        }


        public void Logout()
        {
            SecureStorage.Default.Remove(Constants.AuthUserCredentials);
            SecureStorage.Default.Remove(Constants.LoginTimeStamp);
            SecureStorage.Default.Remove(Constants.RefreshTokenHeaders);
        }

        //  https://code-maze.com/add-bearertoken-httpclient-request/
        public async Task<(bool success, string jwtToken, string refreshToken, string errorMessage)> RefreshTokensForUserAsync(AuthenticateResponse request, HeaderData headers)
        {

            HttpResponseMessage httpResponseMessage = null;
            // Leave for none debug
            /*HttpResponseMessage httpResponseMessage = null;
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.JwtToken);*/


            //HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + response.JwtToken);
            //HttpClient.DefaultRequestHeaders.Add("refreshToken", response.JwtToken);

            //var HttpClient = new HttpClientService().GetPlatfromSpecificHttpClient();
            

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //HttpClient.DefaultRequestHeaders.Accept.Clear();
            //HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",request.JwtToken);
            //HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", request.JwtToken);
                       


            var refreshTokenTimeStamp = DateTime.Now;

            var baseAddress = new Uri("https://psusersapi.azurewebsites.net");
            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var message = new HttpRequestMessage(HttpMethod.Post, "/api/1/Users/refresh-token");
                message.Headers.Add("Cookie", $"refreshToken={headers.Token}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",request.JwtToken);
                httpResponseMessage = await client.SendAsync(message);
                //httpResponseMessage.EnsureSuccessStatusCode();


                if (httpResponseMessage.IsSuccessStatusCode)
                {

                    // Store the cookies from the response
                    //var headers = response.Headers;//.GetValues("refreshToken");
                    IEnumerable<string> cookies = httpResponseMessage.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

                    //  TODO - Preform check in cases when this parse function fails / handle it
                    var TokenDate = GetRefreshHeader(cookies);

                    var content = await httpResponseMessage.Content.ReadAsStringAsync();


                    AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(content);

                    if (authResponse.StatusCode == 200 || !string.IsNullOrEmpty(authResponse.JwtToken))
                    {
                        //Clear stale data
                        Logout();


                        var updateDataRequest = await SaveDataRequestAsync(authResponse, TokenDate, refreshTokenTimeStamp);


                        // Save user
                        //  Store token in secure storage
                        //var serializedUserData = JsonConvert.SerializeObject(authResponse);
                        // check for refresh token

                        //  Dont serilize user credential, just save.ToString()
                        //await SecureStorage.Default.SetAsync(Constants.AuthUserCredentials, authResponse.ToString());

                        //await SecureStorage.Default.SetAsync(Constants.RefreshTokenValue, authResponse.JwtToken);/* Token tied to Expiry */
                        //await SecureStorage.Default.SetAsync(Constants.RefreshTokenExpiry, TokenDate.Expires.ToString());
                        //await SecureStorage.Default.SetAsync(Constants.LoginTimeStamp, LoginTimeStamp.ToString());
                        if (updateDataRequest.Success)
                        {
                            return (true, authResponse.JwtToken, headers.Token, string.Empty);
                        }
                        else
                        {
                            // If we got this far, everyhitng should be ok.  Any failure here is from the call to
                            return (false, string.Empty, string.Empty, $"Could not login, please contact customer suppost");
                        }

                        //  TODO Retry authenticating user
                    }
                    else
                    {
                        return (false, string.Empty, string.Empty, $"Could not login, please contact customer suppost");
                    }
                }
                else
                {
                    return (false, string.Empty, string.Empty, $"Could not login, please contact customer suppost");
                }
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

        private async Task<(bool Success, string ErrorMessage)> SaveDataRequestAsync(AuthenticateResponse respone, HeaderData headers, 
            DateTime? loginTimeStamp)
        {
            try {
                var serializeddData = JsonConvert.SerializeObject(respone, Formatting.Indented);
                var searializedHeaders = JsonConvert.SerializeObject(headers, Formatting.Indented);

                await SecureStorage.Default.SetAsync(Constants.AuthUserCredentials, serializeddData);
               
                if (loginTimeStamp.HasValue)
                {
                    await SecureStorage.Default.SetAsync(Constants.LoginTimeStamp, loginTimeStamp.ToString());
                }
                if(headers != null)
                {
                    await SecureStorage.Default.SetAsync(Constants.RefreshTokenHeaders, searializedHeaders);
                }
                return (true, string.Empty);
            }catch(Exception ex)
            {
                return (false, ex.Message);
            }        
            
        }

    }
}