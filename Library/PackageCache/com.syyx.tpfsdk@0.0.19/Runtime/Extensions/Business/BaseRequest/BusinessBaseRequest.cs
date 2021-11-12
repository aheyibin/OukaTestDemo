using System;
using System.Collections;
using System.Collections.Generic;
using TPFSDK.Utils;
using UnityEngine;


namespace TPFSDK.Business
{
    internal abstract class BusinessBaseRequest 
    {
        private string m_ProviderId, m_MsgId;

        public BusinessBaseRequest(string providerId, string msgId)
        {
            m_ProviderId = providerId;
            m_MsgId = msgId;
        }

        protected void SetMsgId(string msgId)
        {
            m_MsgId = msgId;
        }

        public abstract Dictionary<string, object> OnSuccessReturnData(string response);

        public abstract Dictionary<string, object> OnFailureReturnData(long code, string msg);

        public abstract Dictionary<string, object> OnExceptionReturnData(Exception e);

        protected virtual string GetUrl()
        {
            return ITPFSdk.Instance.networkConfig.tpfProxyUrl;
        }

        protected virtual Dictionary<string, string> GetHeader()
        {
            var sdk = ITPFSdk.Instance;
            var bodyJson = MiniJSON.Json.Serialize(GetBody());
            return TPFHttpUtils.GetTPFHttpHeader(sdk.appId, sdk.appKey, m_ProviderId, sdk.appSecret, m_MsgId, bodyJson, sdk.channelId);
        }

        protected abstract object GetBody();

        /// <summary>
        /// 处理请求在成功、失败、异常情况下返回的参数
        /// </summary>
        /// <param name="data">来自 OnSuccessReturn, OnFailureReturn, OnExceptionReturn 的返回参数</param>
        public abstract void ProcessReturnData(Dictionary<string, object> data);

        public void Post()
        {
            int sessionId = EventCenter.RegisterBusinessRequest(this);
            string url = GetUrl();
            Dictionary<string, string> header = GetHeader();
            string bodyJson = MiniJSON.Json.Serialize(GetBody());
            var req = BusinessUtils.WarpTransparentTransmissionParam(sessionId, url, header, bodyJson);
            ITPFSdk.Instance.SendGameRequest(req);
        }
    }

}
