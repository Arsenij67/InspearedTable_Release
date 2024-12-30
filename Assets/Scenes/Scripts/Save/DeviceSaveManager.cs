using System.IO;
using UnityEngine;
using System.Xml.Linq;
using System;
using Newtonsoft.Json.Linq;
using System.Xml;
using DG.Tweening.Plugins.Core.PathCore;

/// <summary>
/// �������� ��� ���������� � �������� ������ ���������� � ������� XML.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DeviceSaveManager<T> 
{
    private static DeviceSaveManager<T> instanceSaveManager;
    private static XmlElement root = null;
    private static XmlDocument xmlDocument = new XmlDocument();
    private string path = "";

    // ��������� ���������� ���������
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
       
        path = System.IO.Path.Combine(Application.persistentDataPath, "SystemData.xml");
        LoadOrCreateFile(path);
        
    }
    private static void LoadOrCreateFile(string path)
    {
        Debug.Log(path);
        if (!File.Exists(path)) // ���� �� ����������
        {
            Debug.Log(" �� ����������");
            xmlDocument = CreateFile(path);

            xmlDocument.Load(path);
            root = xmlDocument.DocumentElement;

            //��������� ��������
            XmlElement langElem =  xmlDocument.CreateElement("Language");
            XmlElement numberEnterElem = xmlDocument.CreateElement("NumberEnter");
            XmlElement NameElem = xmlDocument.CreateElement("Name");
            XmlElement mailElem = xmlDocument.CreateElement("Mail");
           
            XmlText textLang = xmlDocument.CreateTextNode("ru");
            XmlText textName = xmlDocument.CreateTextNode("��� �����������");
            XmlText textNumberEnter = xmlDocument.CreateTextNode("0");
            XmlText textMail = xmlDocument.CreateTextNode("����� �����������");
            
            langElem.AppendChild(textLang);
            numberEnterElem.AppendChild(textNumberEnter);
            NameElem.AppendChild(textName);
            mailElem.AppendChild(textMail);

            // ����������� � �����
            root.AppendChild(langElem);
            root.AppendChild(numberEnterElem);
            root.AppendChild(NameElem);
            root.AppendChild(mailElem);
            xmlDocument.Save(path);

        }
        else if(new FileInfo(path).Length>0) // ���� ���� ���������� � �� ����
        {  
          xmlDocument.Load(path);
          root = xmlDocument.DocumentElement;
        }
   
    }




    public object GetElement(string name)
    {
        LoadOrCreateFile(path);
        var nodeList =  root.SelectNodes(name);
        return nodeList[0].InnerText;
    }

    public void SaveElement(string key, object value)
    {
        LoadOrCreateFile(path);
        var nodeList = root.SelectSingleNode(key);
        XmlNode changedNode = nodeList;
        changedNode.InnerText = value.ToString();
        root.AppendChild(changedNode);
        xmlDocument.Save(path);
       
    }


    private static XmlDocument CreateFile(string path)

    {   // ������� XmlDocument � ��������� �������� �������
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement rootElem = xmlDocument.CreateElement("SystemData");
            xmlDocument.AppendChild(rootElem);
            // ��������� �������� � ��������� ����
            xmlDocument.Save(path);
            return xmlDocument;
         
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
