using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScenesLoader))]
public class WarningLogger : DashboardAnimator, IAuthorizationListener
{
    //??????????

    [SerializeField] private TMP_Text [] warningTextList;
    protected  Dictionary<string, TMP_Text> warningTextDict;
    [SerializeField]private bool isFirstPassRight = false, isSecondPassRight= false, isMailRight = false;
    internal string pass;
    internal string mail;
    [SerializeField] private TMP_InputField ? mailField, passField, secondPassField;
    public Button actionButton;
    private ScenesLoader sceneLoader;
    // ????????
    string IAuthorizationListener.mail => mail;

    string IAuthorizationListener.pass => pass;

    bool IAuthorizationListener.isAllDataRight => isFirstPassRight && isSecondPassRight && isMailRight;    // ????????, ?????????? ?? ??, ??? ?? ?????? ? ???? ??????? ?????

    private void Awake()
    {
        sceneLoader = GetComponent<ScenesLoader>();
        warningTextDict = warningTextList.Where(selector => selector != null).ToDictionary(k=>k.name.Substring(k.name.Length-4),e=>e);
        // ?????: Mail, ass1, ass2
     
    }

/// <summary>
/// ????????? ???????????? ????? ??????? ?????? ? ?????? ????? ?????????? ???????? ?? ?????
/// </summary>
    public void CheckFirstPass()
    {
        pass = passField.text;
        const int countLetters = 26;
        const int minimalCountDigits = 3;
        const int minCountLiterals = 3;
        if (!string.IsNullOrEmpty(pass))
        {
            char [] alphabetBig = Enumerable.Range('A', countLetters).Select(c => (char)c).ToArray();
            char [] alphabetSmall = Enumerable.Range('a', countLetters).Select(c => (char)c).ToArray();
            char[] digits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
            char[] allSymbols = alphabetBig.Union(alphabetSmall).Union(digits).ToArray();
            char[] allLiterals = alphabetSmall.Union(alphabetBig).ToArray();
            if (pass.Contains(' '))
            {
                DisplayWarning("Remove the spaces", warningTextDict["ass1"], Color.red);

            }
            else
            {
                if (pass.All(lit => allSymbols.Contains(lit))) // ???? ??? ??????? ??? ????? ? ????????? ?????
                {

         
                    if (digits.Where(predicate => pass.Contains(predicate)).ToArray().Length < minimalCountDigits)
                    {
                        DisplayWarning("Enter at least 3 different digits ", warningTextDict["ass1"], Color.red);
                    }
                    else
                    {

                        if (allLiterals.Where(symbol => pass.Contains(symbol)).ToArray().Length >= minCountLiterals)
                        {

                            DisplayWarning("Password is correct!", warningTextDict["ass1"], new Color(0.3f, 0.64f, 0.3f));
                            isFirstPassRight = true;
                        }

                        else 
                        {
                            DisplayWarning("Enter at least 3 different literals", warningTextDict["ass1"],Color.red);
                        }
                    }

                }

                else
                {

                    DisplayWarning("Enter at least 3 different literals and digits", warningTextDict["ass1"], Color.red);

                }
            }

        }
        else
        {
            DisplayWarning("Password is empty", warningTextDict["ass1"], Color.red);
        }

       
    }
/// <summary>
/// ????????? ???????????? ?????  ??????? ?????? ? ?????? ????? ?????????? ???????? ?? ?????
/// </summary>
    public void CheckSecondPass()
    {
        string pass2 = secondPassField.text;
        string pass1 = passField.text;
        if (!string.IsNullOrEmpty(pass2))
        {
            if (pass2.Equals(pass1))
            {
                DisplayWarning("Password is match", warningTextDict["ass2"], new Color(0.3f, 0.64f, 0.3f));
                isSecondPassRight = true;
            }

            else
            {
                DisplayWarning("Passwords don't match!", warningTextDict["ass2"], Color.red);
            }
        }

        else
        {
            DisplayWarning("Password is empty!", warningTextDict["ass2"], Color.red);
        }
       
    }

/// <summary>
/// ????????? ???????????? ????? ????? ? ?????? ????? ?????????? ???????? ?? ?????
/// </summary>
    public void CheckMail()
    {
        mail = mailField.text;

        string[] listDomens = new string [] { "@gmail.com", "@yahoo.com", "@outlook.com", "@hotmail.com", "@icloud.com", "@mail.ru", "@yandex.ru","@informatics.ru"};

        TMP_Text mailText = warningTextDict["Mail"];

        if (!string.IsNullOrEmpty(mail)) // ???? ??? - ?? ????????
        {
            var res =  listDomens.Where(selector => mail.Contains(selector)).FirstOrDefault();
            if (!string.IsNullOrEmpty(res) && mail.EndsWith(res))
            {
                DisplayWarning("Mail is correct!", mailText, new Color(0.3f, 0.64f, 0.3f));
                isMailRight = true;
            }

            else
            {
                DisplayWarning("Mail must have domain", mailText, Color.red);
            }


        }
        else
        {
            DisplayWarning("Enter the mail!",mailText,Color.red);  
        }

       
    }

    private void DisplayWarning(string message,TMP_Text field,Color color)
    { 
       
        field.color = color;
        field.GetComponent<LocaledText>().UpdateText(message);
       
    }

/// <summary>
/// ???????? ?? ??????????? ? ?????????? ?????
/// </summary>
    public async void MoveWarningAnimationBoard()
    {
        Transform[] childrens = new Transform[20];
        int i = 0;
        for (; i < transform.GetChild(0).childCount; i++)
        {
                childrens[i] = transform.GetChild(0).GetChild(i);
        }
        await MoveQueuedAnimationBoard(childrens);
    }

    public void OnRegisterMail()
    {

        DisplayGrowingLoadingPanel("\nTo register, follow the link in the email! ");
        
    }

    public void OnVerifiedMail()
    {
        CloseGrowingLoadingPanel("\nYou have successfully verified your account! ");
       
    }

    public void OnAuthorizationFailed(AggregateException error)
    {
        DisplayGrowingLoadingPanel(error.Message);
       

    }

    public void CloseLoadingPanel()
    {
        CloseGrowingLoadingPanel();
        
    }

    public async void OnLogInSucceeded()
    {
        
        await DisplayGrowingLoadingPanel(string.Format($"The login was completed successfully!"),2f);
        sceneLoader.LoadScene();
       
    }


}
