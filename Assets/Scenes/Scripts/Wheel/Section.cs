using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;

public class Section : MonoBehaviour
{
    [SerializeField] private Image imageSection;// изображение для отображения секции

    [SerializeField] private TMP_Text textTypeInfo; // надпись с типом контента

    private Slider sectionSlider;

    private Section ? childSection;

    [SerializeField] private float fraction = 0.33333f;
    public void Awake()
    {

        if (transform.childCount > 1)
        {
            transform.parent.TryGetComponent<Section>(out childSection);
        } 

        else 
        {
            Debug.Log("Мало дочерних элементов для новой секции");
        }

    }

    /// <summary>
    /// Рисует секцию
    /// </summary>
    /// <param name="fraction">какая часть в долях от 0 до 1 занимает выбранная часть</param>
    /// <param name="howFast">время в секундах рисовки секции</param>

    public IEnumerator DrawSection(float howFast, float occupiedFraction)
    {
        sectionSlider = GetComponent<Slider>();
        float elapsedTime = 0; // время прошедшее после запуска
        float targetValue = Mathf.Clamp(sectionSlider.value + fraction + occupiedFraction, 0, 1);
        float startValue = sectionSlider.value+ occupiedFraction;
        while (elapsedTime < howFast)
        {
            elapsedTime += Time.deltaTime;
            sectionSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / howFast);
            yield return new WaitForEndOfFrame();
        }
        
        yield return StartCoroutine(childSection?.DrawSection(howFast,targetValue));
    }



}
