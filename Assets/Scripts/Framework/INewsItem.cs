using System;

namespace InfinityHeros.News.Framework
{    public interface INewsItem
    {
        public string Title { get; }
        public string ImageURL { get; }
        public DateTime Date { get; }
        public string Contents { get; }
        public IArticleSource ArticleSource { get; }
    }
}
