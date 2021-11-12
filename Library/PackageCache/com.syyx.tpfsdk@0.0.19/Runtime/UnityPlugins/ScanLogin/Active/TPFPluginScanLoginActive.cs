using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TPFSDK.Normal.UnityPlugins.Passive;
using TPFSDK.Utils;
using UnityEngine;

namespace TPFSDK.Normal.UnityPlugins
{
    /**
     * 封装扫码登录管理接口（扫码端，如android/ios端）
     * Active表示扫码的一方
     */
    public sealed class TPFPluginScanLoginActive
    {
        private static readonly object locker = new object();
        private static TPFPluginScanLoginActive instance;

        public static TPFPluginScanLoginActive Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new TPFPluginScanLoginActive();
                        }
                    }
                }

                return instance;
            }
        }

        private TPFPluginScanLoginActive()
        {
        }

        /**
         * 检测是否是支持的登录Url
         */
        public bool IsSupportUrl(string url)
        {
            if (! (url.StartsWith("http://") || url.StartsWith("https://")))
            {
                return false;
            }

            var queryStrigns = GetQueryStringsFromUrl(url);
            if (!queryStrigns.ContainsKey("purp"))
            {
                return false;
            }

            if (queryStrigns["purp"] != "login")
            {
                return false;
            }

            if (!queryStrigns.ContainsKey("qrId"))
            {
                return false;
            }

            return true;
        }

        /**
         * 扫码但是未执行鉴权登录
         */
        public bool PreLogin(string qrCodeUrl) 
        {
            if (!IsSupportUrl(qrCodeUrl))
            {
                return false;
            }

            UpdateLoginState(
                qrCodeUrl, QRCodeStatusCode.ConfirmLogin,/*tpfId*/string.Empty, /*token*/string.Empty, /*clientExtra*/string.Empty,
                (code, dict) =>
                {
                    var tpfSdkInfo = new TPFSdkInfo();
                    tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, code);

                    if (code == TPFCode.TPFCODE_SUCCESS)
                    {
                        var status = dict["status"];
                        if (status == QRCodeStatusCode.ConfirmLogin)
                        {
                            tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.ConfirmLogin);
                        }
                        else
                        {
                            // 状态不匹配，错误
                            Debug.LogError("UpdateLoginState: ConfirmLogin rsp status not match.");
                            tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, TPFCode.TPFCODE_SCAN_LOGIN_RSP_STATUS_ERROR);
                            tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.LoginFailed);
                        }
                    }

                    TPFSdkCallback.Instance.OnScanLoginStatusChange(tpfSdkInfo.ToJson());
                });

            return true;
        }

        /**
         * 取消登录
         */
        public bool CancelLogin(string qrCodeUrl)
        {
            if (!IsSupportUrl(qrCodeUrl))
            {
                return false;
            }

            UpdateLoginState(
                qrCodeUrl, QRCodeStatusCode.CancelLogin, /*tpfId*/string.Empty, /*token*/string.Empty, /*clientExtra*/string.Empty,
                (code, dict) =>
                {
                    var tpfSdkInfo = new TPFSdkInfo();

                    if (code == TPFCode.TPFCODE_SUCCESS)
                    {
                        var status = dict["status"];
                        if (status == QRCodeStatusCode.CancelLogin)
                        {
                            tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, code);
                            tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.CancelLogin);
                        }
                        else
                        {
                            // 状态不匹配，错误
                            Debug.LogError("UpdateLoginState: ConfirmLogin rsp status not match.");
                            tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, TPFCode.TPFCODE_SCAN_LOGIN_RSP_STATUS_ERROR);
                            tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.LoginFailed);
                        }
                    }
                    else
                    {
                        tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, code);
                        tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.LoginFailed);
                    }

                    TPFSdkCallback.Instance.OnScanLoginStatusChange(tpfSdkInfo.ToJson());
                });

            return true;
        }

        /**
         * 执行鉴权登录
         */
        public bool AuthLogin(string qrCodeUrl, string tpfId, string token, string clientExtra)
        {
            if (!IsSupportUrl(qrCodeUrl))
            {
                return false;
            }

            UpdateLoginState(
                qrCodeUrl, QRCodeStatusCode.VerifyLogin, tpfId, token, clientExtra,
                (code, dict) =>
                {
                    var tpfSdkInfo = new TPFSdkInfo();

                    if (code == TPFCode.TPFCODE_SUCCESS)
                    {
                        var status = dict["status"];
                        if (status == QRCodeStatusCode.VerifyLogin)
                        {
                            tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, code);
                            tpfSdkInfo.SetData(TPFParamKey.ID, dict["id"]);
                            tpfSdkInfo.SetData(TPFParamKey.TOKEN, dict["token"]);
                            tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.LoginSuccess);
                            tpfSdkInfo.SetData(TPFParamKey.CLIENT_EXTRA, dict["clientExtra"]);
                        }
                        else
                        {
                            // 状态不匹配，错误
                            Debug.LogError("UpdateLoginState: ConfirmLogin rsp status not match.");
                            tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, TPFCode.TPFCODE_SCAN_LOGIN_RSP_STATUS_ERROR);
                            tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.LoginFailed);
                        }
                    }
                    else
                    {
                        tpfSdkInfo.SetData(TPFParamKey.ERROR_CODE, code);
                        tpfSdkInfo.SetData(TPFParamKey.SCAN_LOGIN_STATUS, (int)ScanLoginStatus.LoginFailed);
                    }

                    TPFSdkCallback.Instance.OnScanLoginStatusChange(tpfSdkInfo.ToJson());
                });

            return true;
        }

        /**
         * 更新登录状态
         */
        private void UpdateLoginState(
            string qrCodeUrl, string qrCodeStatus, string tpfId, string token, string clientExtra, Action<int, IDictionary<string, string>> callback)
        {
            var queryStrings = GetQueryStringsFromUrl(qrCodeUrl);
            var qrId = queryStrings["qrId"];
            var clientExtraEncoded =
                string.IsNullOrEmpty(clientExtra)
                    ? string.Empty
                    : Convert.ToBase64String(Encoding.UTF8.GetBytes(clientExtra)); // 做Base64编码目的是为了避免传递的数据中特殊字符影响Json序列化
            var data = new Dictionary<string, object>
            {
                {"id", tpfId},
                {"token", token},
                {"qrId", qrId},
                {"deviceId", SystemInfo.deviceUniqueIdentifier },
                {"clientTime", DateTimeOffset.Now.ToUnixTimeMilliseconds()},
                {"clientExtra", clientExtraEncoded},
                {"status", qrCodeStatus},
            };

            //string data_json = MiniJSON.Json.Serialize(data);
            //Debug.Log("scan qrcode login update state request: " + data_json);
            //var header = TPFHttpUtils.GetTPFHttpHeader(appId, appKey, TPFSdkProxyConfig.tpfLoginProvideID, appSecret, TPFSdkProxyConfig.tpfLoginUpdateQRCodeStatusMsgID, data_json, channelId);
            
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            ITPFSdk.Instance.requestAdapter.UpdateLoginStateTPF(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        if (callback != null)
                        {
                            callback(TPFCode.TPFCODE_FAIL, null);
                        }

                        return;
                    }

                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json, out errCode, out errMsg);

                        if (errCode != (int)MetaErrCode.Success)
                        {
                            Debug.Log("UpdateLoginState failed, meta.errCode: " + errCode);
                            if (errCode == (int)MetaErrCode.FailQrCodeExpired || errCode == (int)MetaErrCode.FailQrCodeOccupied)
                            {
                                callback(TPFCode.TPFCODE_QR_CODE_INVALID, null);
                            }
                            else if (errCode == (int) MetaErrCode.FailTokenInvalid)
                            {
                                callback(TPFCode.TPFCODE_TOKEN_INVALID, null);
                            }
                            else
                            {
                                callback(TPFCode.TPFCODE_QR_CODE_ERROR, null);
                            }

                            return;
                        }

                        var dataNode = (Dictionary<string, object>)json["data"];
                        var dict = new Dictionary<string, string>();
                        foreach (var dataNodeKv in dataNode)
                        {
                            dict.Add(dataNodeKv.Key, (string)dataNodeKv.Value);
                        }

                        if (callback != null)
                        {
                            callback(TPFCode.TPFCODE_SUCCESS, dict);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        if (callback != null)
                        {
                            callback(TPFCode.TPFCODE_FAIL, null);
                        }
                    }
                });
        }

        private IDictionary<string, string> GetQueryStringsFromUrl(string url)
        {
            var dict = new Dictionary<string, string>();

            var uri = new Uri(url);
            var fullQuery = uri.Query.StartsWith("?") ? uri.Query.Remove(0, 1) : uri.Query;
            var queryArray = fullQuery.Split('&');

            foreach (var query in queryArray)
            {
                var queryKv = query.Split('=');
                if (queryKv.Length != 2)
                {
                    continue;
                }

                dict.Add(queryKv[0], queryKv[1]);
            }

            return dict;
        }
    }
}