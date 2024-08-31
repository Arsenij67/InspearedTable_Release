using DG.Tweening;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
 
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LeaderBoard : DashboardAnimator
{
    [HideInInspector] public Text TextNames;
    [HideInInspector] public Text TextRecords;
    [SerializeField] private Fb fb;
    [SerializeField] private ButtonController buttonController;
    [SerializeField] private GameObject PhoneLoading;
    [Space(10f)]
    [SerializeField] private GameObject InfoPrefab,content,NumberText;
    private Dictionary<string,AudioClip> LeaderBoardClips;// все клипы в словаре
    [SerializeField] private GameObject [] CrownButtons = new GameObject[3];   
    private void Awake()
    {
        LeaderBoardClips = Resources.LoadAll<AudioClip>("Music/").ToDictionary(key => key.name,value => value);
    }
    private enum GO_Out
    {
        Go,
        Out

    }

    private GO_Out Switcher = GO_Out.Go;

    [SerializeField] private AudioClip ClipClick;

    [Obsolete]
    public void Go_OutToBoard()
    {
        Events.MusicClick.Invoke(ClipClick);

        StartCoroutine(Events.ChechInternetConnection(result =>
        {

            if (result.Equals(false))
            {
                buttonController.PopupWarning("No internet connection!", Color.red);
            }
            else// если есть интернет
            {

                int dir = 0;

                switch (Switcher)
                {

                    case GO_Out.Go:
                        Events.AllTasks.Add(PrintBoard());
                        Events.AllTasks.Add(AppearTitles(duration: 0.2f));
                        buttonController.inputTextName.interactable = false;
                       
                        dir = 0;
                        Switcher = GO_Out.Out;
                        break;

                    case GO_Out.Out:
                        Events.AllTasks.Add(DeleteText());
                        dir = 1500;
                        Switcher = GO_Out.Go;
                        buttonController.inputTextName.interactable = true;
                        break;

                    default:
                        break;


                }
               
                Events.AllTasks.Add(DoMoveBoard(dir,AnimTime:0.2f));


            }

        }));

   

    }

    private async Task PrintBoard( )
    {

        await fb.InitInfo();
        print("Скачано");

        DataSnapshot LeaderBoardSnapshot = fb.dataSnapshot;
        int i = 1;

        int CrownIndex = 0;

        foreach (var snapsh in LeaderBoardSnapshot.Children.Reverse())

        {

            var UserPrefab = Instantiate(InfoPrefab, content.transform.position, Quaternion.identity);

           await CreateTextUI(NumberText.GetComponent<Text>(),number:i, offset: new Vector3(8, 0, 0), UserPrefab);//создание места в таблице


            UserPrefab.transform.SetParent(content.transform);

         

            Text textInfoName = UserPrefab.GetComponent<Text>();
 

            Text textInfoScore = UserPrefab.transform.GetChild(0).GetComponent<Text>();



            textInfoName.text +=snapsh.Child("Name").Value.ToString() + "\n";

          

            textInfoScore.text += snapsh.Child("Record").Value.ToString() + "\n";


            if (CrownIndex < CrownButtons.Count())
            {
                var ImageCrown = Instantiate(CrownButtons[CrownIndex], textInfoName.transform.localPosition , Quaternion.identity); //создаем корону

                ImageCrown.transform.SetParent(textInfoName.transform); // задаем родителя коронам 

                ImageCrown.transform.localScale = new Vector3(20, 20, 20);

                ImageCrown.transform.localPosition = new Vector3(-200,0,0);
            }

            i++;

            CrownIndex++;

            await Task.Yield();
        }


   
        


    }


    private async Task CreateTextUI(Text TextPrefab,int number,Vector3 offset,GameObject Parent)
    {

        var Prefab = Instantiate(TextPrefab,Parent.transform.GetChild(0).transform.position - offset,Quaternion.identity).GetComponent<Text>();

        Prefab.fontSize = 40;

        Prefab.transform.SetParent(Parent.transform);
        
        Prefab.transform.DOScale(new Vector3(1, 1, 1), 0.01f);
        Prefab.color = Color.red;
 

    Prefab.text = number.ToString();

    Prefab.supportRichText = true;

    await Task.Yield();

    }

    public virtual Task DoMoveBoard(float dir,float AnimTime)
    {
        var SequenceMove = DOTween.Sequence().Append(transform.DOLocalMoveY(dir, AnimTime)).AppendInterval(1f);

        return  SequenceMove.Play().AsyncWaitForCompletion();
        

    }
 

      private async Task AppearTitles(float duration)
    {

        List<GameObject> Elements = new List<GameObject>();

        Elements.Add(transform.GetChild(0).gameObject);
        Elements.Add(transform.GetChild(1).gameObject);
        Elements.Add(transform.GetChild(2).gameObject);

        foreach (var element in Elements) element.transform.DOScale(Vector2.zero, 0.01F);

        for (int i = 0; i < Elements.Count; i++) 
        {

            Tween SequenceAppear = DOTween.Sequence().
                Append(Elements[i].transform.DOScale(Vector3.zero, duration)).
                Append(Elements[i].transform.DOScale(Vector3.one, duration));

                Events.MusicClick.Invoke(LeaderBoardClips["Popup"]);




            await SequenceAppear.Play().AsyncWaitForCompletion();

        }
    
    }

    private async Task DeleteText()
    { 
        int CountChilds = content.transform.childCount;

        print(CountChilds);

        for (int i = 0; i < CountChilds; i++)
        { 
        
        Destroy(content.transform.GetChild(0).gameObject);
        
        await Task.Yield();

        }
    
    
    }


     

    

 
}
