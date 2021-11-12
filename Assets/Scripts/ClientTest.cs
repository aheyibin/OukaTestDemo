using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TPFSDK;
using UnityEngine;
using Service;
using UnityEngine.UI;

public class ClientTest : MonoBehaviour
{
    public Button btn;
    public Toggle toggle;

    private void Start()
    {
    }

    public void Socket_test()
    {
        Http("http://127.0.0.1:31506/ClientMsgHead","111111111111");
        Debug.Log("Socket_test");
    }

    public static string Http(string url,string data)
    {
        Hashtable header = null;
        string method = "POST";
        string contenttype = "application/json;charset=utf-8";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = string.IsNullOrEmpty(method) ? "GET" : method;
        request.ContentType = string.IsNullOrEmpty(contenttype) ? "application/json;charset=utf-8" : contenttype;
        if (header != null)
        {
            foreach (var i in header.Keys)
            {
                request.Headers.Add(i.ToString(), header[i].ToString());
            }
        }
        if (!string.IsNullOrEmpty(data))
        {
            Stream RequestStream = request.GetRequestStream();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            RequestStream.Write(bytes, 0, bytes.Length);
            RequestStream.Close();
        }
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream ResponseStream = response.GetResponseStream();
        StreamReader StreamReader = new StreamReader(ResponseStream, Encoding.GetEncoding("utf-8"));
        string re = StreamReader.ReadToEnd();
        StreamReader.Close();
        ResponseStream.Close();
        return re;
    }

    public void login()
    { // login

        TPFSdkEventDelegate m_event;
        m_event = Show;
        TPFSdkInfo info = new TPFSdkInfo();
        info.SetData("account", "qiyu111");
        info.SetData("accountType", "0");
        info.SetData("password", "qiyu111");
        info.SetData("type", "1");
        info.SetData("checkType", "0");

        ITPFSdk.Instance.Login(info, (p) => { Debug.Log("On login callback"); });
        //ITPFSdk.Instance.Login(info, m_event);
    }
    public void Register()
    { // Register
            TPFSdkInfo info = new TPFSdkInfo();
            info.SetData("type", 0);
            info.SetData("account", "myAccount");
            info.SetData("password", "myPassword");
            ITPFSdk.Instance.Register(info,
                (p) =>
                {
                    Debug.Log(p.ErrorMessage); 
                    Debug.Log("On Register callback");
                }
            );

        TPFSdkInfo info2 = new TPFSdkInfo();
        info2.SetData("type", 1);
        info2.SetData("phoneNum", "15915410252");
        ITPFSdk.Instance.VerifyCode(info2,
                (sdkEvent) =>
                {
                    Debug.Log(string.Format("{0} -- {1}", sdkEvent.ErrorCode, sdkEvent.ErrorMessage));
                    Debug.Log("On Register callback");
                }
            );
    }

    public void LoginServer()
    {
        UserService.Instance.LoginServer("ouka","123456");   
    }

    public void Show(TPFSdkEvent e)
    {

        Debug.Log("Login Test");
    }
}
