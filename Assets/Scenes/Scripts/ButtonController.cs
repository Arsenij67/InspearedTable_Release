using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    private AudioSource AudioSource;  

    [SerializeField] private AudioClip clipButtonPressed;

    [SerializeField] private LocalizationManager localization;

    [SerializeField] private AudioClip santaLaugth;

    public Button buttonPlay;   

    public List<Toggle> toggles = new List<Toggle>(3); 

    public TMP_InputField inputTextName;

    [SerializeField] private Fb fb;

    [SerializeField] private TMP_Text textWarning;

    private Transform notification;


    private void Awake()
    {
     
        Events.MusicClick.AddListener(PlayMusicGame);

        transform.GetChild(0).GetComponent<AudioSource>().volume = Events.MusicForce;

        AudioSource = GetComponent<AudioSource>();
 

        if (inputTextName == null)
        {

            if(inputTextName!=null)
            inputTextName.text = (string)SaveTypesFactory.deviceSaveManagerString.GetElement("Name");//вывод имени после инициализации
        }
        notification = textWarning?.transform.parent;
        SetDefaultName();
        TryToPlay();
         
    }

   
    public void TryToPlay() // проверяем доступность к игре
    {
        if (buttonPlay == null) return;
        
            if (Events.indexesActived.Count() < 1 || SaveTypesFactory.deviceSaveManagerString.GetElement("Name").ToString().Length <= 4)
            {
            
                buttonPlay.interactable = false;
            }
            else
            {
                buttonPlay.interactable = true;
            }
        print(Events.indexesActived.Count()+"---");
       
    }
    public void ChangeTogleState(int index) // изменение тогла в меню
    {
        if (toggles[index] != null && !toggles[index].isOn) // если тогл выключен, то удаляем его индекс из списка активных
        {
            Events.indexesActived.Remove((short)index);
        }

        else if (toggles[index] != null && toggles[index].isOn) 
        {
            Events.indexesActived.Add((short)index);
        }
        TryToPlay();
    }
    public  async void LoadScene(string scene)
    {
        Events.MusicClick.Invoke(clipButtonPressed);

        AsyncOperation AsyncOperation = SceneManager.LoadSceneAsync(scene);

        while (AsyncOperation.isDone == false)
        {
            await Task.Yield();
        }
        
        if (scene.StartsWith("MainMenu"))
        {
            
            buttonPlay = GameObject.FindGameObjectWithTag("ButtonPlay").GetComponent<Button>();

            buttonPlay.interactable = false;

            Events.indexesActived.Clear();
        }

    }

    private  void PlayMusicGame(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.Play();
        
    }


    public async void InputName()
    {

       StartCoroutine( Events.ChechInternetConnection((connection) =>
        {
            if (connection.Equals(false))
            {
                PopupWarning("No internet connection!", Color.red);
            } 
            return;

        })); // если есть интернет, то программа выполняется

        string name = inputTextName.text;
         
        if (name.Length > 4)// добавлена проверка количества символов
        {

            if (fb.CheckNameUser(name) == false)
            {
                
            
                fb.RemoveData(SaveTypesFactory.deviceSaveManagerInteger.GetElement("Name") as string);//удаляем старые данные из базы
                 
                int maxScore = int.Parse(await fb.GetRecord());
                SaveTypesFactory.deviceSaveManagerString.SaveElement("Name", name); // сохраним новые 

                StartCoroutine(fb.WriteData(SaveTypesFactory.deviceSaveManagerString.GetElement("Name").ToString(),maxScore));

                TryToPlay();//Меняем состояние кнопки играть

                PopupWarning("Name updated!", Color.green);

            }
            else
            {

                PopupWarning("This name is taken!", Color.red);
            }



        }

        else
        {
            TryToPlay();

            PopupWarning("Fill in the name cell!", Color.red);
        }



    }
    public void PopupWarning(string text,Color color,float animNotification = 1.5f)
    {
        
            text = localization.GetLocalizedValue(text);
            localization.OnLanguageChanged.Invoke();

       Tween SequencePopup = DOTween.Sequence()
                .Append(notification.DOLocalMoveX(150f, animNotification))
                .AppendInterval(animNotification)
                .Append(notification.DOLocalMoveX(2000f, animNotification));

        textWarning.text = text;

        textWarning.color = color;

        SequencePopup.Play();

    }

    private void SetDefaultName()
    {
        string name = SaveTypesFactory.deviceSaveManagerString.GetElement("Name") as string;

        if (!string.IsNullOrEmpty(name))
        {
            if (inputTextName)
            {
                inputTextName.text = name;
            }

        }
    }
    
}


public static class Events
{
    public static UnityEvent<AudioClip> MusicClick = new UnityEvent<AudioClip>();

    public static List<Task> AllTasks = new List<Task>();

    public static float MusicForce = 0;

    public static List<short> indexesActived = new List<short>();

    [Obsolete]
    public static IEnumerator ChechInternetConnection(Action<bool> connect)
    {

        foreach (var url in new string[] { "https://inf-ege.sdamgia.ru/problem?id=45241", "https://www.google.ru/", "https://ya.ru/" })
        {

            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();
            if (request.isNetworkError == false)
            {

                connect(true);

                yield break;

            }
            else connect(false);

        }


    }
}

 