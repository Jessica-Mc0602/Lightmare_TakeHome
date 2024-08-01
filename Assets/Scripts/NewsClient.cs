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
        public IEnumerator GetArticles(Action<SteamNewsResponse> callback)
        {
            SteamNewsRequest steamNewsRequest = new(STEAM_APP_ID, ARTICLES_COUNT);
            m_CurrentRequest?.Abort();
            using (m_CurrentRequest = UnityWebRequest.Get(steamNewsRequest))
            {
                yield return m_CurrentRequest.SendWebRequest();
                if(m_CurrentRequest.result != UnityWebRequest.Result.Success)
                {
                    callback(new(m_CurrentRequest.error));
                    yield break;
                }
                string responseBody = m_CurrentRequest.downloadHandler.text;
                SteamNewsResponse steamResponse = JsonConvert.DeserializeObject<SteamNewsResponse>(responseBody);
                callback(steamResponse);
            }
        }
        UnityWebRequest m_CurrentRequest;
        const int ARTICLES_COUNT = 3;
        const int STEAM_APP_ID = 257730;
    }
}

