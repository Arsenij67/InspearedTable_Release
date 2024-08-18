using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveTypesFactory
{
    public static DeviceSaveManager<string> deviceSaveManagerString =  DeviceSaveManager<string>.GetInstance();
    public static DeviceSaveManager<int> deviceSaveManagerInteger = DeviceSaveManager<int>.GetInstance();
}
