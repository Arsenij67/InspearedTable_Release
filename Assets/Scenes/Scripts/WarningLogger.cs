using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.UI;

public class WarningLogger : MonoBehaviour
{
    [SerializeField] private TMP_Text [] warningTextList;

    protected  Dictionary<string, TMP_Text> warningTextDict;

    [SerializeField] private Fb fb;

    private bool isFirstPassRight = false, isSecondPassRight= false, isMailRight = false;
    private bool isAllDataRight  => isFirstPassRight && isSecondPassRight && isMailRight; // свойство, отвечающее за то, все ли данные в поле введены верно


    [SerializeField] private InputField ? mailField, passField, secondPassField;
    private void Awake()
    {

        warningTextDict = warningTextList.Where(selector => selector != null).ToDictionary(k=>k.name.Substring(k.name.Length-4),e=>e);
        // ключи: Mail, ass1, ass2

    }

    public void CheckFirstPass()
    {
        string pass = passField.text;
        const int countLetters = 26;
        if (!string.IsNullOrEmpty(pass))
        {
            char [] alphabetBig = Enumerable.Range('A', countLetters).Select(c => (char)c).ToArray();
            char [] alphabetSmall = Enumerable.Range('a', countLetters).Select(c => (char)c).ToArray();
            char[] digits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
            char[] alphabetLatin = alphabetBig.Union(alphabetSmall).Union(digits).ToArray();

            if (pass.Contains(' '))
            {
                DisplayWarning("Уберите пробелы", warningTextDict["ass1"], Color.red);

            }
            else
            {

                if (pass.All(lit => alphabetLatin.Contains(lit))) // если все символы это цифры и латинские буквы
                {


                    if (digits.Where(pred => pass.Contains(pred)).ToArray().Length < 3)
                    {

                        DisplayWarning("Добавьте минимум три числа "+ digits.Where(pred => pass.Contains(pred)).ToArray().Length, warningTextDict["ass1"], Color.red);
                    }
                    else
                    {

                        DisplayWarning("Пароль корректен", warningTextDict["ass1"], new Color(0.3f, 0.64f, 0.3f));
                        isFirstPassRight = true;
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
        string mail = mailField.text;

        string[] listDomens = new string [] { "@gmail.com", "@yahoo.com", "@outlook.com", "@hotmail.com", "@icloud.com", "@mail.ru", "@yandex.ru"};

        TMP_Text mailText = warningTextDict["Mail"];

        if (!string.IsNullOrEmpty(mail)) // если что - то написано
        {
            if (listDomens.Any(predicate: domen => (mail.Contains(domen))))
            {
                DisplayWarning("Почта введена корректно: ",mailText, new Color(0.3f, 0.64f, 0.3f));
                isMailRight = true;
              
            }
            else
            {
                DisplayWarning("Почта введена НЕ корректно:",mailText, Color.red);
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
    }


}
