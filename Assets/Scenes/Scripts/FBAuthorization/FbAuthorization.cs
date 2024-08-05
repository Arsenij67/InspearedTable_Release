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
            user = FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, pass);
            Debug.Log(2);
        // ���� ���������� ������ �����������
        yield return new WaitUntil(() => user.IsCompleted);

        // �������� �� ������� ������ ��� �����������
        if (user.Exception != null)
        {
            Debug.LogError($"������ �����������: {user.Exception.Flatten().Message}");
            yield break; // ����� �� �������� ��� ������
        }

        // �������� ������ ��� ������������� ����������� �����
      
        var  verificationTask = user.Result.User.SendEmailVerificationAsync();

        // ���� ���������� ������ �������� ������
        yield return new WaitUntil(() => verificationTask.IsCompleted);

        // �������� �� ������� ������ ��� �������� ������
        if (verificationTask.Exception != null)
        {
            Debug.LogError($"������ �������� ������ ��� �������������: {verificationTask.Exception.Flatten().Message}");
            yield break; // ����� �� �������� ��� ������
        }

        // ������ �������� ��� �������� �������������
        StartCoroutine(WaitForVerification());
    }
    else
    {
        Debug.LogWarning("������ ��� ����������� �����������.");
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
