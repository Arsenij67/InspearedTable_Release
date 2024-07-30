using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WarningLogger : MonoBehaviour
{
    [SerializeField] private TMP_Text [] warningTextList;

    private Dictionary<string, TMP_Text> warningTextDict;

    private void Awake()
    {
        warningTextDict = new Dictionary<string, TMP_Text>()
        {
            {"mail", warningTextList[0] }, { "pass1", warningTextList[1] },{ "pass2", warningTextList[2]}
        };
    } 

}
