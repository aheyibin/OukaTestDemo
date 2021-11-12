using System;
using System.Collections.Generic;
using System.IO;
using TPFSDK.Utils;
using UnityEngine;

namespace TPFSDK
{
    /** 
     * 为了屏蔽 SDK 底层必须以 GameObject.SendMessage(go, methodName) 的方式回调给业务，
     * 所有 SDK core 中的异步接口都在这里进行封装提供给业务层。
     */
    public static class CoreExtensions
    {
        // 针对 sdk 核心流程的内部回调，用于通知内部扩展依赖于核心流程信息的模块
        #region internal callbacks
        internal enum CoreEvent
        {
            LOGIN = 0,

            SUBMIT_INFO,

            __MAX__
        }

        private static Action[] s_CoreEventCallbacks;

        static CoreExtensions()
        {
            s_CoreEventCallbacks = new Action[(int)CoreEvent.__MAX__];
            for (int i = 0; i < s_CoreEventCallbacks.Length; i++)
            {
                s_CoreEventCallbacks[i] = delegate { };
            }
        }

        /// <summary>
        /// 订阅核心基础层特定事件
        /// </summary>
        /// <param name="eventKey"></param>
        /// <param name="cb"></param>
        internal static void Subscribe(CoreEvent eventId, Action cb)
        {
            s_CoreEventCallbacks[(int)eventId] += cb;
        }

        private static void Dispatch(CoreEvent eventId)
        {
            s_CoreEventCallbacks[(int)eventId]?.Invoke();
        }
        #endregion


        #region bind event
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sdk"></param>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public static void Login(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_LOGIN, (sdk_event) => 
            {
                Dispatch(CoreEvent.LOGIN);
                callback?.Invoke(sdk_event);
            });

            try
            {
                // 检查是否是游客登录
                const string TOURIST_ACCOUNT_TYPE = "0";
                if (info.GetData(TPFParamKey.TYPE) == TOURIST_ACCOUNT_TYPE)
                {
                    string touristAccountPath = $"{Application.persistentDataPath}/tpfsdk_tourist.cfg";
                    // 先尝试读本地游客账号
                    if (File.Exists(touristAccountPath))
                    { // 成功读取游客账号进行登录
                        string touristAccount = File.ReadAllText(touristAccountPath);
                        info.SetData(TPFParamKey.ACCOUNT, touristAccount);
                        sdk.Login(info);
                    }
                    else
                    { // 本地不存在则首先进行游客注册
                        const int TOURIST_REGISTER_TYPE = 3;
                        TPFSdkInfo registerInfo = new TPFSdkInfo();
                        registerInfo.SetData(TPFParamKey.TYPE, TOURIST_REGISTER_TYPE);
                        sdk.Register(registerInfo, (tpfEvent) =>
                        {
                            Dictionary<string, object> extra = (Dictionary<string, object>)(tpfEvent.EventInfo.GetDataObject(TPFParamKey.EXTRA));
                            string account = Convert.ToString(extra["Account"]);
                            if (tpfEvent.ErrorCode == TPFCode.TPFCODE_REGISTER_SUCCESS)
                            {
                                // 注册成功将游客账号写入本地，并进行登录
                                File.WriteAllText(touristAccountPath, account);
                                info.SetData(TPFParamKey.ACCOUNT, account);
                                sdk.Login(info);
                                return;
                            }
                            else if (tpfEvent.ErrorCode == TPFCode.TPFCODE_REGISTER_FAIL_ACCOUNT_ALREADY_EXIST)
                            {
                                Debug.LogError($"[TPFSDK] Tourist register fail! Account {account} already exist!");
                            }
                            else
                            {
                                Debug.LogError($"[TPFSDK] Tourist register fail! {tpfEvent.EventInfo.GetData(TPFParamKey.SDK_ERRCODE)}, {tpfEvent.EventInfo.GetData(TPFParamKey.SDK_ERRMSG)}");
                            }
                        // 注册失败回调上层登录失败
                        var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_FAIL, tpfEvent.ErrorCode, "Tourist register fail!", null);
                            EventCenter.Dispatch(TPFSdkEventType.EVENT_TYPE_LOGIN, new TPFSdkInfo(ref out_param));
                        });
                    }
                }
                else // 非游客正常登录
                { 
                    sdk.Login(info);
                }
            }
            catch(Exception e)
            {
                Debug.LogError("[TPFSDK] login exception!");
                Debug.LogException(e);
            }
        }

        public static bool Logout(this ITPFSdk sdk, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_LOGOUT, callback);
            return sdk.Logout();
        }

        public static bool Exit(this ITPFSdk sdk, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_EXIT, callback);
            return sdk.Exit();
        }

        public static void LoadAd(this ITPFSdk sdk, int type, TPFSdkInfo extra, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_AD, callback);
            sdk.LoadAd(type, extra);
        }
        
        public static void ShowAd(this ITPFSdk sdk, int type, TPFSdkInfo extra, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_AD, callback);
            sdk.ShowAd(type, extra);
        }

        public static void Register(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_REGISTER, callback);
            sdk.Register(info);
        }

        public static void VerifyCode(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_VERIFYCODE, callback);
            sdk.VerifyCode(info);
        }

        public static void BindAccount(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_BIND_ACCOUNT, callback);
            sdk.BindAccount(info);
        }

        public static void ForgetPwd(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_PASSWORD, callback);
            sdk.ForgetPwd(info);
        }

        public static void ChangePwd(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_PASSWORD, callback);
            sdk.ChangePwd(info);
        }

        public static void RealNameVerify(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_REALNAME, callback);
            sdk.RealNameVerify(info);
        }

        public static void QueryRealName(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_REALNAME, callback);
            sdk.QueryRealName(info);
        }

        public static void QueryUserBind(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_BIND_ACCOUNT, callback);
            sdk.QueryUserBind(info);
        }

        public static void QueryGiftCode(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_GIFTCODE, callback);
            sdk.QueryGiftCode(info);
        }

        public static void PostGiftCode(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_GIFTCODE, callback);
            sdk.PostGiftCode(info);
        }

        public static void QueryPhoneNum(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_QUERY_PHONE, callback);
            sdk.QueryPhoneNum(info);
        }

        public static void FetchPhoneCode(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_VERIFYCODE, callback);
            sdk.FetchPhoneCode(info);
        }

        public static void CheckPhoneCode(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_VERIFY_CODE_CHECK, callback);
            sdk.CheckPhoneCode(info);
        }

        public static void BindPhone(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_BIND_PHONE, callback);
            sdk.BindPhone(info);
        }

        public static void AgreeProtocol(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_PROTOCOL, callback);
            sdk.AgreeProtocol(info);
        }

        public static void ScanQRCode(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_SCAN_QR_CODE_RESULT, callback);
            sdk.ScanQRCode(info);
        }

        [Obsolete("please use DoSinglePay or DoPay instead")]
        public static void Pay(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_PAY, callback);
            sdk.Pay(info);
        }

        public static void DoPay(this ITPFSdk sdk, string parameters, string payMethod, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_PAY, callback);
            sdk.DoPay(parameters, payMethod);
        }

        public static void DoSinglePay(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterBindEvent(TPFSdkEventType.EVENT_TYPE_PAY, callback);
            sdk.DoSinglePay(info);
        }

        public static bool SubmitInfo(this ITPFSdk sdk, TPFSdkInfo info)
        {
            Dispatch(CoreEvent.SUBMIT_INFO);
            return sdk.SubmitInfo(info);
        }
        #endregion


        #region common event

        /// <summary>
        /// 单机游戏登录
        /// </summary>
        /// <param name="sdk"></param>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        /// <param name="queryOrderCallback">单机游戏登录完成后，sdk 会自动查单，游戏需要提前传入对掉单的情况做处理（确认并发货）</param>
        public static void Login(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback, TPFSdkEventDelegate queryOrderCallback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_QueryOrderList, queryOrderCallback);
            sdk.Login(info, callback);
        }

        [Obsolete("please use DoSinglePay or DoPay instead")]
        public static void PrePay(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_PrePay, callback);
            sdk.PrePay(info);
        }

        [Obsolete("please use QueryOrderAndConfirm instead")]
        public static void QueryOrder(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_QueryOrder, callback);
            sdk.QueryOrder(info);
        }

        [Obsolete("please use QueryOrderAndConfirm instead")]
        public static void ConfirmOrder(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_ConfirmOrder, callback);
            sdk.ConfirmOrder(info);
        }

        public static void QueryOrderList(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_QueryOrderList, callback);
            sdk.QueryOrderList(info);
        }

        public static void QueryOrderAndConfirm(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_QueryOrder, callback);
            sdk.QueryOrderAndConfirm(info);
        }

        public static void JoinQQGroup(this ITPFSdk sdk, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_QQ, callback);
            sdk.JoinQQGroup();
        }

        public static void GetQQConfig(this ITPFSdk sdk, TPFSdkEventDelegate callback)
        {
            EventCenter.RegisterCommonEvent(TPFSdkCommonEventKey.CommonEventKey_QQ, callback);
            sdk.GetQQConfig();
        }
        #endregion
    }

}
