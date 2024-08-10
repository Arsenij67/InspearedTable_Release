using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using TMPro;
using System.Text;

public class LocaledText : MonoBehaviour
{
    [SerializeField] protected string key;
    protected LocalizationManager localization;
    protected TMP_Text text;
    [SerializeField] protected TranslateMode translateMode;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TMP_Text>();

        }

        if (localization == null)
        {

            localization = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();
            localization.OnLanguageChanged += UpdateText;
            localization.OnResponseChanged += UpdateText;
            Debug.Log(name);

        }
        

    }

    public virtual async void UpdateText()
    {

            if (translateMode.Equals(TranslateMode.LocalTranslate))
            {

                TranslateFromJson();

            }

            else if (translateMode.Equals(TranslateMode.APITranslate))
            {
               StartCoroutine( Events.ChechInternetConnection
                   (connect =>
                   {
                       if (connect.Equals(true))
                       {
                           TranslateFromAPIAsync(PlayerPrefs.GetString("Language"));
                       }
                   }
                   ));
            }
       

    }

    private void TranslateFromJson()
    {
        text.text = key == "" ? localization.GetLocalizedValue(text.text) : localization.GetLocalizedValue(key);
    }

    private async void TranslateFromAPIAsync(string lang)
    {
        HttpClient client = new HttpClient();
        const string apiKey = "AQVN1K5XuAZWmbj7SNck8xc1RiKWr27tYrSw5jUh"; // ключ от API
        var url = "https://translate.api.cloud.yandex.net/translate/v2/translate";

        var requestBody = new
        {
            targetLanguageCode = lang, // язык перевода
            texts = new[] { text.text }, // переводимый текст
            folderId = "b1g5gt63mkqi8qskjsu0" // имя папки

        };
     

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Api-Key", apiKey);

        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonRequestBody,Encoding.UTF8,"application/json");
        var response =  await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TranslateResponse>(responseContent);
           text.text =  result.Translations[0].Text;
        }

        else
        {
            text.text =  $"Ошибка при переводе: {response.StatusCode} - {response.ReasonPhrase}";
        }


    }

    private void OnDestroy()
    {

        localization.OnLanguageChanged -= UpdateText;


    }
    public struct TranslateRequest
    {
        public string TargetLanguageCode { get; set; }
        public string[] Texts { get; set; }
        public string FolderId { get; set; }
    }

    public struct Translation
    {
        public string Text { get; set; }
    }

    public struct TranslateResponse
    {
        public Translation[] Translations { get; set; }
    }

}

 

 