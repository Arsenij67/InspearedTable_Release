using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WheelFortune : MonoBehaviour
{
    [SerializeField] private Section mainSection;
    private byte countSections = 0;
    private Section lastSection;

    public void RollWhealFortune()
    {
        gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(InitWheelFortune());
    }
    private IEnumerator InitWheelFortune()
    {
        int randomAngle = 3600+Random.Range(0,360);
        GetLastSection(mainSection, out countSections, out lastSection);
        print((float)1 / countSections + " = ACTIVED IND");
        yield return StartCoroutine(lastSection.DrawSection(0.5f, (float)1/countSections,0f)); // ���� ����� ��������� ������
        yield return StartCoroutine(RotateWheel(randomAngle, 10)); // ������ ������ � ��� �����
        Section sectin = CalculateDroppedSelection(randomAngle);
        print("������ = "+ sectin.textTypeInfo.text);
    


    }

    private Section CalculateDroppedSelection(int randomAngle)
    {
        Section newlastSection = mainSection;
        while (newlastSection.transform.childCount > 1 && newlastSection.transform.GetChild(1).GetComponent<Section>())
        {
 
            if (newlastSection.isAreaDropped(randomAngle))
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
        depth = 0;
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
        float start = transform.rotation.eulerAngles.z;
        float end = transform.rotation.eulerAngles.z + angleDistance;
        Vector3 endVector = new Vector3(transform.rotation.x, transform.rotation.y, end);
        Tween rotatingTween = DOTween.Sequence().AppendInterval(2f).Append(mainSection.gameObject.transform.DORotate(endVector, time, RotateMode.FastBeyond360));
       yield return rotatingTween.Play().WaitForCompletion();
    }



}
