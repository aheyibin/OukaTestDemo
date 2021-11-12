/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的接口
** use: demo
** modify:             
*******************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using TPFSDK.Normal.UnityPlugins;
using TPFSDK.Normal.UnityPlugins.Passive;
using UnityEngine;
//using XLua;

namespace TPFSDK
{
    /// <summary>
    /// 操作android ios pc的sdk的接口基类////
    /// </summary>
    public abstract class ITPFSdk
    {
        //tpfsdk加载出的配置
        private static TPFSdkEnvironment _standard_env;

        private static ITPFSdk _instance;
        public static ITPFSdk Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_WIN
                    _instance = new TPFSdkNormal();
#elif UNITY_ANDROID
                _instance = new TPFSdkAndroid();
#elif UNITY_IOS
                _instance = new TPFSdkIOS();
#endif

#pragma warning disable CS0618 // 类型或成员已过时
                    TPFSdkCallback.Init();
#pragma warning restore CS0618 // 类型或成员已过时
                    _instance.Init();
                }

                return _instance;
            }
        }

        /// <summary>
        /// 获得tpf应用ID///
        /// </summary>
        /// <returns></returns>

        string _appId = "";
        public virtual string appId { get { return _appId; } set { _appId = value != null ? value : ""; } }

        /// <summary>
        /// 获得tpf应用key///
        /// </summary>
        /// <returns></returns>

        string _appKey = "";
        public virtual string appKey { get { return _appKey; } set { _appKey = value != null ? value : ""; } }

        /// <summary>
        /// 获得tpf应用secret///
        /// </summary>
        /// <returns></returns>
        string _appSecret = "";
        public virtual string appSecret { get { return _appSecret; } set { _appSecret = value != null ? value : ""; } }

        /// <summary>
        /// 获得tpf应用areaId///
        /// </summary>
        /// <returns></returns>
        string _areaId = "";
        public virtual string areaId { get { return _areaId; } set { _areaId = value != null ? value : ""; } }

        internal class NetworkConfig 
        {
            public string tpfProxyUrl;
            public string officialLoginHost;
            public string tpfLoginHost;
        }

        internal NetworkConfig _networkConfig = new NetworkConfig();
        internal virtual NetworkConfig networkConfig { get { return _networkConfig; } set { _networkConfig = value; } }

        internal class NormalUrlLoginConfig
        {
            public string loginAppKey;
            public string loginAreaKey;
        }
        NormalUrlLoginConfig _loginConfig = null;
        internal virtual NormalUrlLoginConfig loginConfig { get { return _loginConfig; } set { _loginConfig = value; } }

        private ITPFHttpRequestAdapter _requestAdapter;
        internal ITPFHttpRequestAdapter requestAdapter { get { return _requestAdapter; } }

        /// <summary>
        /// 获得渠道ID///
        /// </summary>
        /// <returns></returns>

        string _channelId = "";
        public virtual string channelId { get { return _channelId; } set { _channelId = value != null ? value : ""; } }

        /// <summary>
        /// 获得渠道ID///
        /// </summary>
        /// <returns></returns>
        public abstract string GetChannelID();

        /// <summary>
        /// 获得tpf应用ID///
        /// </summary>
        /// <returns></returns>
        public abstract string GetAppID();

        /// <summary>
        /// 获得tpf应用key///
        /// </summary>
        /// <returns></returns>
        public abstract string GetAppKey();

        /// <summary>
        /// 获得tpf应用secret///
        /// </summary>
        /// <returns></returns>
        public abstract string GetAppSecret();

        /// <summary>
        /// 广告渠道id
        /// </summary>
        /// <returns></returns>
        public abstract string GetSubChannelID();

        /// <summary>
        /// 获取渠道用户信息，此方法的返回值为Bool型，若返回false，则表示当前渠道不支持此方法
        /// </summary>
        /// <returns></returns>
        public abstract bool GetUserInfo();

        /// <summary>
        /// 读取唯一标识写入本地
        /// 本地不存在时返回""
        /// </summary>
        /// <returns></returns>
        public abstract string ReadDeviceIdentify();

        public abstract bool IsOfficial();

        #region 全面屏适配
        public abstract string GetNotchInfo();

        #endregion

        /// <summary>
        /// 获取当前登录的 UserId
        /// </summary>
        /// <returns></returns>
        internal abstract string GetUserId();
        
        /// <summary>
        /// 获取当前登录的 User Token
        /// </summary>
        /// <returns></returns>
        internal abstract string GetToken();

        /// <summary>
        /// init 流程：
        /// 1. 基类中的配置赋值业务层需要参数
        /// 2. 子类中的配置赋值覆盖成打包时更改的参数
        /// 3. 子类中检查 environment 配置，覆盖成环境配置参数，只针对 networkConfig 中可覆盖的参数，详情：RewriteNetworkCfgfromEnvCfg
        /// </summary>
        internal virtual void Init()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            Debug.Log("[TPFSDK] LoadConfig");
            string config_text = ITPFSdkLoader.GetLoader().LoadConfig();
            if (config_text != null)
            {
                Debug.Log(string.Format("[TPFSDK] LoadConfig {0}", config_text));
                try
                {
                    _standard_env = new TPFSdkEnvironment(config_text);
                    // 默认配置赋值
                    appId = _standard_env["appId"] as string;
                    areaId = _standard_env["areaId"] as string;
                    channelId = _standard_env["channelId"] as string;
                    
                    // 中台建立连接所需参数
                    if (_standard_env["appKey"] != null)
                    {
                        appKey = _standard_env["appKey"] as string;
                    }
                    if (_standard_env["appSecret"] != null)
                    {
                        appSecret = _standard_env["appSecret"] as string;
                    }

                    // 旧 url 登录所需参数
                    if (_standard_env["isUseNormalUrlLogin"] != null &&
                        (bool)_standard_env["isUseNormalUrlLogin"])
                    {
                        _requestAdapter = new NormalUrlAdapter(this);
                        loginConfig = new NormalUrlLoginConfig();
                        loginConfig.loginAreaKey = _standard_env["loginAreaKey"] as string;
                        loginConfig.loginAppKey = _standard_env["loginAppKey"] as string;
                    }
                    else
                    {
                        _requestAdapter = new ProxyAdapter(this);
                    }
                }
                catch(Exception e)
                {
                    e = new Exception("[TPFSDK] expection occur while tpf load config", e);
                    Debug.LogException(e);
                }
                RewriteNetworkCfgFromStandardCfg();
            }
            else
            {
                Debug.LogError(string.Format("[TPFSDK] LoadConfig tpf_sdk_config error ! "));
            }
        }

        private Action _on_env_change = delegate { };
        public void OnEnvChange(Action action)
        {
            if(action != null)
                _on_env_change += action;
        }

        /// <summary>
        /// 切换 tpf 网络环境
        /// </summary>
        /// <param name="jsonCfg">覆盖的环境配置，如传空串则自动切换回默认的正式环境</param>
        public virtual void SwitchEnv(string jsonCfg)
        {
            if (string.IsNullOrEmpty(jsonCfg))
            {
                RewriteNetworkCfgFromStandardCfg();
            }
            else
            {
                _environment = new TPFSdkEnvironment(jsonCfg);
                RewriteNetworkCfgFromEnvCfg(_environment);
            }
            if(_on_env_change != null)
            {
                _on_env_change.Invoke();
            }
        }

        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="info"></param>
        public abstract void OpenWeb(TPFSdkInfo info);

        #region 事件上报
        /// <summary>
        /// 自定义事件上报
        /// </summary>
        /// <param name="eventId">事件id，游戏自定义</param>
        /// <param name="eventType">事件类型，游戏自定义</param>
        /// <param name="extra">额外数据，游戏自定义</param>
        /// <returns></returns>
        public abstract bool EventReport(string eventId, string eventType, string extra);

        public abstract bool EventReport(string param);

        /// <summary>
        /// 货币交易流水上报
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract bool CoinTradeEvent(TPFSdkInfo info);

        /// <summary>
        /// 道具交易流水上报
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract bool PropTradeEvent(TPFSdkInfo info);

        /// <summary>
        /// 游戏任务上报
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract bool GameTaskEvent(TPFSdkInfo info);

        /// <summary>
        /// 广告埋点
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract bool AdEvent(TPFSdkInfo info);


        #endregion

        //////////////////////////////////
        #region "user"

        /// <summary>
        /// 触发登陆///
        /// </summary>
        internal abstract bool Login(TPFSdkInfo info);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="info">用户注册数据 </param>
        /// <returns></returns>
        internal abstract bool Register(TPFSdkInfo info);

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="info">用户获取验证码数据 </param>
        /// <returns></returns>
        internal abstract void VerifyCode(TPFSdkInfo info);

        /// <summary>
        /// 退出///
        /// </summary>
        internal abstract bool Logout();

        /// <summary>
        /// 上传游戏角色信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool SubmitInfo(TPFSdkInfo info);

        /// <summary>
        /// 获取游戏上传的角色信息
        /// </summary>
        /// <returns>以 json 形式返回信息</returns>
        internal abstract string GetSubmitInfo();

        /// <summary>
        /// 触发退出///
        /// </summary>
        internal abstract bool Exit();

        /// <summary>
        /// 账号实名认证
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract void RealNameVerify(TPFSdkInfo info);

        /// <summary>
        /// 查询账号实名认证
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract void QueryRealName(TPFSdkInfo info);

        /// <summary>
        /// 官方渠道，在忘记密码时调用该接口
        /// </summary>
        /// <param name="info"></param>
        internal abstract void ForgetPwd(TPFSdkInfo info);

        /// <summary>
        /// 官方渠道，修改密码时调用该接口
        /// </summary>
        /// <param name="info"></param>
        internal abstract void ChangePwd(TPFSdkInfo info);

        /// <summary>
        /// 查询游客是否绑定账号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract void QueryUserBind(TPFSdkInfo info);

        /// <summary>
        /// 官方渠道查询手机号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool QueryPhoneNum(TPFSdkInfo info);

        /// <summary>
        /// 官方渠道获取手机验证码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool FetchPhoneCode(TPFSdkInfo info);

        /// <summary>
        /// 官方渠道校验手机验证码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool CheckPhoneCode(TPFSdkInfo info);

        /// <summary>
        /// 官方渠道绑定手机号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool BindPhone(TPFSdkInfo info);

        /// <summary>
        /// 游客绑定手机号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract void BindAccount(TPFSdkInfo info);

        internal abstract void AgreeProtocol(TPFSdkInfo info);

        #endregion

        #region 支付

        /// <summary>
        /// 支付预下单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool PrePay(TPFSdkInfo info);


        /// <summary>
        /// 触发支付
        /// </summary>
        /// <param name="info"></param>
        internal abstract bool Pay(TPFSdkInfo info);


        /// <summary>
        /// 订单确认发货
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool ConfirmOrder(TPFSdkInfo info);

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool QueryOrder(TPFSdkInfo info);

        /// <summary>
        /// 批量查询订单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool QueryOrderList(TPFSdkInfo info);

        /// <summary>
        /// 单机游戏支付
        /// </summary>
        /// <param name="info"></param>
        internal abstract void DoSinglePay(TPFSdkInfo info);

        /// <summary>
        /// 网络游戏支付
        /// </summary>
        /// <param name="parameters">服务器下单后，回传的参数</param>
        internal abstract void DoPay(string parameters, string payMethod);

        /// <summary>
        /// 查询订单并发货
        /// </summary>
        /// <param name="info"></param>
        internal abstract void QueryOrderAndConfirm(TPFSdkInfo info);
        #endregion 

        #region ad
        /// <summary>
        /// 加载广告
        /// </summary>
        /// <param name="type"></param>
        /// <param name="extra"></param>
        internal abstract void LoadAd(int type, TPFSdkInfo extra);

        /// <summary>
        /// 展示广告
        /// </summary>
        /// <param name="type"></param>
        /// <param name="extra"></param>
        internal abstract void ShowAd(int type, TPFSdkInfo extra);
        #endregion

        internal abstract bool InstallAPK(TPFSdkInfo info);

        internal abstract bool Download(TPFSdkInfo info);

        internal abstract bool PauseDownload(TPFSdkInfo info);

        /// <summary>
        /// 执行扫描二维码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool ScanQRCode(TPFSdkInfo info);

        /// <summary>
        /// 查询礼包卡
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool QueryGiftCode(TPFSdkInfo info);

        /// <summary>
        /// 使用礼包卡
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal abstract bool PostGiftCode(TPFSdkInfo info);

        /// <summary>
        /// sdk 提供的中台业务通用接口
        /// </summary>
        /// <param name="info"></param>
        internal abstract void SendGameRequest(TPFSdkInfo info);

        /// <summary>
        /// QQ一键加群
        /// </summary>
        /// <returns></returns>
        internal abstract bool JoinQQGroup();

        // TODO: 提取到 Business 层
        public abstract void GetQQConfig();

        #region 扫码登录

        /// <summary>
        /// 请求登录二维码
        /// </summary>
        /// <param name="callback">回调参数为: TPFCode(成功: TPFCODE_SUCCESS, 失败: TPFCODE_GET_QR_CODE_ERROR), 二维码 ID, 二维码图片字节流字符串</param>
        /// <returns></returns>
        public bool GetLoginQRCodeImage(Action<int, string, string> callback)
        {
            TPFPluginScanLoginPassive.Instance.GetQRCodeImage(callback);
            return true;
        }

        /**
         * Passive端(PC端)开始监听登录状态变化的事件
         */
        public ScanLoginListener ListenScanLoginStatus(string qrId, int pollingInterval)
        {
            return TPFPluginScanLoginPassive.Instance.StartListen(qrId, pollingInterval);
        }

        /**
         * Active端（手机端）扫码预登录
         */
        public bool ScanLoginPreLogin(string qrCodeUrl)
        {
            return TPFPluginScanLoginActive.Instance.PreLogin(qrCodeUrl);
        }

        /**
         * Active端（手机端）扫码取消登录。取消登录的行为应该发送在预登录后
        */
        public bool ScanLoginCancelLogin(string qrCodeUrl)
        {
            return TPFPluginScanLoginActive.Instance.CancelLogin(qrCodeUrl);
        }

        /**
         * Active端（手机端）登录验证
        */
        public bool ScanLoginAuthLogin(string qrCodeUrl, string tpfId, string token, string clientExtra)
        {
            return TPFPluginScanLoginActive.Instance.AuthLogin(qrCodeUrl, tpfId, token, clientExtra);
        }

        #endregion

        public virtual string getEnvironConfig() { return "{}"; }

        protected TPFSdkEnvironment _environment;
        public TPFSdkEnvironment enviroment { get { return _environment; } }

        private void RewriteNetworkCfgFromStandardCfg()
        {
            RewriteNetworkCfgFromEnvCfg(_standard_env);
        }

        private void RewriteNetworkCfgFromEnvCfg(TPFSdkEnvironment env)
        {
            if(env["tpfProxyUrl"] != null)
            {
                networkConfig.tpfProxyUrl = env.tpfProxyUrl;
            }
            if(env["loginOfficialHost"] != null)
            {
                networkConfig.officialLoginHost = env["loginOfficialHost"] as string;
            }
            if(env["loginHost"] != null)
            {
                networkConfig.tpfLoginHost = env["loginHost"] as string;
            }
        }
    }
}


