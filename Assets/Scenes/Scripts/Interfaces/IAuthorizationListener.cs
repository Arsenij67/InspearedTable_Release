using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IAuthorizationListener
{
    public void OnRegisterMail();
    public void OnVerifiedMail();

    public string mail { get; }
    public string pass { get; }

    public bool isAllDataRight { get; }

    public void OnRegisterFailed(AggregateException error);


}
