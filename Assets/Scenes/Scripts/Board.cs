using DG.Tweening;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class Board : MonoBehaviour
{   
    public static Board Instance { get; private set; }
    
    public Row[] rows;

    public TMP_Text ContentType;
    public Title[,] tiles { get; private set; }

    public int With => tiles.GetLength(0);
    public int Height => tiles.GetLength(1);

    public int Weight { get; private set; }

    private readonly List<Title> _selections = new List<Title>();

    private const float AnimTime = 0.5f;

    private AudioSource _audioSource;

    [SerializeField] private AudioClip ButtonClick;

    [SerializeField] private AudioClip SantaLaught;

    public Button ButtonContent;

    private short IndexShow => Events.DroppedIndex;// выбираем 1 индекс случайно из выбранных нами

    

    private Transform Item_Decoration;
    private Fb fb;
    [Header("Префаб очков")]
    [SerializeField] private GameObject TextScore;
    [SerializeField] private ParticleSystem ParcileExpload;
    [SerializeField] private TMP_Text debbug;

    public async void Awake()
    {
         
            Instance = this;
            fb = GetComponent<Fb>();
            CheckScore();
            UpLoadTiles();

            _audioSource = GetComponent<AudioSource>();

            Item_Decoration = GameObject.Find("SelectedItem").GetComponent<Transform>();
        
       
    }
    private async void MoveDecorate(Title tile)
    {
        
            var SequenceMove = DOTween.Sequence();

            SequenceMove.Join(Item_Decoration.transform.DOMove(tile.transform.position - new Vector3(0, 0.5f, 0), AnimTime));

            await SequenceMove.Play().AsyncWaitForCompletion();
        
         
    }
    [Obsolete]
    public async void Select(Title tile)  
    { 
            if (!_selections.Contains(tile))
            {
                if (_selections.Count() > 0)
                {

                    if (Array.IndexOf(_selections[0].Neightbours, tile) != -1)
                    {
                        _selections.Add(tile);
                    }

                    else
                    {


                        Events.MusicClick.Invoke(SantaLaught);

                        await ReChange(_selections[0], tile);

                        await ReChange(_selections[0], tile);

                        _selections.Clear();

                    }

                }

                else
                {

                    Events.MusicClick.Invoke(ButtonClick);

                    _selections.Add(tile);

                    MoveDecorate(tile);

                }




            }

            if (_selections.Count < 2) return;

            await ReChange(_selections[0], _selections[1]);
            if (CanPop())// уничтожаема ли последовательность айтемов
            {

                Pop();


            }

            else if (!CanPop())

            {
                Events.MusicClick.Invoke(SantaLaught);

                await ReChange(_selections[0], _selections[1]);

            }

            _selections.Clear();
    
    }
    [Obsolete]
    public async Task ReChange(Title tile1, Title tile2)  
    {
      
            RemoveListeners(tiles);

            var icon1 = tile1.Icon;
            var icon2 = tile2.Icon;

            var IconTransf1 = icon1.transform;
            var IconTransf2 = icon2.transform;

            var sequence = DOTween.Sequence();

            sequence.Join(IconTransf1.DOMove(tile2.transform.position, AnimTime)).Join(IconTransf2.DOMove(tile1.transform.position, AnimTime));

            await sequence.Play().AsyncWaitForCompletion();

            SwapValues(tile1.x, tile2.x, out tile1.x, out tile2.x);

            SwapValues(tile1.y, tile2.y, out tile1.y, out tile2.y);

            SwapPositionScene(tile1, tile2);

            foreach (var tile in tiles)
            {

                tile.AddListener();

            }
        
    }


    public void UpLoadTiles()
    {
         
            tiles = new Title[rows.Max(rows => rows.titles.Length), rows.Length];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < With; x++)
                {
                    var tile = rows[y].titles[x];

                    tile.x = x;
                    tile.y = y;

                    tile.item = ItemDataBase.items[UnityEngine.Random.Range(0, ItemDataBase.items.Length)];

                    var ExploadParicle = Instantiate(ParcileExpload, tile.transform.position, Quaternion.identity);

                    var textscore = Instantiate(TextScore, ExploadParicle.transform.position, Quaternion.identity);



                    ExploadParicle.transform.SetParent(tile.transform);

                    textscore.transform.SetParent(ExploadParicle.transform);

                    textscore.transform.SetSiblingIndex(0);

                    textscore.transform.position = ExploadParicle.transform.position;

                    // создание заднего окошка на элементе
                    var frame = Instantiate(tile.item.framePrefab, tile.transform);

                    frame.transform.parent = (tile.transform);
                    //

                    tiles[x, y] = rows[y].titles[x];

                }
            }
    }

    public void MixTiles()
    {
       

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < With; x++)
                {
                    var tile = rows[y].titles[x];
                    if (tile.CanMix)
                    {


                        tile.item = ItemDataBase.items[UnityEngine.Random.Range(0, ItemDataBase.items.Length)];

                        StartCoroutine(tile.FallDown(2));
                    }
                }
            }
       

    }

   
    private bool CanPop()
    {
        

            for (var y = 0; y < Height; y++)
                for (var x = 0; x < With; x++) if (tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) return true;
        return false;

    }


    public void SwapValues(int First, int Second, out int first, out int second)
    {
        first = Second;
        second = First;


    }
    
    /// <param name="title1"> первый кристалл </param>
    /// <param name="title2"> второй кристалл</param>
    void SwapPositionScene(Title title1, Title title2)
    {
       
            Transform Parent1 = title1.gameObject.transform.parent;

            Transform Parent2 = title2.gameObject.transform.parent;


            int NumberChild1, NumberChild2;

            NumberChild1 = title1.transform.GetSiblingIndex();

            NumberChild2 = title2.transform.GetSiblingIndex();



            title1.transform.SetParent(Parent2);
            ///меняем индекс
            title1.transform.SetSiblingIndex(NumberChild2);



            title2.transform.SetParent(Parent1);
            ///меняем индекс
            title2.transform.SetSiblingIndex(NumberChild1);


            ///меняем положение в массиве всех элементов
            var save = tiles[title1.x, title1.y];

            tiles[title1.x, title1.y] = tiles[title2.x, title2.y];

            tiles[title2.x, title2.y] = save;
      
    }

    ///<summary>уничтожаем одинаковые айтемы</summary>  
    [Obsolete]
    private void RemoveListeners(Title[,] buttons)
    {
            foreach (var title in buttons)
            {
                title.button.onClick.RemoveAllListeners();

            }
    }

    [Obsolete]
    private async void Pop() 
    {
       
            RemoveListeners(tiles);

            for (int y = 0; y < Height; y++)
            {


                for (int x = 0; x < With; x++)
                {


                    var tile = tiles[x, y];

                    var ConnectedTiles = tile.GetConnectedTiles();


                    if (ConnectedTiles.Skip(1).Count() < 2) continue;

                    var deflateSequence = DOTween.Sequence();

                    foreach (var ConnectedTile in ConnectedTiles) deflateSequence.Join(ConnectedTile.transform.DOScale(Vector3.zero, AnimTime));


                    await deflateSequence.Play().AsyncWaitForCompletion();

                    var InflaeSequence = DOTween.Sequence();

                    _audioSource.Play();

                    StartCoroutine(ScoreCaracter.Instance.ChangeScoreAndRecord(tile.item.Value * ConnectedTiles.Count(), 50));

                    StartCoroutine(fb.WriteData(SaveTypesFactory.deviceSaveManagerString.GetElement("Name") as string, ScoreCaracter.Instance.MaxScore));



                    foreach (var ConnectedTile in ConnectedTiles) //перебор всех соседей и увеличение размера
                    {

                        ConnectedTile.Popup.ExploadScore(ConnectedTile.Popup.RandomColor, ConnectedTile.item.Value.ToString());

                        ConnectedTile.Popup.Fly();

                        InflaeSequence.Join(ConnectedTile.transform.DOScale(Vector3.one, AnimTime));

                        ConnectedTile.item = ItemDataBase.items[UnityEngine.Random.Range(0, ItemDataBase.items.Length)];


                    }
                    await InflaeSequence.Play().AsyncWaitForCompletion();



                    x = 0;
                    y = 0;

                }




            }

            foreach (var title in tiles)
            {
                title.AddListener();

            }
    }
  
   private async void CheckScore()
    {
        while (true)
        {
            if (ScoreCaracter.Instance.Score >= 3000f && IndexShow == 0) ButtonContent.interactable = true;

            else if (ScoreCaracter.Instance.Score >= 2000f && IndexShow== 1) ButtonContent.interactable = true;

            else if (ScoreCaracter.Instance.Score >= 1500f && IndexShow == 2) ButtonContent.interactable = true;

            await Task.Delay(1000);
 
        }

    }

   
}

