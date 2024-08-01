using Newtonsoft.Json;
using System.Text;

namespace InfinityHeros.News.Steam
{    
    public struct AppNews
    {
        [JsonConstructor]
        public AppNews(SteamNewsItem[] newsItems)
        {
            NewsItems = newsItems; 
        }
        public SteamNewsItem[] NewsItems { get; set; }

        public override readonly string ToString()
        {
            StringBuilder sb = new();
            sb.Append(base.ToString());
            foreach(SteamNewsItem item in NewsItems)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }
    }
}

