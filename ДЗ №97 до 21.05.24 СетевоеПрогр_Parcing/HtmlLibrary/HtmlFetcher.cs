using System;
using System.Net;
using System.Threading.Tasks;

namespace HtmlLibrary
{
    public static class HtmlFetcher
    {
        public static async Task<string> GetHtmlAsync(string url)
        {
            using (WebClient client = new WebClient())
            {
                return await client.DownloadStringTaskAsync(new Uri(url));
            }
        }
    }
}
