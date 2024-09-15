using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;

[RequireComponent(typeof(Slider))]
public class Section : MonoBehaviour
{

    public  TMP_Text textTypeInfo; // надпись с типом контента
    [Tooltip("индекс секции показывает, какой тип информации используется 0 1 или 2")]
    [SerializeField] private byte indexSection; 

    private Slider sectionSlider;

    [SerializeField] private Section sectionParent;

    private float MaxAngle = 0, MinAngle = 0;

    internal bool IsSectionActive()
    {
        print("Активные индексы:");
        Events.indexesActived.ForEach((el) => print(el));
        return  Events.indexesActived.Contains(indexSection);

    }

    /// <summary>
    /// Рисует секцию
    /// </summary>
    /// <param name="fraction">какая часть в долях от 0 до 1 занимает выбранная часть</param>
    /// <param name="howFast">время в секундах рисовки секции</param>

    public IEnumerator DrawSection(float howFast, float sizeOneSection,float occupierFraction )
    {
        float targetValue = occupierFraction;
        if (IsSectionActive())
        {
            
            sectionSlider = GetComponent<Slider>();
            targetValue = Mathf.Clamp(sizeOneSection + occupierFraction, 0, 1);
                       float elapsedTime = 0; // время прошедшее после запуска
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
