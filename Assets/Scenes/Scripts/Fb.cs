using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase;

public class Fb : MonoBehaviour
{
    private DatabaseReference DBRef;

    public static string MyName = "";

    public DataSnapshot dataSnapshot;

    const int MaxCount = 10000;

    public  Task<DataSnapshot> Data = null;

    FirebaseAuth FirebaseAuth;

    [SerializeField] private InputField mailField, passField;

    
    private void Awake()
    {
    
     InitInfo();
        
    }

    public async void InitInfo()
    {
        DBRef = FirebaseDatabase.GetInstance("https://insptable-default-rtdb.firebaseio.com/").RootReference;
        FirebaseAuth = FirebaseAuth.DefaultInstance;

        await ReadData();

        MyName = dataSnapshot.Child(PlayerPrefs.GetString("Name")).Child("Name").Value.ToString();

    }
  
    public async Task ReadData()
    {
        Data  = DBRef.Child("Users").OrderByChild("Record").LimitToFirst(MaxCount).GetValueAsync();

        await  Data;
        
    
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
    /// <returns > сущесвует ли имя или нет</returns>
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

    public IEnumerator ButtonLogIn(string email,string login)
    {
      var logIn =  FirebaseAuth.SignInWithEmailAndPasswordAsync(email, login);

      yield return new WaitUntil(predicate:()=> logIn.IsCompleted);

        if (logIn.Exception != null)
        {
            Debug.Log(logIn.Exception.GetBaseException() as FirebaseException);
        }

        else
        {
            Debug.Log("Вход выполнен!");
        
        }



    }

    public void ButtonRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        Task<AuthResult> auth = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(mailField.text, passField.text);
        
        yield return new WaitUntil(predicate: () => auth.IsCompleted);
     
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
