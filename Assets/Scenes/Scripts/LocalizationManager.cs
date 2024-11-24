using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using TMPro;

public class LocalizationManager : MonoBehaviour
{
    private static Dictionary<string, string> LocalizedText = new Dictionary<string, string>();
    public static Action OnLanguageChanged;
    public static Action OnResponseChanged;
    public static Action<string> OnEndResponse;
    [SerializeField]private TextAsset [] LangFiles = new TextAsset[3];
    private Dictionary<string, TextAsset> DictFiles;
    private const string startLanguage = "en";
    public string CurrentLanguage

    {
        get { return (string)SaveTypesFactory.deviceSaveManagerString.GetElement("Language"); }

        set
        { 
            SaveTypesFactory.deviceSaveManagerString.SaveElement("Language", value);
           
        }
    }


    private void Awake()
    {
        
        
        DictFiles = LangFiles.ToDictionary(key => key.name, value => value);
  
    }

    private void Start()
    {
       

            CurrentLanguage = SetDefaultLanguage();

            LoadLocalizedText(CurrentLanguage);

            OnLanguageChanged?.Invoke();
       
    }

    

    private string SetDefaultLanguage()
    {
       
     string currentLanguage = SaveTypesFactory.deviceSaveManagerString.GetElement("Language") as string;
     if (!string.IsNullOrEmpty(currentLanguage)) return CurrentLanguage;
         
        return startLanguage;
    }
 

    public void LoadLocalizedText(string langName)
    {
            string deltajson;

            LocalizedText.Clear();
            if (DictFiles[langName] != null)
            {
                deltajson = DictFiles[langName].text;

                var loadedData = JsonConvert.DeserializeObject<LocalizationData[]>(deltajson);

                for (int i = 0; i < loadedData.Length; i++)
                {
                    LocalizedText.Add(loadedData[i].key, loadedData[i].value);
                }
                CurrentLanguage = langName;
                print("File read!");

            }

            else { print("File not exist!"); }
        }
      
    
    

    public string GetLocalizedValue(string key)
    {
        
            if (LocalizedText.ContainsKey(key))
            {
                return LocalizedText[key];
            }

            else
            {
                Debug.Log($"Localithation key {key} haven't key!");
                return key;

            }
       
 

    }



}

