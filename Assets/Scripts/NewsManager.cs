using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using InfinityHeros.News.Framework;
using InfinityHeros.News.Steam;
using InfinityHeros.News;

public class NewsManager : MonoBehaviour
{
    public Button downloadButton;
    public GameObject newsArticlePrefab;
    public Transform newsContainer;

    private NewsClient newsClient;
    void Start()
    {
        newsClient = new NewsClient();
        downloadButton.onClick.AddListener(OnDownloadButtonClicked);
    }

    void OnDownloadButtonClicked()
    {
        StartCoroutine(newsClient.GetArticles(OnArticlesDownloaded));
    }
    
    void OnArticlesDownloaded(SteamNewsResponse response)
    {
        if(response.IsError)
        {
            Debug.LogError("Failed to download articles: " + response.ErrorMessage);
            return;
        }
        foreach(var article in response.NewsItems)
        {
            StartCoroutine(TextureStreamer.ReadFromURL(article.ImageURL, (texture) =>
            {
                GameObject articleGO = Instantiate(newsArticlePrefab, newsContainer);
                articleGO.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = article.Title;
                articleGO.transform.Find("Contents").GetComponent<TextMeshProUGUI>().text = article.Contents;
                articleGO.transform.Find("Image").GetComponent<RawImage>().texture = texture;
                articleGO.GetComponent<Button>().onClick.AddListener(() => article.ArticleSource.Open());
            }));
        }
    }
}
