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
    /// ���� �� ������ � ������
    /// </summary>
    /// <param name="email">����� ��� �����</param>
    /// <param name="pass">������ �� ��������</param>
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
            Debug.Log("���� ��������!");
            AuthResult res = logIn.Result;


        }

    }
    public void Register()
    {
        StartCoroutine(Register(warningLoggerRegistration.mail, warningLoggerRegistration.pass));
    }

    /// <summary>
    /// ����������� 
    /// </summary>


        private IEnumerator Register(string email, string pass)
        {
            // ��������, ��� �� ������ ���������
            if (warningLoggerRegistration.isAllDataRight)
            {
                Debug.Log(1);
                // �������� ������� �����������
                var userCreationTask = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, pass);

                // ���� ���������� ������ �����������
                yield return new WaitUntil(() => userCreationTask.IsCompleted);
                Debug.Log(2);

                // �������� �� ������� ������ ��� �����������
                if (userCreationTask.Exception != null)
                {
                    Debug.LogError($"������ �����������: {userCreationTask.Exception.Flatten().Message}");
                    yield break; // ����� �� �������� ��� ������
                }

                // �������� ������ ��� ������������� ����������� �����
                var verificationTask = userCreationTask.Result.User.SendEmailVerificationAsync();

                // ������ �������� ��� �������� �������������
                warningLoggerRegistration.actionButton.interactable = false;

                // ���� ���������� ������ �������� ������
                yield return new WaitUntil(() => verificationTask.IsCompleted);

                // �������� �� ������� ������ ��� �������� ������
                if (verificationTask.Exception != null)
                {
                    Debug.LogError($"������ �������� ������ ��� �������������: {verificationTask.Exception.Flatten().Message}");
                    yield break; // ����� �� �������� ��� ������
                }

                // �������� ����������� ����������� �����
                bool isVerified = false;

                while (!isVerified)
                {
                    // �������� ����������� ���������� � ������������
                    var userRecordTask = FirebaseAuth.SignInWithEmailAndPasswordAsync (email,pass);
                    yield return new WaitUntil(() => userRecordTask.IsCompleted);

                    if (userRecordTask.Exception != null)
                    {
                        Debug.LogError($"������ ��������� ���������� � ������������: {userRecordTask.Exception.Flatten().Message}");
                        yield break; // ����� �� �������� ��� ������
                    }

                    // ���������, ������������ �� ����������� �����
                    isVerified = userRecordTask.Result.User.IsEmailVerified;

                    // �������� ������ ��������
                    Debug.Log($"������ ����������� ����������� �����: {isVerified}");

                    // ���� ����� ��������� ���������
                    yield return new WaitForSeconds(2); // ��������� ������ 2 ������
                }

                warningLoggerRegistration.actionButton.interactable = true;
            }
            else
            {
                Debug.LogWarning("������ ��� ����������� �����������.");
            }
        }

    





}
