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

    private FirebaseAuth FirebaseAuth;

    [SerializeField] private WarningLogger warningLoggerRegistration;
    [SerializeField] private WarningLogger warningLoggerLogIn;

    private void Awake()
    {
    
        InitInfo();
        
    }

/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
|   Методы
───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

/// <summary>
/// Инициирует данные из Firebase
/// </summary>
public async void InitInfo()
    {
        DBRef = FirebaseDatabase.GetInstance("https://insptable-default-rtdb.firebaseio.com/").RootReference;
        FirebaseAuth = FirebaseAuth.DefaultInstance;

       dataSnapshot =  await ReadData();

        MyName = dataSnapshot.Child(PlayerPrefs.GetString("Name")).Child("Name").Value.ToString();

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
public void RemoveData(string Name)
    {
        if (Name != "")
        {
            DBRef.Child("Users").Child(Name).RemoveValueAsync();
        }
    }


/// <summary>
/// Записывает информацию об игроке на сервер
/// </summary>
/// <param name="name">имя в таблице игроков</param>
/// <param name="rec">рекорд, поставленный игроком</param>
/// <returns></returns>
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

                return IsNameExist;


            }

            
        }

            return IsNameExist;

    }


/// <summary>
/// Вход по логину и паролю
/// </summary>
/// <param name="email">почта как логин</param>
/// <param name="pass">пароль от аккаунта</param>
public IEnumerator ButtonLogIn(string email,string pass)
    {
      var logIn =  FirebaseAuth.SignInWithEmailAndPasswordAsync(email, pass);

      yield return new WaitUntil(predicate:()=> logIn.IsCompleted);

        if (logIn.Exception != null)
        {
            Debug.Log(logIn.Exception.GetBaseException() as FirebaseException);
        }

        else
        {
            Debug.Log("Вход выполнен!");
            AuthResult res = logIn.Result;
 
        
        }

    }

    public void Register()
    {
        StartCoroutine(Register(warningLoggerRegistration.mail,warningLoggerRegistration.pass));
    }

/// <summary>
/// регистрация 
/// </summary>
private IEnumerator Register(string email, string pass)
    {
        Task<AuthResult> auth = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, pass);
        
        yield return new WaitUntil(predicate: () => auth.IsCompleted);

        yield return auth.Result.User.SendEmailVerificationAsync();
    }


public struct User
{
    public string Name;
    public int Record;
    public User(string name, int record)
    {
        Name = name;
        Record = record;
    }
}
}
