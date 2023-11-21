
namespace PS.Mobile.Services.Networking 
{
    //https://www.youtube.com/watch?v=-Wj1JYkgWNU
    public partial class HttpClientService
    {
        public HttpClient GetPlatfromSpecificHttpClient()
        {
            var httpMessageHandler = GetPlatformSepcificHttpMessageHandler();
            return new HttpClient(httpMessageHandler);
        }

        public partial HttpMessageHandler GetPlatformSepcificHttpMessageHandler();
    }
}
