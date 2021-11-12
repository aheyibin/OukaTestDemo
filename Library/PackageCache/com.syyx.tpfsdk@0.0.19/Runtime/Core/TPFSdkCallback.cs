/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的回调控制
** use: demo
** modify:             
*******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TPFSDK
{
    /// <summary>
    /// 各平台sdk的回调，保证接口统一性///
    /// </summary>
    internal class TPFSdkCallback : MonoBehaviour
    {
        private object locker = new object();

        private class CallbackEntity
        {
            public TPFSdkEventType Type;
            public TPFSdkInfo Info;
        }


        private static TPFSdkCallback _instance;

        private static object _lock = new object();

        private readonly List<CallbackEntity> _callbackEntityBuffer = new List<CallbackEntity>();

        //初始化回调对象
        internal static TPFSdkCallback Init()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    GameObject callback = GameObject.Find("TpfSdk_callback");
                    if (callback == null)
                    {
                        callback = new GameObject("TpfSdk_callback");
                        UnityEngine.Object.DontDestroyOnLoad(callback);
                        _instance = callback.AddComponent<TPFSdkCallback>();
                    }
                    else
                    {
                        _instance = callback.GetComponent<TPFSdkCallback>();
                    }
                }

                return _instance;
            }
        }

        public static TPFSdkCallback Instance
        {
            get { return Init(); }
        }

        private void Update()
        {
            if (_callbackEntityBuffer.Count > 0)
            {
                lock (locker)
                {
                    for (var i = 0; i < _callbackEntityBuffer.Count; i++)
                    {
                        try
                        {
                            EventCenter.Dispatch(_callbackEntityBuffer[i].Type, _callbackEntityBuffer[i].Info);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("[TPFSDK] event callback error,please check type" + _callbackEntityBuffer[i].Type.ToString());
                            Debug.LogException(ex);
                        }
                    }

                    _callbackEntityBuffer.Clear();
                }
            }
        }

        /// <summary>
        /// sdk初始化的结果的回馈///
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnInitResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity {Info = info, Type = TPFSdkEventType.EVENT_TYPE_INIT });
        }

        /// <summary>
        /// 获取验证码结果的反馈
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnVerifyCodeResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_VERIFYCODE });
        }

        /// <summary>
        /// 注册的反馈
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnRegisterResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_REGISTER });
        }

        /// <summary>
        /// 登陆的回馈///
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnLoginResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_LOGIN });
        }

        /// <summary>
        /// 退出的回馈///
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnLogoutResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_LOGOUT });
        }

        /// <summary>
        /// 密码发生变化回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnPasswordResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_PASSWORD });
        }

        public void OnBindAccountResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_BIND_ACCOUNT});
        }

        /// <summary>
        /// 实名认证回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnRealNameResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_REALNAME });
        }

        /// <summary>
        /// 查询手机号回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnQueryPhoneResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_QUERY_PHONE });
        }

        /// <summary>
        /// 手机验证码校验回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnVerifyCodeCheckResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_VERIFY_CODE_CHECK });
        }

        /// <summary>
        /// 绑定手机回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnBindPhoneResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_BIND_PHONE });
        }

        /// <summary>
        /// 支付的回馈 一般来说只有单机才需要///
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnPayResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_PAY });
        }

        /// <summary>
        /// 退出的回馈///
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnExitResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_EXIT });
        }

        /// <summary>
        /// 分享的回馈///
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnShareResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_SHARE });
        }
        
        /// <summary>
        /// 广告回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnAdResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_AD });
        }

        /// <summary>
        /// 用户协议回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnProtocolResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_PROTOCOL });
        }

        /// <summary>
		/// 通用的回馈///
		/// </summary>
		/// <param name="jsonData"></param>
		public void OnCommonResult(string jsonData)
		{
			TPFSdkInfo info = new TPFSdkInfo(jsonData);
			AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_COMMON_RESULT });
		}

        /**
         * 扫码结果返回
         */
        public void OnScanQRCodeResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity {Info = info, Type = TPFSdkEventType.EVENT_TYPE_SCAN_QR_CODE_RESULT});
        }

        /**
         * 登录状态发生变化
         */
        public void OnScanLoginStatusChange(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity {Info = info, Type = TPFSdkEventType.EVENT_TYPE_SCAN_LOGIN_STATUS_CHANGE });
        }

        /**
         * 查询/使用礼包卡结果返回
         */
        public void OnGiftCodeResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_GIFTCODE });
        }

        /// <summary>
        /// tpf 中台业务回调
        /// </summary>
        /// <param name="jsonData"></param>
        public void OnGameResult(string jsonData)
        {
            TPFSdkInfo info = new TPFSdkInfo(jsonData);
            AddToCallbackBuffer(new CallbackEntity { Info = info, Type = TPFSdkEventType.EVENT_TYPE_TPF_GAME_BUSSINESS });
        }

        private void AddToCallbackBuffer(CallbackEntity callbackEntity)
        {
            lock (locker)
            {
                _callbackEntityBuffer.Add(callbackEntity);
            }
        }
    }
}
