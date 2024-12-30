using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.UI;
using Firebase;
using System.Linq;
using Firebase.Auth;
 

public class Fb : MonoBehaviour
{
    private DatabaseReference DBRef;

    public DataSnapshot dataSnapshot;

    internal Task InitInfoTask;

    internal FirebaseAuth auth;

    const int MaxCount = 10000;

    private void Awake()
    {
        InitInfoTask = InitInfo();
        print("awake info");
    }

/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
|   Методы
───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

/// <summary>
/// Инициирует данные из Firebase
/// </summary>
public async Task InitInfo()
    {
       
         DBRef = FirebaseDatabase.GetInstance("https://insptable-default-rtdb.firebaseio.com/").RootReference;
        auth = FirebaseAuth.DefaultInstance;
        dataSnapshot =  await ReadData();
       
    }


/// <summary>
/// Скачивание данных для таблицы лидеров
/// </summary>
public async Task<DataSnapshot> ReadData()
    {
     
        var Data  = DBRef.Child("Users").OrderByChild("Record").LimitToFirst(MaxCount).GetValueAsync();

        await  Data;
      
        if (Data.Exception == null)
        {
            print("Данные загружены");
            
        }
        

        else 
        {
           print("Ошибка! "+ Data.Exception);
             
        }

       return Data.Result;
    
    }


/// <summary>
/// удаление старой информации из таблицы при изменении имени
/// </summary>
public async void RemoveData(string Name)
    {
        if (Name != "")
        {
           await DBRef.Child("Users").Child(Name).RemoveValueAsync();
        }
    }


/// <summary>
/// Записывает информацию об игроке на сервер
/// </summary>
/// <param name="name">имя в таблице игроков</param>
/// <param name="rec">рекорд, поставленный игроком</param>
/// <returns></returns>
public  IEnumerator  WriteData(string name,int rec, string uid)
    {

        User user = new User(name,rec, uid);

        var jsonUtility = JsonUtility.ToJson(user);

        var Data = DBRef.Child("Users").Child(name).SetRawJsonValueAsync(jsonUtility);

        yield return new WaitUntil(predicate: () => Data.IsCompleted);


    }


/// <summary>
///  Проверяет в базе есть ли имя которое мы вводим
/// </summary>
/// <param name="name"> имя </param>
/// <returns > сущесвует ли имя или нет</returns>
public bool CheckNameUser(string name)
    {
        try
        {
            return dataSnapshot.Children.Any(dataSnapshot => { return dataSnapshot.Child("Name").Equals(name); });
        }
        catch
        {
            return true;
        }
    }
    public async Task<string> GetRecord()
    {
        var resultSnapshot = await ReadData();
        if (!resultSnapshot.Child("Users").Child((string)SaveTypesFactory.deviceSaveManagerString.GetElement("Name")).Exists) //если имя в базе не существует
        {
            return "0";
        }
        //если существует имя
        return resultSnapshot.Child((string)SaveTypesFactory.deviceSaveManagerString.GetElement("Name")).Child("Record").Value.ToString();
       
    }



public struct User
{
    public string Name;
    public int Record;
    public string UID;
    public User(string name, int record, string uid)
    {
        Name = name;
        Record = record;
        UID = uid;
    }
}
}
