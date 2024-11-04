using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ButtonSwithLang : MonoBehaviour
{
    [SerializeField] private LocalizationManager localization;

    [SerializeField] private AudioClip ClickMusic;

    private GameObject[] FlagsTransform = new GameObject[3];

 
    public async void OnButtonClick(string lang)
    {


       await  SellectFlagAnimAsync();
         

        localization.CurrentLanguage = lang;

        localization.LoadLocalizedText(localization.CurrentLanguage);

        LocalizationManager.OnLanguageChanged?.Invoke();
   
    
    }



    private async Task SellectFlagAnimAsync()
    {
        Events.MusicClick.Invoke(ClickMusic);

        FlagsTransform = GameObject.FindGameObjectsWithTag("Flags");


        IEnumerable<GameObject> DeactivedFlags = FlagsTransform.Where(gameObject => gameObject.name != transform.name);
 
        transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.7f).Play() ;

        foreach (GameObject flag in DeactivedFlags)
        {
           flag.GetComponent<Transform>().DOScale(Vector3.one, 0.7f).Play();
        }
        
    }

}
