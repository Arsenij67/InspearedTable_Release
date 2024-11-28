using System.IO;
using UnityEngine;
using System.Xml.Linq;
using System;
using Newtonsoft.Json.Linq;

/// <summary>
/// Менеджер для сохранения и загрузки данных устройства в формате XML.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DeviceSaveManager<T> 
{
    private static DeviceSaveManager<T> instanceSaveManager;
    private static XElement root = new XElement("root");
    private static string path;

    // Получение экземпляра менеджера
    internal static DeviceSaveManager<T> GetInstance()
    {
        if (instanceSaveManager == null)
        {
            instanceSaveManager = new DeviceSaveManager<T>();
        }
        return instanceSaveManager;
    }

    public void Init()
    {
        Debug.Log("ffffffffff");
        path = Path.Combine(Application.persistentDataPath, "SystemData.xml");
        LoadOrCreateFile();
        
    }
    private static void LoadOrCreateFile()
    {
        if (!File.Exists(path))
        {
            CreateFile(path);
            root = new XElement("root");
            root = ConvertStringToXML("Name", default(T));
            SaveToFile();
             
        }
        else
        {
            string allText = File.ReadAllText(path);
            root = XDocument.Parse(allText).Element("root");
        }
        Debug.Log(string.Format($"Created {path}"));
    }

    private static void SaveToFile()
    {
        XDocument xDocument = new XDocument(root);
        File.WriteAllText(path, xDocument.ToString());
    }

    private static XElement ConvertStringToXML(string key, T value)
    {
        root.SetAttributeValue(key, value);
        return root;
    }

    public  void SaveElement(string key, T value)
    {
        root = ConvertStringToXML(key, value);
        SaveToFile();
    }

    public T GetElement(string key)
    {
        if (root.Attribute(key) != null)
        {
            return (T)Convert.ChangeType(root.Attribute(key).Value, typeof(T));
        }
        else
        {
            SaveElement(key, default(T));
            return default(T);
        }
    }

    private static void CreateFile(string path)
    {
        using (FileStream fs = File.Create(path))
        {
            // Файл создан, можно добавить начальное содержимое, если нужно
        }
    }

    
}
public class InvokerSystemData
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        SaveTypesFactory.deviceSaveManagerString.Init();
        SaveTypesFactory.deviceSaveManagerInteger.Init();
    }

}
