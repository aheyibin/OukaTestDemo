/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的IOS接口调用
** use: demo
** modify:             
*******************************************************************/
//#define UNITY_IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TPFSDK.Normal.UnityPlugins.Passive;
using UnityEngine;

//#if UNITY_IOS

namespace TPFSDK
{

    public static class TpfUnityIosBridge
    {
#if UNITY_IOS
        //[DllImport("__Internal")]
        //private static extern bool _ex_scanQrCode(string message);

         [DllImport("__Internal")]
        private static extern bool _ex_initRes(string token);

        [DllImport("__Internal")]
        private static extern bool _ex_download(string plugin, string token);

        [DllImport("__Internal")]
        private static extern bool _ex_install(string plugink, string token);

        [DllImport("__Internal")]
        private static extern bool _ex_deleteGpk(string plugin, string token);

        [DllImport("__Internal")]
        private static extern bool _ex_isInstall(string plugin);

        [DllImport("__Internal")]
        private static extern string _ex_getPluginList();

        [DllImport("__Internal")]
        private static extern string _ex_getPluginInfo(string plugin);

        [DllImport("__Internal")]
        private static extern bool _ex_checkUpdate(string token);

        [DllImport("__Internal")]
        private static extern bool _ex_setConfig(string configJson);

        [DllImport("__Internal")]
        private static extern bool _ex_runPluginCmd(string pluginName,string cmd,string param,string token);

        //[DllImport("__Internal")]
        //private static extern string _ex_getDeviceId();

        [DllImport ("__Internal")]
	    private static extern string tpf_unity_sdk_appId();
        [DllImport ("__Internal")]
	    private static extern string tpf_unity_sdk_loginAppKey();
        [DllImport("__Internal")]
        public static extern string tpf_unity_sdk_appKey();
        [DllImport("__Internal")]
        public static extern string tpf_unity_sdk_appSecret();
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_login(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_register(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_verifyCode(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_forgetPwd(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_changePwd(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_bindAccount(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_realNameVerify(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_queryRealName(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_queryUserBind(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_queryPhoneNum(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_fetchPhoneCode(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_checkPhoneCode(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_bindPhone(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_queryGiftCode(string data);
        [DllImport("__Internal")]
        private static extern void tpf_unity_sdk_postGiftCode(string data);
        [DllImport ("__Internal")]
        public static extern void tpf_unity_sdk_agreeProtocol(string data);
        [DllImport ("__Internal")]
        public static extern void tpf_unity_sdk_switchEnv(string data);
        [DllImport("__Internal")]
        public static extern void tpf_unity_sdk_getAnnouncements(string data);
        [DllImport("__Internal")]
        public static extern void tpf_unity_sdk_getWebUrls(string data);
        [DllImport("__Internal")]
        public static extern void tpf_unity_sdk_getMailsBriefInfo(long offset);
        [DllImport("__Internal")]
        public static extern void tpf_unity_sdk_getMailsDetailInfo(string mails);
        [DllImport("__Internal")]
        public static extern void tpf_unity_sdk_openWeb(string data);
        [DllImport("__Internal")]
        public static extern void tpf_unity_sdk_getCustomerServiceConfig();
        [DllImport("__Internal")]
        public static extern void tpf_unity_sdk_onPushMsgRead(int type);

#endif
        #region gpk relate
        public static bool ScanQrCode(string message)
        {
#if UNITY_IOS
            //return _ex_scanQrCode(message);
            return false;
#else
            return false;
#endif
        }

        public static bool InitRes(string token) {

#if UNITY_IOS
            return _ex_initRes(token);
#else
            return false;
#endif

        }

        public static bool Download(string plugin, string token)
        {
#if UNITY_IOS
            return _ex_download(plugin,token);
#else
            return false;
#endif
        }

        public static bool Install(string plugin, string token)
        {
#if UNITY_IOS
            return _ex_install(plugin,token);
#else
            return false;
#endif
        }

        public static bool DeleteGpk(string plugin,string token)
        {
#if UNITY_IOS
            return _ex_deleteGpk(plugin,token);
#else
            return false;
#endif
        }

        public static bool IsInstall(string plugin)
        {
#if UNITY_IOS
            return _ex_isInstall(plugin);
#else
            return false;
#endif
        }

        public static string GetPluginList()
        {
#if UNITY_IOS
            return _ex_getPluginList();
#else
            return "";
#endif
        }

        public static string GetPluginInfo(string plugin)
        {
#if UNITY_IOS
            return _ex_getPluginInfo(plugin);
#else
            return "";
#endif
        }

        public static bool CheckUpdate(string token)
        {
#if UNITY_IOS
            return _ex_checkUpdate(token);
#else
            return false;
#endif
        }

        public static bool SetConfig(string configJson)
        {
#if UNITY_IOS
            return _ex_setConfig(configJson);
#else
            return false;
#endif
        }

        public static bool RunPluginCmd(string pluginName,string cmd,string param,string token)
        {
#if UNITY_IOS
            return _ex_runPluginCmd(pluginName,cmd,param,token);
#else
            return false;
#endif
        }

        public static string GetUDID()
        {
#if UNITY_IOS
            return "";
            //return _ex_getDeviceId();
#else
            return "";
#endif
        }
        #endregion

        public static void OpenWeb(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_openWeb(data);
#else
#endif
        }

        public static void GetAnnouncements(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_getAnnouncements(data);
#else
#endif
        }
        public static void GetWebUrls(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_getWebUrls(data);
#else
#endif
        }
        public static void GetMailsBriefInfo(long offset)
        {
#if UNITY_IOS
            tpf_unity_sdk_getMailsBriefInfo(offset);
#else
#endif
        }
        public static void GetMailsDetailInfo(string mails)
        {
#if UNITY_IOS
            tpf_unity_sdk_getMailsDetailInfo(mails);
#else
#endif
        }
        public static string GetAppKey()
        {
#if UNITY_IOS
            return tpf_unity_sdk_appKey();
#else
            return "";
#endif        
        }

        public static string GetAppSecret()
        {
#if UNITY_IOS
            return tpf_unity_sdk_appSecret();
#else
            return "";
#endif        
        }

        public static string GetAppID()
        {
#if UNITY_IOS
            return tpf_unity_sdk_appId();
#else
            return "";
#endif        
        }

        public static bool Login(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_login(data);
            return true;
#else
            return false;
#endif
        }
        public static bool Register(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_register(data);
            return true;
#else
            return false;
#endif
        }
        public static void VerifyCode(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_verifyCode(data);
#else
#endif
        }
        public static void ForgetPwd(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_forgetPwd(data);
#else
#endif
        }
        public static void ChangePwd(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_changePwd(data);
#else
#endif
        }
        public static void BindAccount(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_bindAccount(data);
#else
#endif
        }
        public static void RealNameVerify(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_realNameVerify(data);
#else
#endif
        }
        public static void QueryRealName(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_queryRealName(data);
#else
#endif
        }
        public static void QueryUserBind(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_queryUserBind(data);
#else
#endif
        }
        public static bool QueryPhoneNum(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_queryPhoneNum(data);
            return true;
#else
            return false;
#endif
        }
        public static bool FetchPhoneCode(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_fetchPhoneCode(data);
            return true;
#else
            return false;
#endif
        }
        public static bool CheckPhoneCode(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_checkPhoneCode(data);
            return true;
#else
            return false;
#endif
        }
        public static bool BindPhone(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_bindPhone(data);
            return true;
#else
            return false;
#endif
        }
        public static bool QueryGiftCode(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_queryGiftCode(data);
            return true;
#else
            return false;
#endif
        }
        public static bool PostGiftCode(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_postGiftCode(data);
            return true;
#else
            return false;
#endif
        }

        public static void AgreeProtocol(string data)
        {
#if UNITY_IOS
            tpf_unity_sdk_agreeProtocol(data);
#else
#endif
        }

        public static void SwitchEnv(string cfg)
        {
#if UNITY_IOS
            tpf_unity_sdk_switchEnv(cfg);
#else
#endif
        }
        public static void GetCustomerServiceConfig()
        {
#if UNITY_IOS
            tpf_unity_sdk_getCustomerServiceConfig();
#else
#endif
        }
        public static void OnPushMsgRead(int type)
        {
#if UNITY_IOS
            tpf_unity_sdk_onPushMsgRead(type);
#else
#endif
        }
    }                 

	// ios官方渠道
    public class TPFSdkIOS : ITPFSdk
    {
        public TPFSdkIOS()
        {
        }

        public override void OpenWeb(TPFSdkInfo info)
        {
            TpfUnityIosBridge.OpenWeb(info.ToJson());
        }

        //QQ一键加群
        internal override bool JoinQQGroup()
        {
            return false;
        }
        public override void GetQQConfig()
        {
        }
        /// <summary>
        /// 获得渠道ID///
        /// </summary>
        /// <returns></returns>
        public override string GetChannelID()
        {
            // 官方渠道id固定为3
            return "3";
        }

        public override string channelId
        {
            get { return "3"; }
            set { }
        }


        /// <summary>
        /// 获得tpf应用ID///
        /// </summary>
        /// <returns></returns>
        public override string GetAppID()
        {
            // 这个是app store中获取的
            return TpfUnityIosBridge.GetAppID();
        }

        /// <summary>
        /// 获得tpf应用key///
        /// </summary>
        /// <returns></returns>
        public override string GetAppKey()
        {
            return TpfUnityIosBridge.GetAppKey();
        }

        /// <summary>
        /// 获得tpf应用secret///
        /// </summary>
        /// <returns></returns>
        public override string GetAppSecret()
        {
            return TpfUnityIosBridge.GetAppSecret();
        }

        public override string GetSubChannelID()
		{
			return "0";
		}

        internal override string GetUserId()
        {
            throw new NotImplementedException();
        }

        internal override string GetToken()
        {
            throw new NotImplementedException();
        }

        internal override string GetSubmitInfo()
        {
            throw new NotImplementedException();
        }

        public override string ReadDeviceIdentify()
        {
            throw new NotImplementedException();
        }

        public override bool IsOfficial()
        {
            return true;
        }

        internal override void Init()
        {
            base.Init();
            appId = GetAppID();
            appKey = GetAppKey();
            appSecret = GetAppSecret();
            channelId = GetChannelID();
            SwitchEnv(getEnvironConfig());
        }

        public override void SwitchEnv(string jsonCfg)
        {
            TpfUnityIosBridge.SwitchEnv(jsonCfg);
            base.SwitchEnv(jsonCfg);
        }

        #region "user"

        public override bool GetUserInfo()
        {
            // ios do not support GetUserInfo
            return false;
        }

        internal override bool Login(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.Login(info.ToJson());
        }

        internal override bool Logout()
        {
            return false;
        }

        internal override bool Exit()
        {
            return false;
        }

        internal override bool Register(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.Register(info.ToJson());
        }

        public override bool EventReport(string eventId, string eventType, string extra)
        {
            //throw new NotImplementedException();
            return true;
        }

        internal override void VerifyCode(TPFSdkInfo info)
        {
            TpfUnityIosBridge.VerifyCode(info.ToJson());
        }

        internal override void RealNameVerify(TPFSdkInfo info)
        {
            TpfUnityIosBridge.RealNameVerify(info.ToJson());
        }

        internal override void QueryRealName(TPFSdkInfo info)
        {
            TpfUnityIosBridge.QueryRealName(info.ToJson());
        }

        internal override void QueryUserBind(TPFSdkInfo info)
        {
            TpfUnityIosBridge.QueryUserBind(info.ToJson());
        }
        internal override void ForgetPwd(TPFSdkInfo info)
        {
            TpfUnityIosBridge.ForgetPwd(info.ToJson());
        }

        internal override void ChangePwd(TPFSdkInfo info)
        {
            TpfUnityIosBridge.ChangePwd(info.ToJson());
        }

        internal override void BindAccount(TPFSdkInfo info)
        {
            TpfUnityIosBridge.BindAccount(info.ToJson());
        }

        internal override bool QueryPhoneNum(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.QueryPhoneNum(info.ToJson());
        }

        internal override bool FetchPhoneCode(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.FetchPhoneCode(info.ToJson());
        }

        internal override bool CheckPhoneCode(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.CheckPhoneCode(info.ToJson());
        }

        internal override bool BindPhone(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.BindPhone(info.ToJson());
        }

        internal override bool SubmitInfo(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        public override bool CoinTradeEvent(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        public override bool PropTradeEvent(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        public override bool GameTaskEvent(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }
        
        public override bool EventReport(string param)
        {
            throw new NotImplementedException();
        }

        public override bool AdEvent(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        internal override bool PrePay(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        internal override bool QueryOrder(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        internal override bool QueryOrderList(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        internal override bool ConfirmOrder(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        internal override void LoadAd(int type, TPFSdkInfo extra)
        {
            throw new NotImplementedException();
        }

        internal override void ShowAd(int type, TPFSdkInfo extra)
        {
            throw new NotImplementedException();
        }

        internal override void AgreeProtocol(TPFSdkInfo info)
        {
            TpfUnityIosBridge.AgreeProtocol(info.ToJson());
        }

        internal override bool PostGiftCode(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.PostGiftCode(info.ToJson());
        }

        internal override bool QueryGiftCode(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.QueryGiftCode(info.ToJson());
        }
        #endregion


        // 购买商品
        internal override bool Pay(TPFSdkInfo info)
        {
#if UNITY_IOS
            var action = info.GetData(TPFParamKey.SDK_EXTRA);

            if (action == "PAY")
            {
                // ios下PRODUCT_NAME传递的是SKU，ios系统通过SKU识别支付商品
                var productName = info.GetData(TPFParamKey.PRODUCT_NAME);
                // 进行交易验证的时候需要用到ORDER_ID，所以要传递给底层记录下来
                var orderId = info.GetData(TPFParamKey.ORDER_ID);
                // 账号id，赋值给applicationUserName以帮助苹果识别非法用户
                var accountId = info.GetData(TPFParamKey.ROLE_ID);

                // 发起购买请求
                AppStore_startPurchase(productName, orderId, accountId);
            }
            else if (action == "FINISH")
            {
                // ios下PRODUCT_NAME传递的是SKU，ios系统通过SKU识别支付商品
                var productName = info.GetData(TPFParamKey.PRODUCT_NAME);
                var orderId = info.GetData(TPFParamKey.ORDER_ID);

                // 不新增接口了，直接复用原来的接口，通过参数控制行为
                // 移除购买，在购买流程结束之后要调用这个接口
                // 为了避免用户付过钱，但是没有通过验证（可能是网络断开或者客户端闪退），只有客户端明确接受到支付成功或者失败的消息才会移除交易
                AppStore_finishTransaction(productName, orderId);
            }
            else if (action == "CLEAR")
            {
                // 清理掉所有的交易，防止用户卡住
                AppStore_clearAllTransactions();
            }
            else if (action == "INIT")
            {
                // 在登录成功之后调用，添加监听
                AppStore_init();
            }
            else if (action == "LOCALE_WHITELIST")
            {
                // 添加白名单
                var whiteList = info.GetData(TPFParamKey.EXTRA);
                AppStore_setLocaleWhitelist(whiteList);
            }
            else if (action == "TWICE_VALID")
            {
                // 开启二次校验
                var param = info.GetData(TPFParamKey.EXTRA);
                AppStore_setTwiceValid(param != "false");
            }
#endif
            return true;

        }

        internal override bool InstallAPK(TPFSdkInfo info)
        {
            return false;
        }

        internal override bool Download(TPFSdkInfo info)
        {
            return false;
        }

        internal override bool PauseDownload(TPFSdkInfo info)
        {
            return false;
        }

        internal override bool ScanQRCode(TPFSdkInfo info)
        {
            return TpfUnityIosBridge.ScanQrCode(info.ToJson());
        }

        internal override void SendGameRequest(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        public override string GetNotchInfo() {
            return "IOS 暂不适配";
        }

        internal override void DoSinglePay(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

        internal override void DoPay(string parameters, string payMethod)
        {
            throw new NotImplementedException();
        }

        internal override void QueryOrderAndConfirm(TPFSdkInfo info)
        {
            throw new NotImplementedException();
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void AppStore_init();

        [DllImport("__Internal")]
        private static extern void AppStore_requestProducts(string[] skus, int skusNumber);

        [DllImport("__Internal")]
        private static extern void AppStore_startPurchase(string sku, string orderId, string accountId);

        [DllImport("__Internal")]
        private static extern void AppStore_restorePurchases();
        
        [DllImport("__Internal")]
        private static extern void AppStore_finishTransaction(string sku, string orderId);

        [DllImport("__Internal")]
        private static extern void AppStore_clearAllTransactions();

        [DllImport("__Internal")]
        private static extern void AppStore_setLocaleWhitelist(string localeWhitelist);
        
        [DllImport("__Internal")]
        private static extern void AppStore_setTwiceValid(bool enable);
        
#endif
    }
}

//#endif