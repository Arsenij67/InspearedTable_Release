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
        StartCoroutine(Events.ChechInternetConnection(connect =>
        {
            if (connect == false)
            {
                warningLoggerRegistrationListener.OnRegisterFailed(new AggregateException("��� ����������� � ���������!"));
                return;
            }
        }));

            // ��������, ��� �� ������ ���������
            if (warningLoggerRegistrationListener.isAllDataRight)
            {
              // �������� ������� �����������
                var userCreationTask = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, pass);

                // ���� ���������� ������ �����������
                yield return new WaitUntil(() => userCreationTask.IsCompleted);

                // �������� �� ������� ������ ��� �����������
                if (userCreationTask.Exception != null)
                {
                    warningLoggerRegistrationListener.OnRegisterFailed(new System.AggregateException($"������ �����������: {userCreationTask.Exception.Flatten().Message}"));
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
                    warningLoggerRegistrationListener.OnRegisterFailed(new System.AggregateException($"������ �������� ������ ��� �������������: {verificationTask.Exception.Flatten().Message}"));
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
                        warningLoggerRegistrationListener.OnRegisterFailed(new System.AggregateException($"������ ��������� ���������� � ������������: {userRecordTask.Exception.Flatten().Message}"));
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
                warningLoggerRegistrationListener.OnRegisterFailed( new System.AggregateException("������ ��� ����������� �����������."));
              
            }
        }

    





}
