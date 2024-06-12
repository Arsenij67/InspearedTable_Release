using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocaledText : MonoBehaviour
{
    [SerializeField] protected string key;
    protected LocalizationManager localization;
    protected Text text;

    private void Awake()
    {
         

        if (localization == null)
        {

            localization = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();

        }

        localization.OnLanguageChanged += UpdateText;

        Debug.Log(localization.OnLanguageChanged.Method);

        if (text == null)
        { 
        text = GetComponent<Text>();
        
        }

        
    }

  


    public virtual void UpdateText()
    {
        try
        {

            if (gameObject == null) return;




            else if (key == "")
            {

                text.text = localization.GetLocalizedValue(text.text);

            }

            else
            {

                text.text = localization.GetLocalizedValue(key);
            }
        }
        catch (NullReferenceException ex)
        {
            print(transform.name);
        
        }
    
    }


    private void OnDestroy()
    {

        localization.OnLanguageChanged -= UpdateText;
    
    
    }

}
