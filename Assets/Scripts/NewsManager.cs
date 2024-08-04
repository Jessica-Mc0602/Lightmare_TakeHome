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
    public Button downloadButton; // Button to trigger article download
    public GameObject newsArticlePrefab; // Prefab for a news article
    public Transform newsContainer; // Container to hold news article prefabs

    private NewsClient newsClient; // Instance of NewsClient to fetch articles
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
                Debug.Log("Instantiated article prefab");

                var titleTransform = articleGO.transform.Find("Title");
                if (titleTransform == null) Debug.LogError("Title child not found");

                var contentsTransform = articleGO.transform.Find("Contents");
                if (contentsTransform == null) Debug.LogError("Contents child not found");

                var imageTransform = articleGO.transform.Find("Image");
                if (imageTransform == null) Debug.LogError("Image child not found");

                if (titleTransform != null)
                {
                    var titleComponent = titleTransform.GetComponent<TextMeshProUGUI>();
                    if (titleComponent != null)
                    {
                        titleComponent.text = article.Title;
                    }
                    else
                    {
                        Debug.LogError("TextMeshProUGUI component not found on Title child");
                    }
                }

                if (contentsTransform != null)
                {
                    var contentsComponent = contentsTransform.GetComponent<TextMeshProUGUI>();
                    if (contentsComponent != null)
                    {
                        contentsComponent.text = article.Contents;
                    }
                    else
                    {
                        Debug.LogError("TextMeshProUGUI component not found on Contents child");
                    }
                }

                if (imageTransform != null)
                {
                    var imageComponent = imageTransform.GetComponent<RawImage>();
                    if (imageComponent != null)
                    {
                        imageComponent.texture = texture;
                    }
                    else
                    {
                        Debug.LogError("RawImage component not found on Image child");
                    }
                }

                articleGO.GetComponent<Button>().onClick.AddListener(() => article.ArticleSource.Open());
            }));
        }
    }
}
