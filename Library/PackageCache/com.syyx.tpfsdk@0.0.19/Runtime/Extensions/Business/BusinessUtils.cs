using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business
{

    internal static class BusinessUtils
    {
        public static TPFSdkInfo WarpTransparentTransmissionParam(int sessionId, string url, Dictionary<string, string> header, string body)
        {
            TPFSdkInfo info = new TPFSdkInfo();
            info.SetData(TPFParamKey.SESSION_ID_KEY, sessionId.ToString());
            info.SetData(TPFParamKey.URL, url);
            info.SetData(TPFParamKey.HEADER, MiniJSON.Json.Serialize(header));
            info.SetData(TPFParamKey.BODY, body);
            return info;
        }

        public static Dictionary<string, object> WarpCallbackParam(int tpfErrorCode, string tpfErrorMsg, long rspErrorCode, string rspErrorMsg, object extra)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param[TPFParamKey.ERROR_CODE] = tpfErrorCode;
            param[TPFParamKey.ERROR_MSG] = tpfErrorMsg;
            param[TPFParamKey.SDK_ERRCODE] = rspErrorCode.ToString();
            param[TPFParamKey.SDK_ERRMSG] = rspErrorMsg;
            param[TPFParamKey.EXTRA] = extra;
            return param;
        }

        public static Dictionary<string, object> WarpRequestExceptionParam(int tpfExecptionCode)
        {
            return WarpCallbackParam(tpfExecptionCode, "sdk internal exception", 0, "", null);
        }

        public static Dictionary<string, object> WarpRequestFailureParam(int tpfFailureCode, long rspErrorCode, string rspErrorMsg)
        {
            return WarpCallbackParam(tpfFailureCode, "http error", rspErrorCode, rspErrorMsg, null);
        }

        public static Dictionary<string, object> WarpMessageReq(string srcService, string msgId, object msgContent)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["srcService"] = srcService;
            param["msgId"] = msgId;
            List<object> msgContentList = new List<object>();
            msgContentList.Add(msgContent);
            param["msgContent"] = MiniJSON.Json.Serialize(msgContentList);
            return param;
        }

        public static Dictionary<string, object> GetMessageSrvRespMeta(string respJsonStr, out int errCode)
        {
            var resp = (Dictionary<string, object>)MiniJSON.Json.Deserialize(respJsonStr);
            var jsonContent = (string)resp["msgContent"];
            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("no msgContent!");
                errCode = 100;
                return null;
            }
            var msgContent = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonContent);
            errCode = Convert.ToInt32(msgContent["errorCode"]);
            return msgContent;
        }
    }

}
