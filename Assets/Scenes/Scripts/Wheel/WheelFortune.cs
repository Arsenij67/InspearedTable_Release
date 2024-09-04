using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelFortune : MonoBehaviour
{
    [SerializeField] private Section mainSection;
    private byte countSections = 0;
    private Section lastSection;
    private void Awake()
    {
      GetLastSection(mainSection,out countSections, out lastSection);
       StartCoroutine(lastSection.DrawSection(0.4f, 0f));
        
    }

    private  void GetLastSection(Section sect,out byte depth,out Section lastSection)
    {
        depth = 1;
        lastSection = sect;
        while (lastSection.transform.childCount > 1 && lastSection.transform.GetChild(1).GetComponent<Section>()!=null) 
        {
           
            lastSection = lastSection.transform.GetChild(1).GetComponent<Section>();
            depth++;
        }
    }


}
