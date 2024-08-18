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
    private DeviceSaveManager()
    { 
    
    }
    protected static DeviceSaveManager<T>  InstanceSaveManager { get; private set; }
    private XElement root = new XElement("root");
    private string path;
    internal static DeviceSaveManager<T> GetInstance()
    {
        if (InstanceSaveManager == null) InstanceSaveManager = new DeviceSaveManager<T>();
        return InstanceSaveManager;
    }
    private XElement ConvertStringToXML(string key, T value)
    {
        print(!File.Exists(path));
        if (!File.Exists(path)/*|| File.ReadAllText(path).Length<1*/)
        {
            CreateFile(path);
            root = new XElement("root");
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
       print("Element save " + key);
       string path = Path.Combine(Application.dataPath, "Resources\\Languages", "SystemData.xml");
       XElement xElement =  ConvertStringToXML(key,value);
       XDocument xDocument = new XDocument(xElement);
        StreamWriter sw = new StreamWriter(path);
        using (sw.WriteAsync(xDocument.ToString()))
        {
            sw.Close();
        }
        Debug.Log(path);
    }
    public object GetElement(string key)
    {
        path =  Path.Combine(Application.dataPath, "Resources\\Languages", "SystemData.xml"); // путь к системным данным
        if (!File.Exists(path))
        {
            Debug.Log("Файла по пути не существует, поэтому создан");
            SaveElement(key, default(T));
            return null is T;
        }
        using (StreamReader sr = new StreamReader(path))
        {
            root = XDocument.Parse(sr.ReadToEnd().ToString()).Element("root");
            sr.Close();
        }
        if (root.Attribute(key) != null)
        {
            return root.Attribute(key).Value;
        }
        else
        {
            SaveElement(key, default(T));
            return root.Attribute(key) is T;

        }
    }

    private void CreateFile(string path)
    {
        FileStream fs = File.Create(path);
        using (fs)
            fs.Close();
    }
}
