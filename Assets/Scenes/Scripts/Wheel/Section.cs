using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;

[RequireComponent(typeof(Slider))]
public class Section : MonoBehaviour
{
    // переменные
    [SerializeField] private Image imageSection;// изображение для отображения секции

    private Slider sectionSlider;

    private Section ? sectionParent;

    public float adder = 0;

    private float MaxAngle = 0, MinAngle = 0;

    public  byte indexSection;

    // свойства
    internal Image ImageSection => imageSection;
    public void Awake()
    {

         if (transform.childCount > 1)
        {
            transform.parent.TryGetComponent<Section>(out sectionParent);
        } 

    }

    /// <summary>
    /// Рисует секцию
    /// </summary>
    /// <param name="fraction">какая часть в долях от 0 до 1 занимает выбранная часть</param>
    /// <param name="howFast">время в секундах рисовки секции</param>

    public IEnumerator DrawSection(float howFast, float occupiedFraction, float sectionArea)
    {
        float targetValue = Mathf.Clamp(occupiedFraction, 0, 1);
        if (IsSectionActive())
        {
            sectionSlider = GetComponent<Slider>();
            float elapsedTime = 0; // время прошедшее после запуска
            
            float startValue = sectionSlider.value + occupiedFraction;
            targetValue = startValue + sectionArea;
            while (elapsedTime < howFast)
            {
                elapsedTime += Time.deltaTime;
                sectionSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / howFast);
                yield return new WaitForEndOfFrame();
            }
        }
            if (sectionParent)
            {
                yield return StartCoroutine(sectionParent?.DrawSection(howFast, targetValue, sectionArea));
            }
            MinAngle = 360 * occupiedFraction;
            MaxAngle = 360 * targetValue;
        
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
    private Vector2 GetCoordinatesLabel(float Angle, float radius = 10)
    {
        float x = Mathf.Cos((Angle) * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin((Angle) * Mathf.Deg2Rad) * radius;
        return new Vector2(x, -y);
    }

    internal void ArrangeIcon(float sizeOneSection, float occupierFraction=0)
    {
        imageSection.gameObject.SetActive(IsSectionActive());
        float targetValue = occupierFraction;
        if (IsSectionActive())
        {
            targetValue = Mathf.Clamp(sizeOneSection + occupierFraction, 0, 1);
            float startValue = occupierFraction;
            MinAngle = 360 * startValue;
            MaxAngle = 360 * targetValue;
            print($" MinAngle = {MinAngle} MaxAngle = {MaxAngle}  res =  {MinAngle + (MaxAngle - MinAngle) / 2}");
            if (Events.indexesActived.Count() <= 1)
            {
                SetCoordinatesLabel(0);
                return;
            }

            SetCoordinatesLabel((MinAngle + ((MaxAngle - MinAngle) / 2)) +90,100);

        }
        
             
        if (sectionParent)
        {
            sectionParent.ArrangeIcon(sizeOneSection, targetValue);
        }

       
    }
    private void SetCoordinatesLabel(float Angle, float radius = 0)
    {

        imageSection.transform.localPosition = GetCoordinatesLabel((float)Angle, radius);
    }

    private float GetRadiusLabel(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }
    internal bool IsSectionActive()
    {
        return Events.indexesActived.Contains(indexSection);

    }


}
