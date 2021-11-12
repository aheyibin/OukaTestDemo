/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的调用安卓
** use: demo
** modify:             
*******************************************************************/
//#define UNITY_ANDROID

#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TPFSDK.Normal.UnityPlugins.Passive;
using UnityEngine;

namespace TPFSDK
{

    public class TPFSdkAndroid : ITPFSdk
    {

        private AndroidJavaObject jo;

        public TPFSdkAndroid()
        {
            //using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            //{
            //    jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            //}

            using (AndroidJavaClass jc = new AndroidJavaClass("com.tpf.sdk.unity.TPFUnitySdk"))
            {
                jo = jc.CallStatic<AndroidJavaObject>("getInstance");
            }
        }

        /// <summary>
        /// 调用安卓层函数，有返回参数///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private T CallSDKFunc<T>(string method, params object[] param)
        {
            try
            {
                return jo.Call<T>(method, param);
            }
            catch (Exception e)
            {
                TPFLog.Error(e);
            }
            return default(T);
        }

        /// <summary>
        /// 调用安卓层函数，没有返回参数///
        /// </summary>
        /// <param name="method"></param>
        /// <param name="param"></param>
        private void CallSDKFunc(string method, params object[] param)
        {
            try
            {
                jo.Call(method, param);
            }
            catch (Exception e)
            {
                TPFLog.Error(e);
            }
        }

        public override string channelId
        {
            get { return CallSDKFunc<string>("getChannelId"); }
            set { }
        }

        public override string GetAppID()
        {
            return CallSDKFunc<string>("getAppID");
        }

        public override string GetAppKey()
        {
            return CallSDKFunc<string>("getAppKey");
        }

        public override string GetAppSecret()
        {
            return CallSDKFunc<string>("getAppSecret");
        }

        public override string GetChannelID()
        {
            return CallSDKFunc<string>("getChannelId");
        }

        internal override string GetSubmitInfo()
        {
            return CallSDKFunc<string>("getSubmitInfo");
        }

        internal override string GetToken()
        {
            return CallSDKFunc<string>("getToken");
        }

        internal override string GetUserId()
        {
            return CallSDKFunc<string>("getUserId");
        }

        public override string ReadDeviceIdentify()
        {
            return CallSDKFunc<string>("readDeviceIdentify");
        }

        public override bool IsOfficial()
        {
            return CallSDKFunc<bool>("isOfficial");
        }

        public override void OpenWeb(TPFSdkInfo info)
        {
            CallSDKFunc("openWeb", info.ToJson());
        }

        public override string GetSubChannelID()
        {
            return CallSDKFunc<string>("getSubChannelID");
        }


        internal override void Init()
        {
            Debug.Log("TPFSdkAndroid Init");
            base.Init();

            // 覆盖打包的参数
            appId = GetAppID();
            appKey = GetAppKey();
            appSecret = GetAppSecret();
            channelId = GetChannelID();

            SwitchEnv(getEnvironConfig());
        }

        public override void SwitchEnv(string jsonCfg)
        {
            CallSDKFunc("switchEnv", jsonCfg);
            base.SwitchEnv(jsonCfg);
        }

#region "user"

        public override bool GetUserInfo()
        {
            return CallSDKFunc<bool>("getUserInfo");
        }

        internal override bool Register(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("register", info.ToJson());
        }

        internal override bool Logout()
        {
            return CallSDKFunc<bool>("logout");
        }

        internal override bool Exit()
        {
            return CallSDKFunc<bool>("exit");
        }

        internal override bool Login(TPFSdkInfo info)
        {
            string param = string.Empty;
            if (info != null && !info.IsEmpty())
            {
                param = info.ToJson();
            }
            return CallSDKFunc<bool>("login", param.Trim());
        }

        internal override void VerifyCode(TPFSdkInfo info)
        {
            CallSDKFunc("verifyCode", info.ToJson());
        }

        internal override void RealNameVerify(TPFSdkInfo info)
        {
            CallSDKFunc("realNameVerify", info.ToJson());
        }

        internal override void QueryRealName(TPFSdkInfo info)
        {
            CallSDKFunc("queryRealName", info.ToJson());
        }

        internal override void ForgetPwd(TPFSdkInfo info)
        {
            CallSDKFunc("forgetPwd", info.ToJson());
        }

        internal override void ChangePwd(TPFSdkInfo info)
        {
            CallSDKFunc("changePwd", info.ToJson());
        }

        internal override void QueryUserBind(TPFSdkInfo info)
        {
            CallSDKFunc("queryUserBind", info.ToJson());
        }

        internal override void BindAccount(TPFSdkInfo info)
        {
            CallSDKFunc("bindAccount", info.ToJson());
        }

        internal override bool QueryPhoneNum(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("queryPhoneNum", info.ToJson());
        }

        internal override bool FetchPhoneCode(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("fetchPhoneCode", info.ToJson());
        }

        internal override bool CheckPhoneCode(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("checkPhoneCode", info.ToJson());
        }

        internal override bool BindPhone(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("bindPhone", info.ToJson());
        }

        internal override bool SubmitInfo(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("submitInfo", info.ToJson());
        }

        internal override void AgreeProtocol(TPFSdkInfo info)
        {
            CallSDKFunc("agreeProtocol", info.ToJson());
        }

        public override bool CoinTradeEvent(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("coinTradeEvent", info.ToJson());
        }

        public override bool PropTradeEvent(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("propTradeEvent", info.ToJson());
        }

        public override bool GameTaskEvent(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("gameTaskEvent", info.ToJson());
        }

        public override bool EventReport(string param)
        {
            return CallSDKFunc<bool>("eventReport", param);
        }

        public override bool AdEvent(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("adEvent", info.ToJson());
        }

        internal override bool PrePay(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("prePay", info.ToJson());
        }

        internal override bool QueryOrder(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("queryOrder", info.ToJson());
        }

        internal override bool QueryOrderList(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("queryOrderList", info.ToJson());
        }

        internal override bool ConfirmOrder(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("confirmOrder", info.ToJson());
        }

        internal override void LoadAd(int type, TPFSdkInfo extra)
        {
            CallSDKFunc("loadAd", extra.ToJson());
        }

        internal override void ShowAd(int type, TPFSdkInfo extra)
        {
            CallSDKFunc("showAd", extra.ToJson());
        }
        #endregion

        internal override bool JoinQQGroup()
        {
            return CallSDKFunc<bool>("joinQQGroup");
        }

        public override void GetQQConfig()
        {
            CallSDKFunc("getQQConfig");
        }

        internal override bool Pay(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("pay", info.ToJson());
        }

        internal override bool InstallAPK(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("installApk", info.ToJson());
        }

        internal override bool Download(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("download", info.ToJson());
        }

        internal override bool PauseDownload(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("pauseDownload", info.ToJson());
        }

        internal override bool ScanQRCode(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("scanQRCode", info.ToJson());
        }

        internal override bool QueryGiftCode(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("queryGiftCode", info.ToJson());
        }

        internal override bool PostGiftCode(TPFSdkInfo info)
        {
            return CallSDKFunc<bool>("postGiftCode", info.ToJson());
        }

        public override bool EventReport(string eventId, string eventType, string extra)
        {
            return CallSDKFunc<bool>("eventReport", eventId, eventType, extra);
        }
        
        public override string GetNotchInfo(){
            return CallSDKFunc<string>("getNotchInfo");
        }   

        public override string getEnvironConfig()
        {
            Debug.Log("TPFSdkAndroid getEnvironConfig");
            var envConfg = CallSDKFunc<string>("getEnvironConfig");
            Debug.Log(string.Format("TPFSdkAndroid getEnvironConfig: {0}", (envConfg == null ? "{}" : envConfg)));
            return envConfg;
        }
        
        internal override void SendGameRequest(TPFSdkInfo info)
        {
            CallSDKFunc("sendGameRequest", info.ToJson());
        }

        internal override void DoSinglePay(TPFSdkInfo info)
        {
            CallSDKFunc("doSinglePay", info.ToJson());
        }

        internal override void DoPay(string parameters, string payMethod)
        {
            CallSDKFunc("doPay", parameters, payMethod);
        }

        internal override void QueryOrderAndConfirm(TPFSdkInfo info)
        {
            CallSDKFunc("queryOrderAndConfirm", info.ToJson());
        }
    }
}

#endif