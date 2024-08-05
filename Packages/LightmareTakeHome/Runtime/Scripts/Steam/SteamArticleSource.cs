using InfinityHeros.News.Framework;
using UnityEngine;

namespace InfinityHeros.News.Steam
{
    public readonly struct SteamArticleSource : IArticleSource
    {
        public readonly string URL { get; }
        public SteamArticleSource(string url)
        {
            URL = url;
        }

        public void Open()
        {
            Application.OpenURL(URL);
        }
    }
}


