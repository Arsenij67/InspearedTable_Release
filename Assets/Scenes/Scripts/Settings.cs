using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
  public enum Direction
    {

        To = 1,
        Out = -1

    }
public sealed class Settings : DashboardAnimator
{

    Direction dir = Direction.To;

    private void Start()
    {
        Events.MusicForce = 0.5f;

        audioSource.volume = Events.MusicForce;

        MusicSlider.value = Events.MusicForce;

        for (int i = 0; i < transform.childCount; i++)
        {
            settingsElements[i]= transform.GetChild(i);
             
        }


    }
    private const short maxCountChildrens = 20;

    private Transform[] settingsElements = new Transform [maxCountChildrens];

    [SerializeField] private AudioClip AudioClickButton;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Slider MusicSlider;

    [SerializeField] private AudioClip PopupClip;

    [SerializeField] private RectTransform WindowExit;

    


    public async void MoveSettings(float speed = 1.5f)
    {

        PlayBoardMusic();

        if (Events.AllTasks != null)
        {
            Task[] tasks = Events.AllTasks.ToArray();

            while (!Task.WhenAll(tasks).IsCompleted)
            { 
                await Task.Yield();
            }
        }

        if (dir == Direction.To)
        {
            dir = Direction.Out;


            foreach (Transform element in settingsElements)
            {
                element.DOScale(Vector3.zero, 0f);
            }



            await transform.DOLocalMoveX(0, speed).Play().AsyncWaitForCompletion();

            await UploadSettingsAnim(settingsElements);

        
        }


        else if (dir == Direction.Out)
        {
            dir = Direction.To;
            await transform.DOLocalMoveX(-1400, speed).Play().AsyncWaitForCompletion();

        }

    }

    private void PlayBoardMusic()
    {
        Events.MusicClick.Invoke(AudioClickButton);

    }
    public void ChangeMusicForce()
    {

        audioSource.volume = MusicSlider.value;

        Events.MusicForce = audioSource.volume;


    }


    public void VK() => Application.OpenURL("https://vk.com/id446930815");


    public async Task UploadSettingsAnim(object [] elements,float speed=0.1f)
    {
        foreach (Transform element in elements)
        {
            if (element != null)
            {
                Events.MusicClick.Invoke(PopupClip);

                Tween SequencePopup = DOTween.Sequence()
                   .Append(element.DOScale(2 * Vector3.one, speed)).
                    Append(element.DOScale(Vector3.one, speed)).Play();

                await SequencePopup.AsyncWaitForCompletion();
            }
        }

 



    }


    
    public async void ExitGame()
    {
        RectTransform [] ExitMenuChildrens = WindowExit.GetChild(0).GetComponentsInChildren<RectTransform>(true);

        WindowExit.transform.localScale = Vector3.one;

        await UploadSettingsAnim (ExitMenuChildrens,0.05f);

    }

  

    public void ExitDesition(string descrition)
    {
        Events.MusicClick.Invoke(AudioClickButton);

        if (descrition.Equals("Yes"))
        {
            Application.Quit();
          

        }

        else if (descrition.Equals("No"))
        {

            WindowExit.gameObject.SetActive(false);

            foreach (var element in WindowExit.GetChild(0).GetComponentsInChildren<RectTransform>(true))
            {

                element.transform.DOScale(Vector3.zero,0.0001f).Play();
            
            }
        }

        
    }






}
