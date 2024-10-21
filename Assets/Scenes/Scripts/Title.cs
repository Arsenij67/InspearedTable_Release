using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;
using Unity.VisualScripting;

public sealed class Title:MonoBehaviour
{
    public int x, y;


    private Item _item;


    public Item item
    {
        get
        {


            return _item;
        }
        set
        {


            _item = value;


            Icon.sprite = _item.sprite;

        }

    }


    public Image Icon;

    public Button button;

    private ParticlePopup popup;
    public ParticlePopup Popup
    {

        get
        {
            
            popup = transform.GetChild(0).GetComponent<ParticlePopup>();

            return popup;
        
        }

        private set { }
    
    }

    private bool canMix = true;
    internal bool CanMix => canMix;


    public Title Left => x > 0 ? Board.Instance.tiles[x - 1, y] : null; // свойства,хранящие своих соседей
    public Title Top => y > 0 ? Board.Instance.tiles[x, y - 1] : null;
    public Title Right => x < Board.Instance.With - 1 ? Board.Instance.tiles[x + 1, y] : null;
    public Title Down => y < Board.Instance.Height - 1 ? Board.Instance.tiles[x, y + 1] : null;

    public Title[] Neightbours => new[] // массив всех соседей каждого тайла
        {

        Left,
        Top,
        Right,
        Down,

        };


    private void Awake() {
        
       
 
        AddListener();
       
    }

    public void AddListener() => button.onClick.AddListener(() => Board.Instance.Select(this));


    public List<Title> GetConnectedTiles(List<Title> exclude = null)
    {
        var result = new List<Title> { this, };
        if (exclude == null)
        {

            exclude = new List<Title> { this, };


        }


        else { exclude.Add(this);
        }

        foreach ( var neighbour in Neightbours)
        {

            if (neighbour == null || exclude.Contains(neighbour) || neighbour._item != _item) continue;

            result.AddRange(neighbour.GetConnectedTiles(exclude));

        }

        return result; 
    
    
    }

    internal IEnumerator FallDown(float time)
    {
        // Ссылка на текущий объект
        canMix = false;

        Transform thisTransform = Instantiate(this.gameObject,this.transform).transform;
        thisTransform.transform.localPosition = new Vector2(0,0);

        // Создаем последовательность анимаций
        DG.Tweening.Sequence sequenceFall = DOTween.Sequence();
        DG.Tweening.Sequence sequenceRotate = DOTween.Sequence();

        Vector3 endVector = new Vector3(transform.rotation.x, transform.rotation.y, 720);

        sequenceFall.Append(thisTransform.DOMoveY(thisTransform.position.y - 10f, time)).Append(thisTransform.DOMoveY(thisTransform.position.y - 10f, time+time*0.25f).SetEase(Ease.OutQuad));

        sequenceRotate.Append(thisTransform.DORotate(endVector, time, RotateMode.FastBeyond360));

        sequenceRotate.Play();

        yield return sequenceFall.Play().WaitForCompletion();

        Destroy(thisTransform.gameObject);
        canMix = true;


    }


}
