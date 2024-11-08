
using System;

public interface IAuthorizationListener
{
    public void OnRegisterMail();
    public void OnVerifiedMail();

    public string mail { get; }
    public string pass { get; }

    public bool isAllDataRight { get; }

    public void OnAuthorizationFailed(AggregateException error);

    public void OnLogInSucceeded ();


}
