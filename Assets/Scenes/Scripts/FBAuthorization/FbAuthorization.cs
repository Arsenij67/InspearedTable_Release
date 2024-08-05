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
                var userCreationTask = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, pass);

                // Ждем завершения задачи регистрации
                yield return new WaitUntil(() => userCreationTask.IsCompleted);
                Debug.Log(2);

                // Проверка на наличие ошибок при регистрации
                if (userCreationTask.Exception != null)
                {
                    Debug.LogError($"Ошибка регистрации: {userCreationTask.Exception.Flatten().Message}");
                    yield break; // Выход из корутины при ошибке
                }

                // Отправка письма для подтверждения электронной почты
                var verificationTask = userCreationTask.Result.User.SendEmailVerificationAsync();

                // Запуск корутины для ожидания подтверждения
                warningLoggerRegistration.actionButton.interactable = false;

                // Ждем завершения задачи отправки письма
                yield return new WaitUntil(() => verificationTask.IsCompleted);

                // Проверка на наличие ошибок при отправке письма
                if (verificationTask.Exception != null)
                {
                    Debug.LogError($"Ошибка отправки письма для подтверждения: {verificationTask.Exception.Flatten().Message}");
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
                        Debug.LogError($"Ошибка получения информации о пользователе: {userRecordTask.Exception.Flatten().Message}");
                        yield break; // Выход из корутины при ошибке
                    }

                    // Проверяем, подтверждена ли электронная почта
                    isVerified = userRecordTask.Result.User.IsEmailVerified;

                    // Логируем статус проверки
                    Debug.Log($"Статус верификации электронной почты: {isVerified}");

                    // Ждем перед следующей проверкой
                    yield return new WaitForSeconds(2); // Проверяем каждые 2 секунд
                }

                warningLoggerRegistration.actionButton.interactable = true;
            }
            else
            {
                Debug.LogWarning("Данные для регистрации некорректны.");
            }
        }

    





}
