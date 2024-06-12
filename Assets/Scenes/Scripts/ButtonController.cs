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

public class ButtonController : MonoBehaviour
{
    private AudioSource AudioSource;  

    [SerializeField] private AudioClip ClipButtonPressed;

    [SerializeField] private LocalizationManager localization;

    [SerializeField] private AudioClip SantaLaugth;

    public Button ButtonPlay;   

    public List<Toggle> toggles = new List<Toggle>(3); 

    private static InputField InputTextName;

    [SerializeField] private Fb fb;

    [SerializeField] private Text TextWarning;

    private Transform Notification;

    private void Awake()
    {
        Events.MusicClick.AddListener(PlayMusicButon);

        transform.GetChild(0).GetComponent<AudioSource>().volume = Events.MusicForce;

        AudioSource = GetComponent<AudioSource>();
 
       

        if (InputTextName == null)
        {
        
            InputTextName = GameObject.FindGameObjectWithTag("Input")?.GetComponent<InputField>();

            if(InputTextName!=null)
            InputTextName.text = PlayerPrefs.GetString("Name");//вывод имени после инициализации
        }

        Notification = TextWarning?.transform.parent;



        ChangeTogleButtons(-1);


    }

   
    public void ChangeTogleButtons(int Index)
    {
         if(Index!=-1)
        GetCountSwithedTogles(Index);

        Events.MusicClick.Invoke(ClipButtonPressed);
        try
        {
            if (Events.IndexesActived.Count() < 1 || Fb.MyName.Length <= 4)
            {
                ButtonPlay.interactable = false;
            }
            else
            {
                ButtonPlay.interactable = true;
            }
        }

        catch (Exception ex)
        {

            ex.ToString();
        
        }
       
    }
    private int GetCountSwithedTogles(int index) 
    {

        switch (index)
        {
            case 0:
                if (!toggles[0].isOn && toggles[0]!=null)
                {
                    Events.IndexesActived.Remove(0);
                
                     
                }
                else
                {
                    Events.IndexesActived.Add(0);
                    

                }

                return Events.IndexesActived.Count();


            case 1:
                if (!toggles[1].isOn && toggles[1] != null)
                {
                    Events.IndexesActived.Remove(1);
                    
                }
                else
                {
                     Events.IndexesActived.Add(1);
                     
                }
                return Events.IndexesActived.Count();



            case 2:

                if (!toggles[2].isOn && toggles[2] != null)
                {
                    Events.IndexesActived.Remove(2);
                     
                }
                else
                {
                    Events.IndexesActived.Add(2);
                     

                }
                return Events.IndexesActived.Count();

            default:
                return Events.IndexesActived.Count() ;







        }

    }
    public  async void LoadScene(string scene)
    {
        Events.MusicClick.Invoke(ClipButtonPressed);

        AsyncOperation AsyncOperation = SceneManager.LoadSceneAsync(scene);


        while (AsyncOperation.isDone == false)
        {

            await Task.Yield();
        }
        

        if (scene.Equals("MainMenu"))
        {
            
            ButtonPlay = GameObject.FindGameObjectWithTag("ButtonPlay").GetComponent<Button>();

            ButtonPlay.interactable = false;

            Events.IndexesActived.Clear();
        }

     
      
 

    }

    private  void PlayMusicButon(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.Play();
        
    }

    [Obsolete]
    public void InputName()
    {

       
        Fb.MyName = InputTextName.text;

       StartCoroutine( Events.ChechInternetConnection((connection) =>
        {
            if (connection.Equals(true))
            {

                if (Fb.MyName.Length > 4)// добавлена проверка количества символов
                {


                    if (fb.CheckData(Fb.MyName) == false)
                    {
                        StartCoroutine(fb.WriteData(Fb.MyName, PlayerPrefs.GetInt("Max")));

                        ChangeTogleButtons(-1);//Меняем состояние кнопки играть

                        fb.RemoveData(PlayerPrefs.GetString("Name"));//удаляем старые данные из базы

                        PlayerPrefs.SetString("Name", Fb.MyName); // сохраним новые 


                        PopupWarning("Name updated!", Color.green);

                    }
                    else
                    {

                        PopupWarning("This name is taken!", Color.red);
                    }



                }

                else {
                    ChangeTogleButtons(-1);

                    PopupWarning("Fill in the name cell!", Color.red);
                }



            }

            else PopupWarning("No internet connection!", Color.red);



        }));

       

    }
    public void PopupWarning(string text,Color color,float AnimNotification = 1.5f)
    {
        
            text = localization.GetLocalizedValue(text);
            localization.OnLanguageChanged.Invoke();
        

        

       Tween SequencePopup = DOTween.Sequence()
                .Append(Notification.DOLocalMoveX(150f, AnimNotification))
                .AppendInterval(AnimNotification)
                .Append(Notification.DOLocalMoveX(2000f, AnimNotification));

        TextWarning.text = text;

        TextWarning.color = color;

        SequencePopup.Play();

    }
    
}

 
public static class Events
{
    public static UnityEvent<AudioClip> MusicClick = new  UnityEvent<AudioClip>();

    public static List<Task> AllTasks = new List<Task>();

    public static float MusicForce = 0;

   


    public static List<short> IndexesActived = new List<short>();


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

 