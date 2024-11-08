

public static class SaveTypesFactory
{
    public static DeviceSaveManager<string> deviceSaveManagerString =  DeviceSaveManager<string>.GetInstance();
    public static DeviceSaveManager<int> deviceSaveManagerInteger = DeviceSaveManager<int>.GetInstance();
}
