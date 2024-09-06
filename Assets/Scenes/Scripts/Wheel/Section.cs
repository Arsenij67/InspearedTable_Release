using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;

[RequireComponent(typeof(Slider))]
public class Section : MonoBehaviour
{
    [SerializeField] private Image imageSection;// ����������� ��� ����������� ������

    [SerializeField] private TMP_Text textTypeInfo; // ������� � ����� ��������

    private Slider sectionSlider;

    private Section ? childSection;

    private  readonly float fraction = 0.33333f;

    private float MaxAngle = 0, MinAngle = 0;
    public void Awake()
    {

        if (transform.childCount > 1)
        {
            transform.parent.TryGetComponent<Section>(out childSection);
        } 

        else 
        {
            Debug.Log("���� �������� ��������� ��� ����� ������");
        }

    }

    /// <summary>
    /// ������ ������
    /// </summary>
    /// <param name="fraction">����� ����� � ����� �� 0 �� 1 �������� ��������� �����</param>
    /// <param name="howFast">����� � �������� ������� ������</param>

    public IEnumerator DrawSection(float howFast, float occupiedFraction)
    {
        sectionSlider = GetComponent<Slider>();
        float elapsedTime = 0; // ����� ��������� ����� �������
        float targetValue = Mathf.Clamp(sectionSlider.value + fraction + occupiedFraction, 0, 1);
        float startValue = sectionSlider.value+ occupiedFraction;
        while (elapsedTime < howFast)
        {
            elapsedTime += Time.deltaTime;
            sectionSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / howFast);
            yield return new WaitForEndOfFrame();
        }
        if (childSection)
        {
            yield return StartCoroutine(childSection?.DrawSection(howFast, targetValue));
        }
        MinAngle = 360 * occupiedFraction;
        MaxAngle = 360* targetValue;
    }
    public bool isAreaSelected(float endAngle)
    {
       
        while (endAngle > 360)
        {
            endAngle -= 360;
        }
        print(MinAngle + " = MinAngle " + MaxAngle + " = MaxAngle " + name + " True or False: "+ (endAngle > MinAngle && endAngle < MaxAngle)+ " End engle = "+ endAngle);
        return endAngle > MinAngle && endAngle < MaxAngle;
    }

}
