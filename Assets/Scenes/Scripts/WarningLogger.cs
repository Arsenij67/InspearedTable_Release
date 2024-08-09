using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WarningLogger : DashboardAnimator,IAuthorizationListener
{
    //переменные

    [SerializeField] private TMP_Text [] warningTextList;
    protected  Dictionary<string, TMP_Text> warningTextDict;
    [SerializeField]private bool isFirstPassRight = false, isSecondPassRight= false, isMailRight = false;
    internal string pass;
    internal string mail;
    [SerializeField] private InputField ? mailField, passField, secondPassField;
    public Button actionButton;
    [SerializeField] private LocalizationManager localizationManager;
    // свойства
    string IAuthorizationListener.mail => mail;

    string IAuthorizationListener.pass => pass;

    bool IAuthorizationListener.isAllDataRight => isFirstPassRight && isSecondPassRight && isMailRight;    // свойство, отвечающее за то, все ли данные в поле введены верно

    private void Awake()
    {
        warningTextDict = warningTextList.Where(selector => selector != null).ToDictionary(k=>k.name.Substring(k.name.Length-4),e=>e);
        // ключи: Mail, ass1, ass2
     
    }

/// <summary>
/// Проверяет правильность ввода второго пароля и решает какую информацию выводить на экран
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
                DisplayWarning("Уберите пробелы", warningTextDict["ass1"], Color.red);

            }
            else
            {
                if (pass.All(lit => allSymbols.Contains(lit))) // если все символы это цифры и латинские буквы
                {

         
                    if (digits.Where(predicate => pass.Contains(predicate)).ToArray().Length < minimalCountDigits)
                    {
                        DisplayWarning("Добавьте минимум три разных числа ", warningTextDict["ass1"], Color.red);
                    }
                    else
                    {

                        if (allLiterals.Where(symbol => pass.Contains(symbol)).ToArray().Length >= minCountLiterals)
                        {

                            DisplayWarning("Пароль корректен", warningTextDict["ass1"], new Color(0.3f, 0.64f, 0.3f));
                            isFirstPassRight = true;
                        }

                        else 
                        {
                            DisplayWarning("Добавьте минимум три буквы", warningTextDict["ass1"],Color.red);
                        }
                    }

                }

                else
                {

                    DisplayWarning("Используйте только латинские буквы и цифры", warningTextDict["ass1"], Color.red);

                }
            }

        }
        else
        {
            DisplayWarning("Вы не ввели пароль!", warningTextDict["ass1"], Color.red);
        }

    }
/// <summary>
/// Проверяет правильность ввода  первого пароля и решает какую информацию выводить на экран
/// </summary>
    public void CheckSecondPass()
    {
        string pass2 = secondPassField.text;
        string pass1 = passField.text;
        if (!string.IsNullOrEmpty(pass2))
        {
            if (pass2.Equals(pass1))
            {
                DisplayWarning("Пароли совпадают", warningTextDict["ass2"], new Color(0.3f, 0.64f, 0.3f));
                isSecondPassRight = true;
            }

            else
            {
                DisplayWarning("Пароли не совпадают!", warningTextDict["ass2"], Color.red);
            }
        }

        else
        {
            DisplayWarning("Вы не продублировали пароль!", warningTextDict["ass2"], Color.red);
        }

    }

/// <summary>
/// Проверяет правильность ввода почты и решает какую информацию выводить на экран
/// </summary>
    public void CheckMail()
    {
        mail = mailField.text;

        string[] listDomens = new string [] { "@gmail.com", "@yahoo.com", "@outlook.com", "@hotmail.com", "@icloud.com", "@mail.ru", "@yandex.ru"};

        TMP_Text mailText = warningTextDict["Mail"];

        if (!string.IsNullOrEmpty(mail)) // если что - то написано
        {
            var res =  listDomens.Where(selector => mail.Contains(selector)).FirstOrDefault();
            if (!string.IsNullOrEmpty(res) && mail.EndsWith(res))
            {
                DisplayWarning("Почта введена корректно: ", mailText, new Color(0.3f, 0.64f, 0.3f));
                isMailRight = true;
            }

            else
            {
                DisplayWarning("Почта введена НЕ корректно:", mailText, Color.red);
            }


        }
        else
        {
            DisplayWarning("Вы не ввели почту!",mailText,Color.red);  
        }
    }

    private void DisplayWarning(string message,TMP_Text field,Color color)
    { 
        field.color = color;
        field.text = message;
        localizationManager?.OnLanguageChanged?.Invoke();
    }

/// <summary>
/// Отвечает за перемещение с анимациями доски
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

        DisplayGrowingLoadingPanel("Письмо отправлено на почту. \r\nПодтвердите его, перейдя по ссылке в письме!");
        localizationManager.OnLanguageChanged();
    }

    public void OnVerifiedMail()
    {
        CloseGrowingLoadingPanel();
        localizationManager.OnLanguageChanged();
    }

    public void OnAuthorizationFailed(AggregateException error)
    {
        DisplayGrowingLoadingPanel(error.Message);
        localizationManager.OnLanguageChanged();
        
    }

    public void CloseLoadingPanel()
    {
        CloseGrowingLoadingPanel();
        localizationManager.OnLanguageChanged();
    }

    public void OnLogInSucceeded()
    {
        DisplayGrowingLoadingPanel(string.Format($"Вы успешно зашли в аккаунт!"));
        localizationManager.OnLanguageChanged();
    }


}
