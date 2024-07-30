using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;
using DG.Tweening.Plugins.Core.PathCore;
using System.Linq;

public class LocalizationManager : MonoBehaviour
{

    private static Dictionary<string, string> LocalizedText;
    public  Action OnLanguageChanged;

    [SerializeField] private TextAsset [] LangFiles = new TextAsset[3];

    private Dictionary<string, TextAsset> DictFiles;

 
    public string CurrentLanguage


    {
        get { return PlayerPrefs.GetString("Language"); }

        set
        { 
            PlayerPrefs.SetString("Language", value);
           
        }
    }
   
    private void Start()
    {
        DictFiles = LangFiles.ToDictionary(key => key.name, value => value);


        OnLanguageChanged.Invoke();

        CurrentLanguage = SetDefaultLanguage();
      
        LoadLocalizedText(CurrentLanguage);
     


    }

    

    private string SetDefaultLanguage()
    {
        


        if (PlayerPrefs.HasKey("Language")) return CurrentLanguage;


        return "en";


    }
 

    public void LoadLocalizedText(string langName)

    {

        string deltajson;

     

//#if UNITY_ANDROID && !UNITY_EDITOR

//   filePath = System.IO.Path.Combine(Application.persistentDataPath,langName);
//        if (!File.Exists(filePath))
        
//        {
//            string fromPath = System.IO.Path.Combine(Application.streamingAssetsPath, langName);

//            WWW reader = new WWW(fromPath);
//            while (!reader.isDone) { }

//            File.WriteAllBytes(filePath, reader.bytes);

//        }

//#endif

        print(langName);
      

       


        if (DictFiles[langName]!=null)
        {
            deltajson = DictFiles[langName].text;
 
            print("File exist");
            LocalizedText = new Dictionary<string, string>();
  
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

