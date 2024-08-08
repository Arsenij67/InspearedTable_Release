using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingBoard : MonoBehaviour
{

    [HideInInspector]public bool InternetExist = false;
   

    //public async Task GetLoadingBoard()
    //{
    //    Transform Circle = transform.GetChild(1).transform;
    //    float AngleRotate = 60;
    //    while (!InternetExist)
    //    {
    //        gameObject.SetActive(true);
    //        AngleRotate += 60;
    //        await Circle.DOLocalRotate(new Vector3(0, 0, AngleRotate), 2f).Play().AsyncWaitForCompletion();
    //    }
    //    RemoveLoadingBoard();

    //}

    //public void RemoveLoadingBoard()
    //{

    //    gameObject.SetActive(false);

    //}
}
