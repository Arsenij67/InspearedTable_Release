using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;

public class Fb : MonoBehaviour
{
    private DatabaseReference DBRef;

    public static string MyName = "";

    public DataSnapshot dataSnapshot;

    const int MaxCount = 10000;

    public  Task<DataSnapshot> Data = null;
 

    private void Awake()
    {
    
            InitInfo();
        
    }

    public void InitInfo()
    {
        DBRef = FirebaseDatabase.GetInstance("https://insptable-default-rtdb.firebaseio.com/").RootReference;


        StartCoroutine(ReadData());
    }
  

    public IEnumerator ReadData()
    {
        Data  = DBRef.Child("Users").OrderByChild("Record").LimitToFirst(MaxCount).GetValueAsync();


        yield return new WaitUntil(predicate: () => Data.IsCompleted);
    
        if (Data.Exception == null)
        {
            print("Данные загружены");
            
        }
        
        

        else {
            print("Ошибка! "+ Data.Exception);
             
        }

       dataSnapshot = Data.Result;


    }

    public void RemoveData(string Name) { if (Name != "") DBRef.Child("Users").Child(Name).RemoveValueAsync(); }

    

    public  IEnumerator  WriteData(string name,int rec)
    {

        User user = new User(name,rec);

       

        var jsonUtility = JsonUtility.ToJson(user);

        var Data = DBRef.Child("Users").Child(name).SetRawJsonValueAsync(jsonUtility);

        return new WaitUntil(predicate: () => Data.IsCanceled);

        
     

      

    }

 

    /// <summary>
    ///  Проверяет в базе есть ли имя которое мы вводим
    /// </summary>
    /// <param name="name"> имя </param>
    /// <returns></returns>
    public bool CheckData(string name)
    {
        bool IsNameExist = false;

        
        

        foreach (var snapshot in dataSnapshot.Children)
        {

            if (snapshot.Child("Name").Value.ToString() == name)
            {
               
                IsNameExist = true;

                dataSnapshot = null;

                Data = null;

                return IsNameExist;


            }

            
        }



            return IsNameExist;

        
    }


    public struct User
    {


        public string Name;
        public int Record;
        public  User(string name,int record)

        {
        Name = name;

        Record = record;
        
        
        }

    }


}
