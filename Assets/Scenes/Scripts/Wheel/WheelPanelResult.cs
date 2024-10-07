using UnityEngine.UI;
using UnityEngine;

public class WheelPanelResult : DashboardAnimator
{
    // ����������
    [SerializeField] private Image imagePanel;

    [SerializeField] private ParticleSystem confettiSystem;
    internal void SetDroppedImage(Image img)
    {
        imagePanel.sprite = img.sprite;
    
    }
    internal async void OutputResult(Section droppedSection)
    {
        Content currentCont = InputContent.GetContentByIndex(droppedSection.indexSection);
        await DisplayGrowingLoadingPanel(" ����������! � ��� ������: " + currentCont.Name,1);
        confettiSystem.Play();

    }
}
