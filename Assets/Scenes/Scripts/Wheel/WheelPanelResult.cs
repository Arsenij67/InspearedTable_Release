using UnityEngine.UI;
using UnityEngine;

public class WheelPanelResult : DashboardAnimator
{
    // переменные
    [SerializeField] private Image imagePanel;

    [SerializeField] private ParticleSystem confettiSystem;

    [SerializeField] private AudioClip clipResult;
    internal void SetDroppedImage(Image img)
    {
        imagePanel.sprite = img.sprite;
    
    }
    internal async void OutputResult(Section droppedSection)
    {
        Content currentCont = InputContent.GetContentByIndex(droppedSection.indexSection);
        await DisplayGrowingLoadingPanel(" Поздравляю! У вас выпало: " + currentCont.Name,1);
        confettiSystem.Play();
        Events.MusicClick.Invoke(clipResult);

    }
}
