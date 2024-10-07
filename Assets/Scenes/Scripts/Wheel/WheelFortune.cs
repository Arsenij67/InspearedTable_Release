using DG.Tweening;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class WheelFortune : DashboardAnimator
{
    [SerializeField] private Section mainSection;
    private byte countSections = 0;
    private Section lastSection;
   public void RollWheel()
    {
        StartCoroutine(InitWheelFortune());  
     
    }

    public async void OpenRollWheel()
    {
        await base.DisplayGrowingLoadingPanel("",2);
        RollWheel();
    }

    private IEnumerator InitWheelFortune()
    {
       
        int randomAngle = 3600+Random.RandomRange(360,720);
        GetLastSection(mainSection, out countSections, out lastSection);
        lastSection.ArrangeIcon((float)1 / countSections);
        yield return StartCoroutine(lastSection.DrawSection(0.3f, 0,(float)1/countSections)); // ���� ����� ��������� ������
        yield return StartCoroutine(RotateWheel(randomAngle, 10)); // ������ ������ � ��� �����
        Section currentsection = CalculateDroppedSelection(randomAngle);
        Content c = InputContent.GetContentByIndex(currentsection.indexSection);
        print(c.name);

    }

    private Section CalculateDroppedSelection(int randomAngle)
    {
        Section newlastSection = mainSection;
        while (newlastSection.transform.childCount > 1 && newlastSection.transform.GetChild(1).GetComponent<Section>())
        {
 
            if (newlastSection.IsSectionActive() && newlastSection.isAreaSelected(randomAngle))
            {
               
                break;
            }

            newlastSection = newlastSection.transform.GetChild(1).GetComponent<Section>();

        }
        return newlastSection;
    }

    /// <summary>
    /// ��������� ����� ��������� ������ 
    /// </summary>
    /// <param name="rootSect">������� ������ ������ </param>
    /// <param name="depth">���������� ������� ����� ��������� ������</param>
    /// <param name="lastSection">������ �� ����� ��������� ������ � ����</param>
    private void GetLastSection(Section rootSect,out byte depth,out Section lastSection)
    {
       depth = -0;
        
        lastSection = rootSect;
      
        while (lastSection)
        {
            if (lastSection.IsSectionActive())
            {
                depth++;
            }
            if (lastSection.transform.GetChild(1).GetComponent<Section>() == null) break;

            lastSection = lastSection.transform.GetChild(1).GetComponent<Section>();
           
        }
      
        
    }

    private IEnumerator RotateWheel(float angleDistance, float time)
    {
        float end = transform.rotation.eulerAngles.z + angleDistance;
        Vector3 endVector = new Vector3(transform.rotation.x, transform.rotation.y, end);
        Tween rotatingTween = DOTween.Sequence().AppendInterval(1f).Append(mainSection.gameObject.transform.DORotate(endVector, time, RotateMode.FastBeyond360));
       yield return rotatingTween.Play().WaitForCompletion();
    }



}
