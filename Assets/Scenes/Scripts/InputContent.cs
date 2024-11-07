using DG.Tweening;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public abstract class Content : InputContent
{


    public abstract string Name { get; }
   

    public string[] contents;
    protected static int NumberCont;
    public abstract TextAsset File { get; set; }
    public abstract int Price { get; set; }
    public abstract void ShowContent();

    public Image contentImage;

    internal void SetTitleBoard(ref TMP_Text title)
    {
        title.text = Name;

    }




} 
public class Motivation : Content//2

{
    public override string Name => "Motivation";
    public override int Price { get; set; } = 1500;
    public override TextAsset File { get; set; }
    public Motivation()
    {
       

        File = Resources.Load<TextAsset>(path:"Content/Motivation");

        string file = File.text.ToString();

        print(file);
        contents = file.Split("*"); // ?????? ?????????
        print(contents);
        NumberCont = contents.Length;



    }
    public override void ShowContent()
    {
       
        if (NumberCont > 0)
        {
            NumberCont -= 1;
            int index = NumberCont;
            MoveY(0);
            Info = GameObject.FindGameObjectWithTag("Content").GetComponent<TMPro.TMP_Text>();

            Info.text = (contents[index]); // ?????????? ??? ????????? ???????

            contents[index].Remove(index); // ???????? ??????????? ????????? ?? ?????? ??????? 
        }

        else
        {
           Info.text = ("Запас мотивация закончился!");

        }
        LocalizationManager.OnResponseChanged();

    }
}




public class Joke : Content//1
{
     
    public override TextAsset File { get; set; }
 
    public override int Price { get; set; } = 2000;

    public override string Name => "Jokes";

    public  Joke()
    {

       

        File =Resources.Load<TextAsset>(path: "Content/Jokes");

        string file = File.text.ToString();

        print(file);
        contents = file.Split("*"); // ?????? ?????????
        print(contents);

        NumberCont = contents.Length;

    }
    public override void ShowContent()
    {
        
        if (NumberCont > 0)
        {
            MoveY(0);
            NumberCont -= 1;
            int index = NumberCont;
            Info = GameObject.FindGameObjectWithTag("Content").GetComponent<TMPro.TMP_Text>();


            Info.text = (contents[index]); // ?????????? ??? ????????? ???????
        }
        else
        {
            Info.text = ("Запас шуток закончился!");
        
        }
        LocalizationManager.OnResponseChanged();

    }
}


 
public class Story : Content //0
{
    public override string Name => "Story";
    
    public override int Price { get; set; } = 3000;
    public override TextAsset File { get; set; }


    public Story()
    {
        
        File = Resources.Load<TextAsset>(path: "Content/Story");

        string file = File.text.ToString();
        print(file);

        contents = file.Split("*"); // ?????? ?????????
        print(contents);

        NumberCont = contents.Length;

    }

    public override void ShowContent()
    {
       

        if (NumberCont > 0)
        {
            MoveY(0);
            int index = NumberCont-=1;

            Info = GameObject.FindGameObjectWithTag("Content").GetComponent<TMPro.TMP_Text>();
            Info.text = contents[index];
            // ?????????? ??? ????????? ???????
           
            contents[index].Remove(index);
        }

        else
        {
            Info.text = "Запас историй закончился!";
          
        }
        LocalizationManager.OnResponseChanged();

    }

    
}


public class InputContent : MonoBehaviour
{
    [SerializeField] private int Pos =0;

    [SerializeField] private AudioClip MaryCrist;

    [SerializeField]  private AudioClip ClickButton;

    public Transform BoardContent;

    public TMP_Text Info;

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

        BoardContent.gameObject.SetActive(true); // ?????

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
 
    }

    protected void MoveY(float Y) { BoardContent.DOLocalMoveY(Y, 1f).Play(); }

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
 


