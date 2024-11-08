using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ParticlePopup : MonoBehaviour
{
    private ParticleSystem Particle;

    private Text TextScore;

    private void Awake()
    {
        Particle = GetComponent<ParticleSystem>();

    }
    [HideInInspector]
    public Color RandomColor
    {
        get { return ColorDist[Random.Range(0, ColorDist.Count())]; }
        private set { }
    }

    private Color [] ColorDist =  

    { Color.red,
      Color.yellow, 
      Color.green, 
      Color.blue,
      Color.magenta
    };

  
    public void ExploadScore(Color color,string score="")
    {
        TextScore = transform.GetChild(0).GetComponent<Text>();

        Particle.startColor = color;

        TextScore.text = $"+{score}";

        TextScore.color = color;

        Canvas.ForceUpdateCanvases();

        Particle.Play();
       
    
    
    }// для системы частиц

    public async void Fly(float lenght = 200f,float duration=1.5f)
    {
        if (this)
        {
            Text ScoreText = transform?.GetChild(0).GetComponent<Text>();
            float StartPosition = transform.position.y;
            float EndPosition = transform.position.y + lenght;

            Tween SequenceFly = DOTween.Sequence().Append(transform.DOLocalMoveY(EndPosition, 1.5f)).Append(ScoreText.DOFade(0, 2f));

            await SequenceFly.AsyncWaitForCompletion();

            transform.position = new Vector3(transform.position.x, StartPosition, transform.position.z);
        }
    
    }// для текста
}
