//#define XLUA
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;



namespace TPFSDK.Utils
{

#if XLUA
    [XLua.LuaCallCSharp]
#endif

    public static class TPFHttpUtils
    {

#if XLUA
        [XLua.CSharpCallLua]
        public static List<Type> _CSharpCallLua = new List<Type>() {
                typeof(Action<long, byte[]>),
            };
#endif

        static string initSession = System.Guid.NewGuid().ToString();

        public static void Init()
        {
            initSession = System.Guid.NewGuid().ToString();
        }

#if UNITY_2018_1_OR_NEWER
        public class BypassCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                //Simply return true no matter what
                return true;
            }
        }
#endif

        public static byte[] UTF8String2Bytes(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new byte[0];
            }

            return Encoding.UTF8.GetBytes(str);
        }

        public static string Bytes2UTF8String(byte[] bytes)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            return System.Text.Encoding.UTF8.GetString(bytes);
        }


        public static void Post(string url, IDictionary<string, string> headers, string rawString, Action<long, byte[]> callback)
        {
            var raw = Encoding.UTF8.GetBytes(rawString);
            Post(url, headers, raw, callback);
        }

        public static void Post(string url, IDictionary<string, string> headers, byte[] raw, Action<long, byte[]> callback)
        {
               var unityWebRequest = new UnityWebRequest(
                url,
                UnityWebRequest.kHttpVerbPOST,
                new DownloadHandlerBuffer(),
                new UploadHandlerRaw(raw) { contentType = "application/json" });

#if UNITY_2018_1_OR_NEWER
            unityWebRequest.certificateHandler = new BypassCertificate();
#endif
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    unityWebRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            var lastInitSession = initSession;
            var asyncOperation = unityWebRequest.SendWebRequest();
            asyncOperation.completed += operation =>
            {
                var unityWebRequestAsyncOperation = operation as UnityWebRequestAsyncOperation;
                var downloadHandler = unityWebRequestAsyncOperation.webRequest.downloadHandler;
                if (callback != null && lastInitSession == initSession)
                {
                    var request = unityWebRequestAsyncOperation.webRequest;
                    UnityEngine.Debug.Log(string.Format("url[{0}], netError[{1}], httpError[{2}], errorMsg[{3}], respCode[{4}]",
                                        url, request.isNetworkError, request.isHttpError, request.error, request.responseCode));
                    callback(unityWebRequestAsyncOperation.webRequest.responseCode, downloadHandler.data);
                }
            };
        }

        public static void GetCommonHttpRspMeta(Dictionary<string, object> rsp, out int errCode, out string errMsg)
        {
            var metaNode = (Dictionary<string, object>)rsp["meta"];
            errCode = metaNode.ContainsKey("errCode") ? (int)(long)metaNode["errCode"] : 0;
            errMsg = metaNode.ContainsKey("errMsg") ? (string)metaNode["errMsg"] : "";
        }

        public static Dictionary<string, string> GetCommonHttpHeaders(string appId, string channelId)
        {
            var headers = new Dictionary<string, string>
            {
                {"appId", appId},
                {"channelId", channelId}
            };

            return headers;
        }

        public static Dictionary<string, string> GetTPFHttpHeader(string appId, string appKey, string providerId, string appSecret, string msgId, string bodyJson)
        {
            long timeStamp = TimeUtils.ToUnixTime(DateTime.Now);
            string contentMd5 = System.Convert.ToBase64String(MD5Utils.GetMD5Hash(bodyJson));

            string sign
                    = appId + "\n"
                    + msgId + "\n"
                    + contentMd5 + "\n"
                    + appKey + "\n"
                    + timeStamp + "\n"
                    + appSecret;

            UnityEngine.Debug.Log("GetTPFHttpHeader sign: " + sign);

            sign = MD5Utils.GetMD5String(sign);

            UnityEngine.Debug.Log("GetTPFHttpHeader signMd5: " + sign);

            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "X-Tpf-App-Id", appId },
                { "X-Tpf-Msg-Id", msgId },
                { "X-Tpf-Provider-Id", providerId },
                { "X-Tpf-App-Key", appKey },
                { "X-Tpf-Signature", sign },
                { "X-Tpf-Timestamp", timeStamp.ToString() },
                { "X-Tpf-Version", "1.0.0" },
            };

            return header;
        }

        public static Dictionary<string, string> GetTPFHttpHeader(string appId, string appKey, string providerId, string appSecret, string msgId, string bodyJson, string channelId)
        {
            long timeStamp = TimeUtils.ToUnixTime(DateTime.Now);
            string contentMd5 = System.Convert.ToBase64String(MD5Utils.GetMD5Hash(bodyJson));

            string sign
                    = appId + "\n"
                    + msgId + "\n"
                    + contentMd5 + "\n"
                    + appKey + "\n"
                    + timeStamp + "\n"
                    + appSecret;

            UnityEngine.Debug.Log("GetTPFHttpHeader sign: " + sign);

            sign = MD5Utils.GetMD5String(sign);

            UnityEngine.Debug.Log("GetTPFHttpHeader signMd5: " + sign);

            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "X-Tpf-App-Id", appId },
                { "X-Tpf-Msg-Id", msgId },
                { "X-Tpf-Provider-Id", providerId },
                { "X-Tpf-App-Key", appKey },
                { "X-Tpf-Signature", sign },
                { "X-Tpf-Timestamp", timeStamp.ToString() },
                { "X-Tpf-Version", "1.0.0" },
                { "appId", appId },
                { "channelId", channelId }
            };

            return header;
        }

        public static Dictionary<string, string> GetConfigHeader(string appId, string appKey, string appSecret, string paramJson)
        {
            long timeStamp = TimeUtils.ToUnixTime(DateTime.Now);


            UnityEngine.Debug.Log(string.Format("paramJson[{0}]", paramJson));
            //string contentMd5 = Base64.Base64Encode(MD5Utils.GetMD5Has(paramJson));
            string contentMd5 = System.Convert.ToBase64String(MD5Utils.GetMD5Hash(paramJson));

            UnityEngine.Debug.Log(string.Format("contentMd5[{0}]", contentMd5));

            string sign
                    = "700006" + "\n"
                    + "4001" + "\n"
                    + contentMd5 + "\n"
                    + appKey + "\n"
                    + timeStamp + "\n"
                    + appSecret;

            UnityEngine.Debug.Log(string.Format("sign[{0}]", sign));
            //sign = System.Text.Encoding.UTF8.GetString(MD5Utils.GetMD5Hash(sign));
            sign = MD5Utils.GetMD5String(sign);

            UnityEngine.Debug.Log(string.Format("signMD5[{0}]", sign));

            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "X-Tpf-App-Id", appId },
                { "X-Tpf-Msg-Id", "4001" },
                { "X-Tpf-Provider-Id", "700005" },
                { "X-Tpf-App-Key", appKey },
                { "X-Tpf-Signature", sign },
                { "X-Tpf-Timestamp", timeStamp.ToString() },
                { "X-Tpf-Version", "1.0.0" },
            };
            return header;
        }


        public static Dictionary<string, string> GetConfigParam(string appId, string channelId, string configVer)
        {
            var configParam = new Dictionary<string, string>
            {
                { "appId", appId },
                { "regionId", channelId },
                { "version", "1.0.0" },
                { "cfgId", configVer },
                { "cfgType", "string" },
            };

            return configParam;
        }


    }
}