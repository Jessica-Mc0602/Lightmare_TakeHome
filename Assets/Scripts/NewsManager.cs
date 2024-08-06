using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using InfinityHeros.News.Framework;
using InfinityHeros.News.Steam;
using InfinityHeros.News;
using System.Text.RegularExpressions;

public class NewsManager : MonoBehaviour
{
    public Button downloadButton; // Button to trigger article download
    public Button clearButton; // Button to clear articles
    public GameObject newsArticlePrefab; // Prefab for a news article
    public Transform newsContainer; // Container to hold news article prefabs
    
    private NewsClient newsClient; // Instance of NewsClient to fetch articles
    void Start()
    {
        newsClient = new NewsClient();
        downloadButton.onClick.AddListener(OnDownloadButtonClicked);
        clearButton.onClick.AddListener(ClearArticles);
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

                var imageTransform = articleGO.transform.Find("Image");
                if (imageTransform == null) Debug.LogError("Image child not found");

                var contentsTransform = articleGO.transform.Find("Contents");
                if (contentsTransform == null) Debug.LogError("Contents child not found");

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

                if (contentsTransform != null)
                {
                    var contentsComponent = contentsTransform.GetComponent<TextMeshProUGUI>();
                    if (contentsComponent != null)
                    {
                        //Clean HTML content
                        var cleanedContent = CleanHtmlContent(article.Contents);
                        // Split the content into lines, skip the first line, and join the rest
                        var contentLines = cleanedContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        if(contentLines.Length >1)
                        {
                            var modifiedContents = string.Join("\n", contentLines, 1, contentLines.Length - 1);
                            contentsComponent.text = modifiedContents;
                        }
                        else
                        {
                            contentsComponent.text = cleanedContent; // Fallback if there's only one line
                        }
                    }
                    else
                    {
                        Debug.LogError("TextMeshProUGUI component not found on Contents child");
                    }
                }
                articleGO.GetComponent<Button>().onClick.AddListener(() => article.ArticleSource.Open());
            }));
        }
    }


    string CleanHtmlContent(string content)
    {
        // Replace custom tags with TextMeshPro equivalents
        content = Regex.Replace(content, @"\[b\]", "<b>");
        content = Regex.Replace(content, @"\[/b\]", "</b>");
        content = Regex.Replace(content, @"\[i\]", "<i>");
        content = Regex.Replace(content, @"\[/i\]", "</i>");
        content = Regex.Replace(content, @"\[h1\]", "<size=32>");
        content = Regex.Replace(content, @"\[/h1\]", "</size>");
        content = Regex.Replace(content, @"\[h2\]", "<size=28>");
        content = Regex.Replace(content, @"\[/h2\]", "</size>");
        content = Regex.Replace(content, @"\[h3\]", "<size=26>");
        content = Regex.Replace(content, @"\[/h3\]", "</size>");

        // Replace custom URL tags with TextMeshPro link tags
        content = Regex.Replace(content, @"\[url=(.*?)\]", "<link=\"$1\">");
        content = Regex.Replace(content, @"\[/url\]", "</link>");

        // Remove remaining custom tags
        content = Regex.Replace(content, @"\[.*?\]", string.Empty);

        return content;
    }
    void ClearArticles()
    {
        foreach (Transform child in newsContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
