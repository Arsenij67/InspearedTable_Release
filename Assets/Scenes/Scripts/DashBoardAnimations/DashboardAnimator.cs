using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Localization.Editor;
using UnityEngine;

public class DashboardAnimator : MonoBehaviour
{
    public Direction dir = Direction.To;

    [SerializeField] private Transform targetPosition = null;
    [SerializeField] private Transform dashboardLoading;
    [SerializeField] private TMP_Text  mainText;

    internal async Task MoveQueuedAnimationBoard(Transform[] elements, float speed = 0.6f)
    {
        if (Events.AllTasks != null)
        {
            Task[] tasks = Events.AllTasks.ToArray();

            while (!Task.WhenAll(tasks).IsCompleted)
            {
                await Task.Yield();
            }
        }

        float timeDelay = 0.2f;
        float timeOffset = 0.2f;

        foreach (Transform element in elements)
        {
            if (element != null)
            {
                Tween SequencePopup = DOTween.Sequence()
                .AppendInterval(timeDelay)
                .Append(element.DOMoveX(targetPosition.position.x * (int) dir, speed)).Play();
                timeDelay += timeOffset;
            }

        }
        ChangeDirection();

    }
    private void ChangeDirection()
    {
        if (dir.Equals(Direction.To))
        {
            dir = Direction.Out;
            return;
        }

        if (dir.Equals(Direction.Out))
        {
            dir = Direction.To;
            targetPosition.position = new Vector3(Mathf.Abs(targetPosition.position.x), transform.position.y, transform.position.z);
            return;
        }
    }
/// <summary>
/// ????????? ????
/// </summary>
/// <param name="information">?????????????? ????????? ????? ?????????</param>
    protected async Task DisplayGrowingLoadingPanel(string information,float interval = 5)
    {
        print("LF");
        dashboardLoading.gameObject.SetActive(true);
        mainText.text = information;

        LocalizationManager.OnResponseChanged?.Invoke();
 
        await DOTween.Sequence().Append(dashboardLoading.DOScale(Vector3.zero,0))
            .Append(dashboardLoading.DOScale(Vector3.one, 1))
            .AppendInterval(interval).Play().AsyncWaitForCompletion();

    }

    protected async void CloseGrowingLoadingPanel(string information = "")
    {
        mainText.text = information;
        Sequence sequencePop = DOTween.Sequence();
        if (!string.IsNullOrEmpty(information))
        {
            sequencePop.AppendInterval(1f);
        }
        await sequencePop.Append(dashboardLoading.DOScale(Vector3.zero, 0.5f)).Play().AsyncWaitForCompletion();
        dashboardLoading.gameObject.SetActive(false);
        
    }



}
