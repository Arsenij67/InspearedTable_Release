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

           
                LocalizationManager.OnLanguageChanged += UpdateText;
                
          

        }
        

    }

    public virtual void UpdateText()
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
                           TranslateFromAPIAsync((string)SaveTypesFactory.deviceSaveManagerString.GetElement("Language"),text.text);
                       }
                   }
                   ));
            }
       

    }

    public virtual void UpdateText(string textToTranslate = "none")
    {
        if (translateMode.Equals(TranslateMode.LocalTranslate))
        {

            TranslateFromJson();

        }

        else if (translateMode.Equals(TranslateMode.APITranslate))
        {
            StartCoroutine(Events.ChechInternetConnection
                (connect =>
                {
                    if (connect.Equals(true))
                    {
                        TranslateFromAPIAsync((string)SaveTypesFactory.deviceSaveManagerString.GetElement("Language"), textToTranslate);
                    }
                }
                ));
        }


    }




    private void TranslateFromJson()
    {
        text.text = key == "" ? localization.GetLocalizedValue(text.text) : localization.GetLocalizedValue(key);
    }

    private async void TranslateFromAPIAsync(string lang, string text)
    {
        HttpClient client = new HttpClient();
        const string apiKey = "AQVN04q_jBXOeXRdy6nh2l24qhV3e1RNXfvam80V"; // ???? ?? API
        var url = "https://translate.api.cloud.yandex.net/translate/v2/translate";

        var requestBody = new
        {
            targetLanguageCode = lang, // ???? ????????
            texts = new[] { text}, // ??????????? ?????
            folderId = "b1gcs887qqjchlhcu03p" // ??? ?????

        };

        print(text);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Api-Key", apiKey);

        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonRequestBody,Encoding.UTF8,"application/json");
        var response =  await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TranslateResponse>(responseContent);
            this.text.text =  ReplaceUnreadableSymbols(result.Translations[0].Text);
        }

        else
        {
            this.text.text =  $": {response.StatusCode} - {response.ReasonPhrase}";
        }


    }

    private string ReplaceUnreadableSymbols(string translatedText)
    {
        // ???????? ??????????? ????? ?? ??????????
        return translatedText
            .Replace("à", "a")
            .Replace("â", "a")
            .Replace("ä", "a")
            .Replace("ç", "c")
            .Replace("é", "e")
            .Replace("è", "e")
            .Replace("ê", "e")
            .Replace("ë", "e")
            .Replace("î", "i")
            .Replace("ï", "i")
            .Replace("ô", "o")
            .Replace("ù", "u")
            .Replace("û", "u")
            .Replace("ü", "u")
            .Replace("ÿ", "y");

    }

    private void OnDestroy()
    {

        LocalizationManager.OnLanguageChanged -= UpdateText;
        LocalizationManager.OnResponseChanged -= UpdateText;


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

 

 