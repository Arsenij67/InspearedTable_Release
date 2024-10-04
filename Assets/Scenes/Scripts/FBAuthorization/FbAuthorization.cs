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
    private IAuthorizationListener warningLoggerRegistrationListener;
    private IAuthorizationListener warningLoggerLogInListener;


    private FirebaseAuth FirebaseAuth;

    private void Awake()
    {
        FirebaseAuth = FirebaseAuth.DefaultInstance;
        var obj =  GameObject.FindGameObjectsWithTag("AuthListener");
        warningLoggerRegistrationListener = obj[0].GetComponent<WarningLogger>();
        warningLoggerLogInListener = obj[1].GetComponent<WarningLogger>();

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
    /// ����������� 
    /// </summary>
    private IEnumerator Register(string email, string pass)
    {
        StartCoroutine(Events.ChechInternetConnection(connect =>
        {
            if (connect == false)
            {
                warningLoggerRegistrationListener.OnAuthorizationFailed(new AggregateException("��� ����������� � ���������!"));
 
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
                    warningLoggerRegistrationListener.OnAuthorizationFailed(new System.AggregateException($"������ �����������: {userCreationTask.Exception.Flatten().Message}"));
       
                    yield break; // ����� �� �������� ��� ������
                }

            // �������� ������ ��� ������������� ����������� �����

            StartCoroutine(SendVerificationMail(userCreationTask));

            // �������� �� ������� ������ ��� �������� ������
            

                // �������� ����������� ����������� �����
                bool isVerified = false;

                while (!isVerified)
                {
                    // �������� ����������� ���������� � ������������
                    var userRecordTask = FirebaseAuth.SignInWithEmailAndPasswordAsync (email,pass);
                    yield return new WaitUntil(() => userRecordTask.IsCompleted);

                    if (userRecordTask.Exception != null)
                    {
                        warningLoggerRegistrationListener.OnAuthorizationFailed(new System.AggregateException($"������ ��������� ���������� � ������������: {userRecordTask.Exception.Flatten().Message}"));

                    yield break; // ����� �� �������� ��� ������
                    }

                    // ���������, ������������ �� ����������� �����
                    isVerified = userRecordTask.Result.User.IsEmailVerified;

                    // ���� ����� ��������� ���������
                    yield return new WaitForSeconds(2); // ��������� ������ 2 ������
                }
            //��������� ������ � ��������� �����������
            warningLoggerRegistrationListener.OnVerifiedMail();
            }
            else
            {
                warningLoggerRegistrationListener.OnAuthorizationFailed( new System.AggregateException("������ ��� ����������� �����������."));

            }
        
        }
    /// <summary>
    /// ���� �� ������ � ������
    /// </summary>
    /// <param name="email">����� ��� �����</param>
    /// <param name="pass">������ �� ��������</param>
    private IEnumerator ButtonLogIn(string email, string pass)
    {
        StartCoroutine(Events.ChechInternetConnection(connect =>
        {
            if (connect == false)
            {
                warningLoggerLogInListener.OnAuthorizationFailed(new AggregateException("��� ����������� � ���������!"));

                return;
            }
        }));

        // ��������, ��� �� ������ ���������
        if (warningLoggerLogInListener.isAllDataRight)
        {
            var logIn = FirebaseAuth.SignInWithEmailAndPasswordAsync(email, pass);

            yield return new WaitUntil(predicate: () => logIn.IsCompleted);

            if (logIn.Exception != null)
            {
                warningLoggerLogInListener.OnAuthorizationFailed(new AggregateException("Pass or mail is not correct: " + logIn.Exception.Flatten().Message));
                Debug.Log(logIn.Exception.Flatten().Message);
            }

            else
            {
                AuthResult res = logIn.Result;
                if (res.User.IsEmailVerified) // ���� ����� �� �������������� - �������� ��������������
                {
                    warningLoggerLogInListener.OnLogInSucceeded();

                }

                else
                {
                    warningLoggerLogInListener.OnAuthorizationFailed(new AggregateException("����� �� ������������!"));
                    StartCoroutine(SendVerificationMail(logIn));
                
                }

            }

        }

        else
        {
            warningLoggerLogInListener.OnAuthorizationFailed(new AggregateException("������ ������� �����������!"));
            
        }

       

    }
    private IEnumerator SendVerificationMail(Task<AuthResult> userCreationTask)
    {
        var verificationTask = userCreationTask.Result.User.SendEmailVerificationAsync();

        // ���� ���������� ������ �������� ������
        yield return new WaitUntil(() => verificationTask.IsCompleted);
        //����������� �� �������� ������
        if (verificationTask.Exception != null)
        {
            warningLoggerRegistrationListener.OnAuthorizationFailed(new System.AggregateException($"������ �������� ������ ��� �������������: {verificationTask.Exception.Flatten().Message}"));

            yield break; // ����� �� �������� ��� ������
        }
        warningLoggerRegistrationListener.OnRegisterMail();
        
    }





}
