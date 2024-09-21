using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
using System;

[RequireComponent(typeof(Slider))]
public class Section : MonoBehaviour
{

    public Image ImageTypeInfo; // надпись с типом контента
    [Tooltip("индекс секции показывает, какой тип информации используется 0 1 или 2")]
    public byte indexSection; 

    private Slider sectionSlider;

    [SerializeField] private Section sectionParent;

    private float MaxAngle = 0, MinAngle = 0;

    private void Awake()
    {
     
        if (!Events.indexesActived.Contains(indexSection))
        {
            ImageTypeInfo.gameObject.SetActive(false);

        }
    }
    internal bool IsSectionActive()
    {
        return  Events.indexesActived.Contains(indexSection);

    }

    /// <summary>
    /// Рисует секцию
    /// </summary>
    /// <param name="occupierFraction">какая часть в долях от 0 до 1 занимает выбранная часть</param>
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

            MinAngle = 360 * startValue;
            MaxAngle = 360 * targetValue;

            print($" MinAngle = {MinAngle} MaxAngle = {MaxAngle}  res =  {MinAngle + (MaxAngle - MinAngle) / 2}");
            Vector2 coordinates = GetCoordinatesLabel(MinAngle+(MaxAngle-MinAngle)/2-60, 30);
            ImageTypeInfo.transform.localPosition =  coordinates;

            while (elapsedTime < howFast)
            {
                elapsedTime += Time.deltaTime;
                sectionSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / howFast);
                yield return new WaitForEndOfFrame();
            }
            
        }

        if (sectionParent)
        {
            yield return StartCoroutine(sectionParent.DrawSection(howFast, sizeOneSection, targetValue));
        }
        
       
    }
    internal bool isAreaDropped(float endAngle)
    {
       
        while (endAngle > 360)
        {
            endAngle -= 360;
        }
    
        return endAngle > MinAngle && endAngle < MaxAngle;
    }

    private Vector2 GetCoordinatesLabel(float Angle, float radius = 1)
    {
      
       float x = Mathf.Sin(Angle*(Mathf.PI/180)) *radius;
       float y = Mathf.Cos(Angle* (Mathf.PI / 180)) * radius;
       return new Vector2(x, y);
    }

    private float GetRadiusLabel(float x,float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

}
