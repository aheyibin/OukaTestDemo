using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Utils
{
    internal static class ParamUtils
    {
        public static void GetOfficialHttpRspMeta(IDictionary<string, object> json_dict, out int errCode, out string errMsg)
        {
            var meta = (IDictionary<string, object>)json_dict["meta"];
            errCode = meta.ContainsKey("errorCode") ? (int)(long)meta["errorCode"] : 0;
            errMsg = meta.ContainsKey("errorMessage") ? (string)meta["errorMessage"] : "";
        }

        public static Dictionary<string, object> WarpTPFCallbackParam(int tpfErrorCode, long rspErrorCode, string errorMsg, object extra, string commonEventKey = "")
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param[TPFParamKey.ERROR_CODE] = tpfErrorCode;
            param[TPFParamKey.ERROR_MSG] = errorMsg;
            param[TPFParamKey.COMMON_EVENT_KEY] = commonEventKey;
            param[TPFParamKey.SDK_ERRCODE] = rspErrorCode.ToString();
            param[TPFParamKey.SDK_ERRMSG] = errorMsg;
            param[TPFParamKey.EXTRA] = extra;
            return param;
        }
    }

}
