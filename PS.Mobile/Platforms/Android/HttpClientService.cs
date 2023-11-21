
using System.Net.Security;
using Xamarin.Android.Net;

namespace PS.Mobile.Services.Networking
{
    //  https://www.youtube.com/watch?v=-Wj1JYkgWNU
    public partial class HttpClientService
    {
        public partial HttpMessageHandler GetPlatformSepcificHttpMessageHandler()
        {
            var androidHttpHandler = new AndroidMessageHandler
            { 
                ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) =>
                {
                    if((certificate?.Issuer == "CN=localhost") || (sslPolicyErrors == SslPolicyErrors.None))
                        return true;
                    return false;
                }
            };
            return androidHttpHandler;
        }
    }
}
