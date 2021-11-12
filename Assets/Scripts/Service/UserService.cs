using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPFSDK;
using UnityEngine;
using Msg;
using ProtoBuf;

namespace Service
{
    public class UserService : Singleton<UserService>
    {

        public UserService()
        {
            MsgHandleMgr.Instance.RegisterHandle("UserLoginRequest", OnLogin);
        }

        private void OnLogin(IExtensible res)
        {
            Debug.LogError("receive");
        }

        public void login(string account,string password)
        { // login

            TPFSdkInfo info = new TPFSdkInfo();
            info.SetData("account", "qiyu111");
            info.SetData("accountType", "0");
            info.SetData("password", "qiyu111");
            info.SetData("type", "1");
            info.SetData("checkType", "0");

            ITPFSdk.Instance.Login(info, (p) => { Debug.Log("On login callback"); });
            //TPFSdkEventDelegate m_event;
            //m_event = Show;
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
                    Debug.Log("On Register callback");
                }
            );
        }

        public void Show(TPFSdkEvent e)
        {

            Debug.Log("Login Test");
        }

        public void LoginServer(string userId, string token)
        {

            UserLoginRequest msg = new UserLoginRequest();
            msg.UserId = userId;
            msg.Token = token;

            MsgHandleMgr.Instance.AddMsg(msg);
            //ClientNetMgr.Instance.SendRequest("UserLoginRequest", msg, "http://127.0.0.1:31506", "world-service:31001");

        }
    }
}
