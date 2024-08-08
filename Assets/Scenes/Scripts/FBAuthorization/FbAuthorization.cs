using Firebase.Auth;
using Firebase;
using System.Collections;
using Firebase.Auth;
using System.Threading.Tasks;

using UnityEngine;

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
        StartCoroutine(Register(warningLoggerRegistrationListener.mail, warningLoggerRegistrationListener.pass));
    }

    /// <summary>
    /// ����������� 
    /// </summary>
    private IEnumerator Register(string email, string pass)
        {
            // ��������, ��� �� ������ ���������
            if (warningLoggerRegistrationListener.isAllDataRight)
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
 
    

                // ���� ���������� ������ �������� ������
                yield return new WaitUntil(() => verificationTask.IsCompleted);
            //����������� �� �������� ������
            warningLoggerRegistrationListener.OnRegisterMail();
              

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
            //��������� ������ � ��������� �����������
            warningLoggerRegistrationListener.OnVerifiedMail();
            }
            else
            {
                Debug.LogWarning("������ ��� ����������� �����������.");
            }
        }

    





}
