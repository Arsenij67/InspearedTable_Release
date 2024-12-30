using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    //??????????
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

    private LocaledText localedTextNotif;

    public enum sceneName
    {
        Authorization,
        GameAndroid,
        MainMenuAndroid,
    }

    private void Awake()
    {
        notification = textWarning?.transform.parent;
        localedTextNotif = textWarning.GetComponent<LocaledText>();
        Events.MusicClick.AddListener(PlayMusicGame);
        transform.GetChild(0).GetComponent<AudioSource>().volume = Events.MusicForce;
        AudioSource = GetComponent<AudioSource>();
        if (inputTextName == null)
        {

            if(inputTextName!=null)
            inputTextName.text = (string)SaveTypesFactory.deviceSaveManagerString.GetElement("Name");//????? ????? ????? ?????????????
        }
       
        SetDefaultName();
        TryToPlay();

        if (buttonPlay)
        {
            buttonPlay.interactable = false;
        }
        Events.indexesActived.Clear();
        CheckFirstEnter();

    }
 

    private void OnEnable()
    {
       LocalizationManager.OnLanguageChanged+= localedTextNotif.UpdateText;
       LocalizationManager.OnResponseChanged+= localedTextNotif.UpdateText;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= localedTextNotif.UpdateText;
        LocalizationManager.OnResponseChanged -= localedTextNotif.UpdateText;
    }
    void CheckFirstEnter()
    {
        float animLength = 3f;
        if (IsUserFirstEnter())
        {
            PopupWarning("Change your name at the top to find yourself on the leaderboard", Color.yellow, animLength);
           
        }
        IncreaseQuantityEnter();

    }

    public void TryToPlay() // ????????? ??????????? ? ????
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
       
    }
    public void ChangeTogleState(int index) // ????????? ????? ? ????
    {
        if (toggles[index] != null && !toggles[index].isOn) // ???? ???? ????????, ?? ??????? ??? ?????? ?? ?????? ????????
        {
            Events.indexesActived.Remove((short)index);
        }

        else if (toggles[index] != null && toggles[index].isOn) 
        {
            Events.indexesActived.Add((short)index);
        }
        TryToPlay();
    }
    public static async void LoadScene(sceneName sceneName)
    {

        AsyncOperation AsyncOperation = SceneManager.LoadSceneAsync(sceneName.ToString());

        while (AsyncOperation.isDone == false)
        {
            await Task.Yield();
        }

    }

    private void PlayMusicGame(AudioClip clip)
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

        })); // ???? ???? ????????, ?? ????????? ???????????

        string name = inputTextName.text;
         
        if (name.Length > 4)// ????????? ???????? ?????????? ????????
        {

            if (fb.CheckNameUser(name) == false)
            {
                
            
                fb.RemoveData(SaveTypesFactory.deviceSaveManagerString.GetElement("Name") as string);//??????? ?????? ?????? ?? ????
                 
                int maxScore = int.Parse(await fb.GetRecord());
                SaveTypesFactory.deviceSaveManagerString.SaveElement("Name", name); // ???????? ????? 
                
                StartCoroutine(fb.WriteData(SaveTypesFactory.deviceSaveManagerString.GetElement("Name").ToString(),maxScore,fb.auth.CurrentUser.UserId));

                TryToPlay();//?????? ????????? ?????? ??????

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
        textWarning.text = text;

        textWarning.color = color;

        float animTo = 2f;
        float animBack =  2f;

        LocalizationManager.OnResponseChanged.Invoke();

        Tween SequencePopup = DOTween.Sequence()
                .Append(notification.DOLocalMoveX(110f, animTo))
                .AppendInterval(animNotification)
                .Append(notification.DOLocalMoveX(2000f, animBack));

      

        SequencePopup.Play();

    }

    private async void SetDefaultName()
    {
        string name = SaveTypesFactory.deviceSaveManagerString.GetElement("Name") as string;
       
        if (!string.IsNullOrEmpty(name))
        {
            await Task.Delay(100);
            if (inputTextName && fb.CheckNameUser(name))
            {
                inputTextName.text = name;
            }

        }
    }

    public void GoToMainMenu()
    {
        LoadScene(sceneName.MainMenuAndroid);
    }

    private bool IsUserFirstEnter()
    {
        uint numberEnter = Convert.ToUInt32(SaveTypesFactory.deviceSaveManagerInteger.GetElement("NumberEnter"));

        return numberEnter <= 0;
  
    }
    void IncreaseQuantityEnter()
    {
        uint numberEnter = Convert.ToUInt32(SaveTypesFactory.deviceSaveManagerInteger.GetElement("NumberEnter"));
        SaveTypesFactory.deviceSaveManagerInteger.SaveElement("NumberEnter", ++numberEnter);
         
    }

}


public static class Events
{
    public static UnityEvent<AudioClip> MusicClick = new UnityEvent<AudioClip>();

    public static List<Task> AllTasks = new List<Task>();

    public static float MusicForce = 0;

    public static List<short> indexesActived = new List<short>();

    public static short DroppedIndex;

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

 