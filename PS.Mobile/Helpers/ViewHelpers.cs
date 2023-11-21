
namespace PS.Mobile.Helpers
{
    public static class ViewHelpers
    {
        public static List<string> GetLogoPaths(List<string> logos)
        {
            var result = new List<string>();
            foreach (string item in logos)
            {
                string url = PS.Mobile.Helpers.Constants.baseUrl + item;
                result.Add(url);
            }
            return result;
        }
    }
}
