using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public sealed class ScoreCaracter : MonoBehaviour
{
    public static ScoreCaracter Instance { get; private set; }

    [SerializeField] private TMP_Text  TextScoreUI;

    [SerializeField] private TMP_Text MaxScoreUI;

    [SerializeField] private LocaledText localedMaxScore;

    [SerializeField] private LocaledText localedScore;

    private Fb fb;

    private int _score;
    public int Score {
        get 
        {
            
            return _score;

        }

        set
        {
            _score = value;
            TextScoreUI.text = "Score: ";
            localedScore += _score;
            LocalizationManager.OnResponseChanged();

        }
    
    
  
    }

    private int _maxscore = 0;
    public static ScoreCaracter getInstance()
    { 
        if (Instance == null) return new ScoreCaracter();
        return new ScoreCaracter();
    }
    private ScoreCaracter()
    {
        Instance = this;
    }
    private async void Start()
    {
        fb = FindObjectOfType<Fb>();
        await fb.InitInfoTask; // ждем конец инициализации
        MaxScore = Convert.ToInt32(fb.dataSnapshot.Child(SaveTypesFactory.deviceSaveManagerString.GetElement("Name") as string).Child("Record").Value);
    }
    
    public int MaxScore
    {
        get { return _maxscore; }

        set
        {
            _maxscore = value;
          
            MaxScoreUI.text = ("Max Score: ");
            localedMaxScore +=_score;
            LocalizationManager.OnResponseChanged();



        }

    }

    /// <summary>
    /// Отнимает или прибавляет очки к текущему счёту и обновляет рекорд
    /// </summary>
    /// <param name="mark"></param>
    /// <param name="miliseconds">количество секунд на одну итерацию</param>
    /// <returns></returns>
    public IEnumerator ChangeScoreAndRecord(int mark,float miliseconds)
    {
        
        int iter = mark / 10;

        if (mark > 0)
        {
            while (mark > 0)
            {

                Score += iter;
                mark -= iter;

                yield return new WaitForSeconds(miliseconds / 1000);

            }
        }
        else
        {

            while (mark < 0)
            {

                Score += iter;
                mark -= iter;

                yield return new WaitForSeconds(miliseconds / 1000);

            }

        }
       yield return StartCoroutine(UpdateRecord(50));
    }
/// <summary>
/// Увеличивает рекорд на разницу между счётом и последним рекордом
/// </summary>
/// <param name="milliseconds"> время одного кадра  в миллисекундах</param>
/// <returns></returns>
private IEnumerator UpdateRecord(float milliseconds = 50)
    { 
        float mark = Score - MaxScore;
        float iter = mark / 10;

        if (mark > 0)
        {
            while (mark > 0)
            {

                MaxScore += (int) iter;
                mark -= iter;
                yield return new WaitForSeconds(milliseconds / 1000);

            }
        }
    }

   



}

   
 

