using InfinityHeros.News.Steam;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;

namespace InfinityHeros.News.Framework
{
    public class NewsClient
    {
        UnityWebRequest m_CurrentRequest;
        const int ARTICLES_COUNT = 3;
        const int STEAM_APP_ID = 257730;

        public IEnumerator GetArticles(Action<SteamNewsResponse> callback)
        {
            string url = $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={STEAM_APP_ID}&count={ARTICLES_COUNT}";
            Debug.Log($"Requesting articles from: {url}");
                        
            m_CurrentRequest?.Abort();
            m_CurrentRequest = UnityWebRequest.Get(url);

            using (m_CurrentRequest)
            {
                yield return m_CurrentRequest.SendWebRequest(); //Wait for the request to complete
                Debug.Log(m_CurrentRequest.result);

                if (m_CurrentRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error fetching articles: {m_CurrentRequest.error}");
                    callback(new SteamNewsResponse(m_CurrentRequest.error));
                    m_CurrentRequest = null; //Clear the current request after completion
                    yield break;
                }

                string responseBody = m_CurrentRequest.downloadHandler.text;
                if(string.IsNullOrEmpty(responseBody))
                {
                    Debug.LogError("Error: response body is null or empty");
                    callback(new SteamNewsResponse("Response body is null or empty"));
                    m_CurrentRequest = null; //Clear the current request after completion
                    yield break;
                }
                Debug.Log($"Response Body: {responseBody}");

                try
                {
                    SteamNewsResponse steamResponse = JsonConvert.DeserializeObject<SteamNewsResponse>(responseBody);
                    if (steamResponse == null)
                    {
                        Debug.LogError("Error: deserialized SteamNewsResponse is null");
                        callback(new SteamNewsResponse("Deserialized response is null"));
                        m_CurrentRequest = null; //Clear the current request after completion
                        yield break;
                    }
                    callback(steamResponse);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception during JSON dersericaliztion: {ex.Message}");
                    callback(new SteamNewsResponse(ex.Message));
                }
                m_CurrentRequest = null; //Clear the current request after completion
            }
        }        
    }
}

