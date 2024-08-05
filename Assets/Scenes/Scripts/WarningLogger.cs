using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.UI;

public class WarningLogger : MonoBehaviour
{
    [SerializeField] private TMP_Text [] warningTextList;

    protected  Dictionary<string, TMP_Text> warningTextDict;

    private bool isFirstPassRight = false, isSecondPassRight= false, isMailRight = false;
    private bool isAllDataRight  => isFirstPassRight && isSecondPassRight && isMailRight; // свойство, отвечающее за то, все ли данные в поле введены верно

    internal string pass;
    internal string mail;
    public Direction dir = Direction.To;
    [SerializeField]private Transform targetPosition = null;

    [SerializeField] private InputField ? mailField, passField, secondPassField;
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

         
                    if (digits.Where(predicate => pass.Contains(predicate)).ToArray().Length < minimalCountDigits)
                    {
                        DisplayWarning("Добавьте минимум три разных числа ", warningTextDict["ass1"], Color.red);
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
        await MoveWaringAnimationBoard(childrens);
    }
    private async Task MoveWaringAnimationBoard(Transform [] elements,float speed = 0.6f)
    {
        if (Events.AllTasks != null)
        {
            Task[] tasks = Events.AllTasks.ToArray();

            while (!Task.WhenAll(tasks).IsCompleted)
            { 
                await Task.Yield();
            }
        }
 
        float timeDelay = 0.2f;
        float timeOffset = 0.2f;
       
        foreach (Transform element in elements)
        {
            if (element != null)
            {
                Tween SequencePopup = DOTween.Sequence()
                     .AppendInterval(timeDelay)
                     .Append(element.DOMoveX(targetPosition.position.x*(int)dir, speed)).Play();
              timeDelay+=timeOffset;
            }

        }
        ChangeDirection();
        
    }
    private void ChangeDirection()
    {
        if (dir.Equals(Direction.To))
        {
            dir = Direction.Out;
            return;
        }

        if (dir.Equals(Direction.Out))
        {

            dir = Direction.To;
            targetPosition.position = new Vector3(Mathf.Abs(targetPosition.position.x), transform.position.y, transform.position.z);

            return;
        }

    }

    


}
