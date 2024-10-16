using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class WheelPanelResult : DashboardAnimator
{
    // ����������
    [SerializeField] private Image imagePanel;

    [SerializeField] private ParticleSystem confettiSystem;

    [SerializeField] private AudioClip clipResult;
    internal void SetDroppedImage(Image img)
    {
        imagePanel.sprite = img.sprite;
    
    }
    internal IEnumerator OutputResult(Section droppedSection)
    {
        Content currentCont = InputContent.GetContentByIndex(droppedSection.indexSection);
        Events.DroppedIndex = droppedSection.indexSection;
        Debug.Log(Events.DroppedIndex);
        yield return  DisplayGrowingLoadingPanel(" ����������! � ��� ������: " + currentCont.Name,2);
        confettiSystem.Play();
        Events.MusicClick.Invoke(clipResult);
        yield return new WaitForSeconds(3f);

    }
}
