using InfinityHeros.News.Framework;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace InfinityHeros.News.Steam
{
    public readonly struct SteamNewsItem : INewsItem
    {
        public readonly string Author { get; }
        public readonly string Title { get; }
        public readonly string ImageURL { get; }
        public readonly DateTime Date { get; }
        public readonly string Contents { get; }
        public readonly IArticleSource ArticleSource { get; }

        [JsonConstructor]
        public SteamNewsItem(string title, long date, string contents, string url, string author)
        {
            Author = author;
            Title = title;
            Date = DateTimeOffset.FromUnixTimeSeconds(date).DateTime;
            ArticleSource = new SteamArticleSource(url);

            if(TryParseClanImageURL(contents, out string clanImageURL)&& clanImageURL.Length<= contents.Length)
            {
                Contents = contents[clanImageURL.Length..];
                ImageURL = ConvertClanImageToURL(clanImageURL);
            }
            else
            {
                ImageURL = BACKUP_URL;
                Contents = contents;
            }
            Contents = Contents.Trim();
        }

        static bool TryParseClanImageURL( string imageSource, out string clanImageURL)
        {
            m_Regex ??= new Regex(CLAN_PATTERN);
            Match match = Regex.Match(imageSource, CLAN_PATTERN);
            if(match.Success)
            {
                clanImageURL = match.Value;
                return true;
            }
            else
            {
                clanImageURL = string.Empty;
                return false;
            }
        }

        static string ConvertClanImageToURL(string clanImageURL)
        {
            return clanImageURL.Replace("{STEAM_CLAN_IMAGE}", CLAN_CDC);
        }

        const string BACKUP_URL = "https://clan.akamai.steamstatic.com/images/5622060/d2cacaaf1eabcf5e84348737e3f0ff60f2470315.png";
        const string CLAN_CDC = "https://clan.akamai.steamstatic.com/images//";
        const string CLAN_PATTERN = @"\{STEAM_CLAN_IMAGE\}/\d+/[a-f0-9]+\.png";
        static Regex m_Regex;
    }
}
