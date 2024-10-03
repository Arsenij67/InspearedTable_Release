using DG.Tweening;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class WheelFortune : MonoBehaviour
{
    [SerializeField] private Section mainSection;
    private byte countSections = 0;
    private Section lastSection;
    private async void Awake()
    {

        StartCoroutine(InitWheelFortune());  
     
    }

    private IEnumerator InitWheelFortune()
    {
        int randomAngle = 3600+270;
        GetLastSection(mainSection, out countSections, out lastSection);
        yield return StartCoroutine(lastSection.DrawSection(0.3f, 0f)); // ���� ����� ��������� ������
        yield return StartCoroutine(RotateWheel(randomAngle, 10)); // ������ ������ � ��� �����
        print(CalculateDroppedSelection(randomAngle).name+"  rand Angle"+ randomAngle);
    }

    private Section CalculateDroppedSelection(int randomAngle)
    {
        Section newlastSection = mainSection;
        while (newlastSection.transform.childCount > 1 && newlastSection.transform.GetChild(1).GetComponent<Section>())
        {
 
            if (newlastSection.isAreaSelected(randomAngle))
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
        depth = 1;
        lastSection = mainSection;
        while (lastSection.transform.childCount > 1 && lastSection.transform.GetChild(1).GetComponent<Section>()!=null) 
        {
           
            lastSection = lastSection.transform.GetChild(1).GetComponent<Section>();
            depth++;
        }
    }

    private IEnumerator RotateWheel(float angleDistance, float time)
    {
        float start = transform.rotation.eulerAngles.z;
        float end = transform.rotation.eulerAngles.z + angleDistance;
        Vector3 endVector = new Vector3(transform.rotation.x, transform.rotation.y, end);
        Tween rotatingTween = DOTween.Sequence().AppendInterval(1f).Append(mainSection.gameObject.transform.DORotate(endVector, time, RotateMode.FastBeyond360));
       yield return rotatingTween.Play().WaitForCompletion();
    }



}
