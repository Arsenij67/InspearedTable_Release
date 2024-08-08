using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAuthorizationListener
{
    public void OnRegisterMail();
    public void OnVerifiedMail();

    public string mail { get; }
    public string pass { get; }

    public bool isAllDataRight { get; }


}
