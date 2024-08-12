using System.IO;
using UnityEngine;
using System.Xml.Linq;
using System;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

/// <summary>
/// класс- синглтон для сохранения служебной информации на девайс
/// </summary>
/// <typeparam name="T"></typeparam>
public class DeviceSaveManager<T>:MonoBehaviour
{
    protected static DeviceSaveManager<T>  InstanceSaveManager { get; private set; }
    private XElement root = new XElement("root");
    string path = Path.Combine(Application.dataPath, "Resources\\Languages", "SystemData.xml"); // путь к системным данным
    private DeviceSaveManager()
    { }
    internal static DeviceSaveManager<T> GetInstance()
    {
        if (InstanceSaveManager == null) InstanceSaveManager = new DeviceSaveManager<T>();
        return InstanceSaveManager;
    }
    private XElement ConvertStringToXML(string key, T value)
    {
        if (!File.Exists(path))
        {
            FileStream fs = File.Create(path);
            using (fs)
                root = new XElement("root");
            fs.Close();
          
        }
        else
        {
            string allText = "";
            using (StreamReader sr = new StreamReader(path))
            {
                allText = sr.ReadToEnd();
                sr.Close();

            }
            root = XDocument.Parse(allText).Element("root");
           
        }
        root.SetAttributeValue(key,value);
        return root;
    }
    public void SaveElement(string key,T value)
    {
       string path = Path.Combine(Application.dataPath, "Resources\\Languages", "SystemData.xml");
       XElement xElement =  ConvertStringToXML(key,value);
       XDocument xDocument = new XDocument(xElement);
        if (!File.Exists(path))
        {
            FileStream fs = File.Create(path);
            using (fs)
                fs.Close();
        }
        StreamWriter sw = new StreamWriter(path);
        using (sw.WriteAsync(xDocument.ToString()))
        {
            sw.Close();
        }
        Debug.Log(path);
    }
    public string GetElement(string key)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Файла по пути не существует");
            return null;
        }
        using (StreamReader sr = new StreamReader(path))
        {
            root = XDocument.Parse(sr.ReadToEnd().ToString()).Element("root");
            sr.Close();
        }
       return root.Attribute(key).Value;
        
    
    }
}
