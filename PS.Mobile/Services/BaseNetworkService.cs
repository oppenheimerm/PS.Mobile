namespace PS.Mobile.Services
{
    public class BaseNetworkService
    {
        public HttpClient HttpClient;

        public BaseNetworkService(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.HttpClient = new HttpClient();
            this.HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }
}