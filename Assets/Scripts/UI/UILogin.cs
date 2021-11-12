using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public InputField phoneNO;
    public InputField pwd;
    public InputField pwdEnsure;
    public InputField message;
    public Toggle check;
    public Button btnMsgSend;
    public Button Register;

    public void OnClickRegister()
    {
        if (pwd.text!=pwdEnsure.text)
        {

        }
    }

    public void OnClickLogin()
    { 

    }

    public void OnClickGuest()
    {

    }
}
