using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TPFSDK
{
    /**
     * 统一的事件处理中心，为 SDK 通用的扩展接口、TPF 中台支持的业务接口提供一对一的回调映射
     */
    internal static class EventCenter
    {
        public static void Dispatch(TPFSdkEventType type, TPFSdkInfo info)
        {
            switch (type)
            {
                case TPFSdkEventType.EVENT_TYPE_TPF_GAME_BUSSINESS:
                    DispatchBusinessRequest(info);
                    break;
                case TPFSdkEventType.EVENT_TYPE_COMMON_RESULT:
                    DispatchCommonEvent(info);
                    break;
                default:
                    DispatchBindEvent(type, info);
                    break;
            }
        }

        #region business
        private static Dictionary<int, Business.BusinessBaseRequest> s_BusinessRequestMap = new Dictionary<int, Business.BusinessBaseRequest>();

        private static int s_BusinessSessionID = 0;

        /// <summary>
        /// 注册 SDK 和 `TPFSdkCallback.OnGameResult` 绑定的通用中台业务事件
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Session ID</returns>
        public static int RegisterBusinessRequest(Business.BusinessBaseRequest request)
        {
            s_BusinessRequestMap[++s_BusinessSessionID] = request;
            return s_BusinessSessionID;
        }

        private static void DispatchBusinessRequest(TPFSdkInfo info)
        {
            int sessionID = info.GetInt(TPFParamKey.SESSION_ID_KEY);
            long code = info.GetLong(TPFParamKey.HTTP_RESPOND_CODE);
            string jsonData = info.GetData(TPFParamKey.JSON_DATA_KEY);
            if (s_BusinessRequestMap.TryGetValue(sessionID, out var request))
            {
                Dictionary<string, object> outParam = null;
                try
                {
                    if (code == 200)
                    {
                        outParam = request.OnSuccessReturnData(jsonData);
                    }
                    else
                    {
                        outParam = request.OnFailureReturnData(code, jsonData);
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError("[TPFSDK] internal exception:");
                    Debug.LogException(e);
                    outParam = request.OnExceptionReturnData(e);
                }
                finally
                {
                    request.ProcessReturnData(outParam);
                }
                s_BusinessRequestMap.Remove(sessionID);
            }
            else
            {
                Debug.LogError($"[TPFSDK] invalid session id! {sessionID}, code = {code}, jsonData = {jsonData}");
            }
        }

        /// <summary>
        /// 针对 TPF Business 回调唤醒，自动将 data 封装成 TPFSdkEvent 派发
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="data">data 以 ref 形式传入直接回调到客户端，须保证对象传入后不会做任何修改</param>
        public static void BusinessInvoke(this TPFSdkEventDelegate cb, ref Dictionary<string, object> data)
        {
            TPFSdkEvent sdkEvent = new TPFSdkEvent(TPFSdkEventType.EVENT_TYPE_TPF_GAME_BUSSINESS, new TPFSdkInfo(ref data));
            cb?.Invoke(sdkEvent);
        }
        #endregion

        #region TPF Specific Bind Event

        private static TPFSdkEventDelegate[] s_SepcificCallbackMap = new TPFSdkEventDelegate[(int)TPFSdkEventType.__MAX__];

        /// <summary>
        /// 注册 SDK 底层和 `TPFSdkCallback` 一对一绑定的回调事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public static void RegisterBindEvent(TPFSdkEventType type, TPFSdkEventDelegate callback)
        {
            s_SepcificCallbackMap[(int)type] = callback;
        }

        private static void DispatchBindEvent(TPFSdkEventType type, TPFSdkInfo info)
        {
            TPFSdkEvent sdkEvent = new TPFSdkEvent(type, info);
            var callback = s_SepcificCallbackMap[(int)type];
            if(callback == null)
            {
                Debug.Log($"[TPFSDK] bind event type = {type.ToString()} does not have handler!");
                return;
            }
            callback.Invoke(sdkEvent);
        }
        #endregion

        #region TPF Common Event
        private static Dictionary<string, TPFSdkEventDelegate> s_CommonEventCallbackMap = new Dictionary<string, TPFSdkEventDelegate>();
        /// <summary>
        /// 注册 SDK 利用 `TPFSdkCallback.OnCommonResult` 通用绑定回调扩展的事件
        /// </summary>
        /// <param name="commonEventKey"></param>
        /// <param name="callback"></param>
        public static void RegisterCommonEvent(string commonEventKey, TPFSdkEventDelegate callback)
        {
            s_CommonEventCallbackMap[commonEventKey] = callback;
        }

        private static void DispatchCommonEvent(TPFSdkInfo info)
        {
            string commonEventKey = info.GetData(TPFParamKey.COMMON_EVENT_KEY);
            TPFSdkEvent sdkEvent = new TPFSdkEvent(TPFSdkEventType.EVENT_TYPE_COMMON_RESULT, info);
            s_CommonEventCallbackMap[commonEventKey]?.Invoke(sdkEvent);
        }
        #endregion

    }

}
