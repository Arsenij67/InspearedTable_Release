using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;

[RequireComponent(typeof(Slider))]
public class Section : MonoBehaviour
{

    public  TMP_Text textTypeInfo; // ������� � ����� ��������
    [Tooltip("������ ������ ����������, ����� ��� ���������� ������������ 0 1 ��� 2")]
    [SerializeField] private byte indexSection; 

    private Slider sectionSlider;

    [SerializeField] private Section sectionParent;

    private float MaxAngle = 0, MinAngle = 0;

    internal bool IsSectionActive()
    {
        print("�������� �������:");
        Events.indexesActived.ForEach((el) => print(el));
        return  Events.indexesActived.Contains(indexSection);

    }

    /// <summary>
    /// ������ ������
    /// </summary>
    /// <param name="fraction">����� ����� � ����� �� 0 �� 1 �������� ��������� �����</param>
    /// <param name="howFast">����� � �������� ������� ������</param>

    public IEnumerator DrawSection(float howFast, float sizeOneSection,float occupierFraction )
    {
        float targetValue = occupierFraction;
        if (IsSectionActive())
        {
            
            sectionSlider = GetComponent<Slider>();
            targetValue = Mathf.Clamp(sizeOneSection + occupierFraction, 0, 1);
                       float elapsedTime = 0; // ����� ��������� ����� �������
            float startValue = occupierFraction;
            while (elapsedTime < howFast)
            {
                elapsedTime += Time.deltaTime;
                sectionSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / howFast);
                yield return new WaitForEndOfFrame();
            }
            MinAngle = 360 * startValue;
            MaxAngle = 360 * targetValue;
   
        }

        if (sectionParent)
        {
            yield return StartCoroutine(sectionParent.DrawSection(howFast, sizeOneSection, targetValue));
        }
        
       
    }
    public bool isAreaDropped(float endAngle)
    {
       
        while (endAngle > 360)
        {
            endAngle -= 360;
        }
    
        return endAngle > MinAngle && endAngle < MaxAngle;
    }


}
