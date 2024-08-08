using Firebase.Auth;
using Firebase;
using System.Collections;
using Firebase.Auth;
using System.Threading.Tasks;

using UnityEngine;
using Unity.VisualScripting;
using System;

public class FbAuthorization : MonoBehaviour
{
    [SerializeField] private IAuthorizationListener warningLoggerRegistrationListener;
    [SerializeField] private IAuthorizationListener warningLoggerLogInListener;


    private FirebaseAuth FirebaseAuth;
   
    private void Awake()
    {
        FirebaseAuth = FirebaseAuth.DefaultInstance;
       var obj =  GameObject.FindGameObjectsWithTag("AuthListener");
        warningLoggerRegistrationListener = obj[0].GetComponent<WarningLogger>();
        warningLoggerLogInListener = obj[1].GetComponent<WarningLogger>();

        print(obj[0].name);
        print(obj[1].name);

    }
    /// <summary>
    /// Вход по логину и паролю
    /// </summary>
    /// <param name="email">почта как логин</param>
    /// <param name="pass">пароль от аккаунта</param>
    public IEnumerator ButtonLogIn(string email, string pass)
    {
        StartCoroutine(Events.ChechInternetConnection(connect =>
        {
            if (connect == false)
            {
                warningLoggerLogInListener.OnAuthorizationFailed(new AggregateException("Нет подключения к интернету!"));
                return;
            }
        }));

        // Проверка, все ли данные корректны
        if (warningLoggerLogInListener.isAllDataRight)
        {
            var logIn = FirebaseAuth.SignInWithEmailAndPasswordAsync(email, pass);

            yield return new WaitUntil(predicate: () => logIn.IsCompleted);

            if (logIn.Exception != null)
            {
                warningLoggerLogInListener.OnAuthorizationFailed(new AggregateException(logIn.Exception.Flatten().Message));
                Debug.Log(logIn.Exception.Flatten().Message);
            }

            else
            {

                warningLoggerLogInListener.OnLogInSucceeded();
                AuthResult res = logIn.Result;

            }
        }

        else
        {
            warningLoggerLogInListener.OnAuthorizationFailed(new AggregateException("Данные введены некорректно!"));
        }

    }
    public void Register()
    {
        StartCoroutine(Register(warningLoggerRegistrationListener.mail, warningLoggerRegistrationListener.pass));
    }

    public void LogIn()
    {
        StartCoroutine(ButtonLogIn(warningLoggerLogInListener.mail, warningLoggerLogInListener.pass));
    }

    /// <summary>
    /// регистрация 
    /// </summary>
    private IEnumerator Register(string email, string pass)
    {
        StartCoroutine(Events.ChechInternetConnection(connect =>
        {
            if (connect == false)
            {
                warningLoggerRegistrationListener.OnAuthorizationFailed(new AggregateException("Нет подключения к интернету!"));
                return;
            }
        }));

            // Проверка, все ли данные корректны
            if (warningLoggerRegistrationListener.isAllDataRight)
            {
              // Начинаем процесс регистрации
                var userCreationTask = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, pass);

                // Ждем завершения задачи регистрации
                yield return new WaitUntil(() => userCreationTask.IsCompleted);

                // Проверка на наличие ошибок при регистрации
                if (userCreationTask.Exception != null)
                {
                    warningLoggerRegistrationListener.OnAuthorizationFailed(new System.AggregateException($"Ошибка регистрации: {userCreationTask.Exception.Flatten().Message}"));
                    yield break; // Выход из корутины при ошибке
                }

                // Отправка письма для подтверждения электронной почты
                var verificationTask = userCreationTask.Result.User.SendEmailVerificationAsync();
 
    

                // Ждем завершения задачи отправки письма
                yield return new WaitUntil(() => verificationTask.IsCompleted);
            //уведомление об отправке письма
                warningLoggerRegistrationListener.OnRegisterMail();
              

            // Проверка на наличие ошибок при отправке письма
            if (verificationTask.Exception != null)
                {
                    warningLoggerRegistrationListener.OnAuthorizationFailed(new System.AggregateException($"Ошибка отправки письма для подтверждения: {verificationTask.Exception.Flatten().Message}"));
                    yield break; // Выход из корутины при ошибке
                }

                // Проверка верификации электронной почты
                bool isVerified = false;

                while (!isVerified)
                {
                    // Получаем обновленную информацию о пользователе
                    var userRecordTask = FirebaseAuth.SignInWithEmailAndPasswordAsync (email,pass);
                    yield return new WaitUntil(() => userRecordTask.IsCompleted);

                    if (userRecordTask.Exception != null)
                    {
                        warningLoggerRegistrationListener.OnAuthorizationFailed(new System.AggregateException($"Ошибка получения информации о пользователе: {userRecordTask.Exception.Flatten().Message}"));
                        yield break; // Выход из корутины при ошибке
                    }

                    // Проверяем, подтверждена ли электронная почта
                    isVerified = userRecordTask.Result.User.IsEmailVerified;

                    // Логируем статус проверки
                    Debug.Log($"Статус верификации электронной почты: {isVerified}");

                    // Ждем перед следующей проверкой
                    yield return new WaitForSeconds(2); // Проверяем каждые 2 секунд
                }
            //закрываем панель с ожиданием верификации
            warningLoggerRegistrationListener.OnVerifiedMail();
            }
            else
            {
                warningLoggerRegistrationListener.OnAuthorizationFailed( new System.AggregateException("Данные для регистрации некорректны."));
              
            }
        }

    





}
