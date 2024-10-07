using UnityEngine.UI;
using UnityEngine;

public class WheelPanelResult : DashboardAnimator
{
    [SerializeField] private Image imagePanel;
    internal void SetDroppedImage(Image img)
    {
        imagePanel.sprite = img.sprite;
    
    }
    internal async void OutputResult(Section droppedSection)
    {
        Content currentCont = InputContent.GetContentByIndex(droppedSection.indexSection);
        await DisplayGrowingLoadingPanel(" Поздравляю! У вас выпало: " + currentCont.Name, 2f);

    }
}
