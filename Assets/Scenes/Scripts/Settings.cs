using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
public sealed class Settings : MonoBehaviour
{

    public enum Direction
    {

        To,
        Out

    }
    Direction dir = Direction.To;

    private void Start()
    {
        Events.MusicForce = 0.5f;

        audioSource.volume = Events.MusicForce;

        MusicSlider.value = Events.MusicForce;



    }

    [SerializeField] private Transform[] Settings_Elements;

    [SerializeField] private AudioClip AudioClickButton;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Slider MusicSlider;

    [SerializeField] private AudioClip PopupClip;

    [SerializeField] private RectTransform WindowExit;


    public async void MoveSettings(float speed = 1.5f)
    {

        Events.MusicClick.Invoke(AudioClickButton);



        if (Events.AllTasks != null)
        {
            Task[] tasks = Events.AllTasks.ToArray();

            while (!Task.WhenAll(tasks).IsCompleted)
            { await Task.Yield(); }
        }

        if (dir == Direction.To)
        {
            dir = Direction.Out;


            foreach (Transform element in Settings_Elements)
            {
                element.DOScale(Vector3.zero, 0.001f);
            }



            await transform.DOLocalMoveX(0, speed).Play().AsyncWaitForCompletion();

            await UploadSettingsAnim(Settings_Elements);

        
        }


        else if (dir == Direction.Out)
        {
            dir = Direction.To;
            await transform.DOLocalMoveX(-1400, speed).Play().AsyncWaitForCompletion();

        }

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
            Events.MusicClick.Invoke(PopupClip);

            Tween SequencePopup = DOTween.Sequence()
               .Append(element.DOScale(2 * Vector3.one, speed)).
                Append(element.DOScale(Vector3.one, speed)).Play();

            await SequencePopup.AsyncWaitForCompletion();
        }

 



    }


    public async void ExitGame()
    {
        RectTransform [] ExitMenuChildrens = WindowExit.GetChild(0).GetComponentsInChildren<RectTransform>(true);

        WindowExit.gameObject.SetActive(true);

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

    public CancellationTokenSource token; 

    Task Thread1 = new Task(() => {

         


    });





}
