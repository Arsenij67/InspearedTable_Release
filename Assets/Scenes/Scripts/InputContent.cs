using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public abstract  class Content : InputContent
{
 

    public  TMP_Text Info { get; set; }

    public string[] contents;
  
    public abstract TextAsset File { get; set; }
    public abstract int Price { get; set; }
    public abstract void ShowContent();

    public Image content;

} 
public class Motivation : Content//2

{
    private static int NumberCont = 15;
    public override int Price { get; set; } = 1500;
    public override TextAsset File { get; set; }
    public Motivation()
    {

        File = (TextAsset)Resources.Load(path: "Content/Motivation"); //�������� ������ �� ����� 

        Info = GameObject.Find("Content").GetComponent<TMP_Text>(); // ���������� ��������� Text �� �����

        string file = File.text.ToString(); // �������������� ������ � �����

        contents = file.Split("*"); // ������ ���� ���������
        NumberCont = contents.Length;

    }
    public override void ShowContent()
    {
       
        if (NumberCont > 0)
        {
            NumberCont -= 1;
            int index = NumberCont;
            MoveY(0);

            Info.text = contents[index]; // ���������� ��� ��������� �������

            contents[index].Remove(index); // �������� ����������� ��������� �� ������ ������� 
        }

        else
        {
            Info.text = "����� ��������� ����������!";

        }
    }
}




public class Joke : Content//1
{
    private static int NumberCont = 12;
    public override TextAsset File { get; set; }
 
    public override int Price { get; set; } = 2000;

    public  Joke()
    {
        File = (TextAsset)Resources.Load(path: "Content/Jokes");

        Info = GameObject.Find("Content").GetComponent<TMP_Text>();

        string file = File.text.ToString();


        contents = file.Split("*"); // ������ ���������

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
            Info.text = "����� ��������� ����������!";
        
        }
    }
}


 
public class Story : Content //0
{
    private static int NumberCont = 0;
    public override int Price { get; set; } = 3000;
    public override TextAsset File { get; set; }


    public Story()
    {
        File = (TextAsset)Resources.Load(path: "Content/Story");

        Info = GameObject.Find("Content").GetComponent<TMP_Text>();

        string file = File.text.ToString();

        contents = file.Split("*"); // ������ �������

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
        { Info.text = "����� ������ ����� ����������!"; }
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

    private void Awake()
    {
        InitializeContent();
    }
    internal Content content = null;
    public  async void InitializeContent()
    {
        content = null;

        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();

        Events.MusicClick.Invoke(MaryCrist);

        BoardContent.SetActive(true); // �����



       short index = board.GetCotentType();

       content =  GetClassBuyIndex(index);

    }
    public Content GetClassBuyIndex(int i)
    {
        switch (i)
        {
            case 0:
                return new Story();
                 
            case 1:
                return new Joke();
      
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

            StartCoroutine(ScoreCaracter.Instance.ChangeScoreAndRecord(-(content.Price), 50));// ������ �� �������
        }
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
 


