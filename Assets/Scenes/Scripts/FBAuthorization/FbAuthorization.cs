using Firebase.Auth;
using Firebase;
using System.Collections;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;

public class FbAuthorization : MonoBehaviour
{
    [SerializeField] private WarningLogger? warningLoggerRegistration;
    [SerializeField] private WarningLogger? warningLoggerLogIn;
    Task<AuthResult> user = null;

    private FirebaseAuth FirebaseAuth;
    private void Awake()
    {
        FirebaseAuth = FirebaseAuth.DefaultInstance;
    }
    /// <summary>
    /// Вход по логину и паролю
    /// </summary>
    /// <param name="email">почта как логин</param>
    /// <param name="pass">пароль от аккаунта</param>
    public IEnumerator ButtonLogIn(string email, string pass)
    {
        var logIn = FirebaseAuth.SignInWithEmailAndPasswordAsync(email, pass);

        yield return new WaitUntil(predicate: () => logIn.IsCompleted);

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
        StartCoroutine(Register(warningLoggerRegistration.mail, warningLoggerRegistration.pass));
    }

    /// <summary>
    /// регистрация 
    /// </summary>
    private IEnumerator Register(string email, string pass)
{
         
    // Проверка, все ли данные корректны
    if (warningLoggerRegistration.isAllDataRight)
    {
            Debug.Log(1);
            // Начинаем процесс регистрации
            user = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, pass);
            Debug.Log(2);
        // Ждем завершения задачи регистрации
        yield return new WaitUntil(() => user.IsCompleted);

        // Проверка на наличие ошибок при регистрации
        if (user.Exception != null)
        {
            Debug.LogError($"Ошибка регистрации: {user.Exception.Flatten().Message}");
            yield break; // Выход из корутины при ошибке
        }

        // Отправка письма для подтверждения электронной почты
      
        var  verificationTask = user.Result.User.SendEmailVerificationAsync();

        // Ждем завершения задачи отправки письма
        yield return new WaitUntil(() => verificationTask.IsCompleted);

        // Проверка на наличие ошибок при отправке письма
        if (verificationTask.Exception != null)
        {
            Debug.LogError($"Ошибка отправки письма для подтверждения: {verificationTask.Exception.Flatten().Message}");
            yield break; // Выход из корутины при ошибке
        }

        // Запуск корутины для ожидания подтверждения
        StartCoroutine(WaitForVerification());
    }
    else
    {
        Debug.LogWarning("Данные для регистрации некорректны.");
    }
}


    private IEnumerator WaitForVerification()
    {
        
        warningLoggerRegistration.actionButton.interactable = false;
        yield return new WaitUntil(predicate: () => user.Result.User.IsEmailVerified);
        warningLoggerRegistration.actionButton.interactable = true;

    }
    
    private void Update()
    {
        if (user != null)
        Debug.Log (user.Result.User.IsEmailVerified);
        
    }
}
