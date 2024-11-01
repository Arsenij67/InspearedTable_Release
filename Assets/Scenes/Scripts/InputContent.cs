using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public abstract  class Content : InputContent
{


    public abstract string Name { get; }
    public  TMP_Text Info { get; set; }

    public string[] contents;
  
    public abstract TextAsset File { get; set; }
    public abstract int Price { get; set; }
    public abstract void ShowContent();

    public Image content;

    internal void SetTitleBoard(ref TMP_Text title)
    {
        title.text = Name;
     
    }


} 
public class Motivation : Content//2

{
    public override string Name => "Motivation";
    private static int NumberCont = 15;
    public override int Price { get; set; } = 1500;
    public override TextAsset File { get; set; }
    public Motivation()
    {

        File = (TextAsset)Resources.Load(path: "Content/Motivation"); //???????? ?????? ?? ????? 

        Info = GameObject.Find("Content").GetComponent<TMP_Text>(); // ?????????? ????????? Text ?? ?????

        string file = File.text.ToString(); // ?????????????? ?????? ? ?????

        contents = file.Split("*"); // ?????? ???? ?????????
        NumberCont = contents.Length;

    }
    public override void ShowContent()
    {
       
        if (NumberCont > 0)
        {
            NumberCont -= 1;
            int index = NumberCont;
            MoveY(0);

            Info.text = contents[index]; // ?????????? ??? ????????? ???????

            contents[index].Remove(index); // ???????? ??????????? ????????? ?? ?????? ??????? 
        }

        else
        {
            Info.text = "????? ????????? ??????????!";

        }
        LocalizationManager.OnResponseChanged.Invoke();

    }
}




public class Joke : Content//1
{
    private static int NumberCont = 12;
    public override TextAsset File { get; set; }
 
    public override int Price { get; set; } = 2000;

    public override string Name => "Joke";

    public  Joke()
    {
        File = (TextAsset)Resources.Load(path: "Content/Jokes");

        Info = GameObject.Find("Content").GetComponent<TMP_Text>();

        string file = File.text.ToString();


        contents = file.Split("*"); // ?????? ?????????

        NumberCont = contents.Length;

    }
    public override void ShowContent()
    {
        
        if (NumberCont > 0)
        {
            MoveY(0);
            NumberCont -= 1;
            int index = NumberCont;
            Info.text = contents[index];   
        }
        else
        {
            Info.text = "????? ???????? ??????????!";
        
        }
        LocalizationManager.OnResponseChanged.Invoke();
    }
}


 
public class Story : Content //0
{
    public override string Name => "Story";

    private static int NumberCont = 0;
    public override int Price { get; set; } = 3000;
    public override TextAsset File { get; set; }


    public Story()
    {
        File = (TextAsset)Resources.Load(path: "Content/Story");

        Info = GameObject.Find("Content").GetComponent<TMP_Text>();

        string file = File.text.ToString();

        contents = file.Split("*"); // ?????? ???????

        NumberCont = contents.Length;
    }
    public override void ShowContent()
    {
       

        if (NumberCont > 0)
        {
            MoveY(0);
            int index = NumberCont-1;

            NumberCont -= 1;

            

            Info.text = contents[index];

            contents[index].Remove(index);
        }

        else
        { Info.text = "????? ?????? ????? ??????????!"; }

        LocalizationManager.OnResponseChanged.Invoke();
    }

    
}


public class InputContent : MonoBehaviour
{
    [SerializeField] private int Pos =0;

    [SerializeField] private AudioClip MaryCrist;

    [SerializeField]  private AudioClip ClickButton;
    public GameObject BoardContent;

    public Button ButtonBoardContent;

    private Board board;

    public Content droppedContent;
    private void Awake()
    {
        InitializeContent();
    }
    private Content content = null;
    public void InitializeContent()
    {
        content = null;

        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();

        Events.MusicClick.Invoke(MaryCrist);

        BoardContent.SetActive(true); // ?????

        content = GetContentByIndex(Events.DroppedIndex);

        content.SetTitleBoard(ref board.ContentType);
    }
   
    public static Content GetContentByIndex(short index)

    {
        switch (index)
        {

            case 0:
                return new Story();
          
            case 1:
                return  new Joke();
             
            case 2:
                return new Motivation();

            default:
                return null;
        }

    }

    public void ShowContent()
    {
        
        if (content.Price > 0)
        {
            
            MoveY(0);
           
        
            content.ShowContent();

            StartCoroutine(ScoreCaracter.Instance.ChangeScoreAndRecord(-(content.Price), 50));// ?????? ?? ???????
        }

        LocalizationManager.OnResponseChanged.Invoke();
    }

    protected async void MoveY(float Y) { await BoardContent.transform.DOLocalMoveY(Y, 1f).Play().AsyncWaitForCompletion(); }

    public void ExitFromBoardContent()
    {

        Events.MusicClick.Invoke(ClickButton);

        if (ScoreCaracter.Instance.Score < content.Price)

            ButtonBoardContent.interactable = false;

        else
        {

            ButtonBoardContent.interactable = true;

        }
        MoveY(5000);



    }


 }
 


