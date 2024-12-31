
using System;

public interface IAuthorizationListener
{
    public void OnRegisterMail();
    public void OnVerifiedMailSucceded(string name, int rec, string uid);

    public string mail { get; }
    public string pass { get; }

    public bool isAllDataRight { get; }

    public void OnAuthorizationFailed(AggregateException error);

    public void OnLogInSucceeded (string uid);

}
