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
     * 封装扫码登录管理接口（被扫码端，如win端）
     * Passive表示被扫码的一方
     */
    public sealed class TPFPluginScanLoginPassive
    {
        private const int DefaultQRCodeTexWidth = 300;
        private const int DefaultQRCodeTexHeight = 300;

        private static readonly object locker = new object();
        private static TPFPluginScanLoginPassive instance;

        public static TPFPluginScanLoginPassive Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new TPFPluginScanLoginPassive();
                        }
                    }
                }

                return instance;
            }
        }

        private TPFPluginScanLoginPassive()
        {
        }

        /**
         * 获取登录二维码
         */
        public bool GetQRCodeImage(Action<int, string, string> callback, string extra = null)
        {
            extra = extra ?? string.Empty;
            var data = new Dictionary<string, object>
            {
                {"clientExtra", extra},
                {"deviceId", SystemInfo.deviceUniqueIdentifier },
                {"clientTime", DateTimeOffset.Now.ToUnixTimeMilliseconds()}
            };
            //var data_json = MiniJSON.Json.Serialize(data);
            //var headers = TPFHttpUtils.GetTPFHttpHeader(appId, appKey, TPFSdkProxyConfig.tpfLoginProvideID, appSecret, TPFSdkProxyConfig.tpfLoginGetQRCodeMsgID, data_json, "-1"); // 此时并没有确定的渠道，channelId传-1

            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            ITPFSdk.Instance.requestAdapter.GetQRCodeImageTPF(ref url, ref header, ref data_json, data);


            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        if (callback != null)
                        {
                            callback(TPFCode.TPFCODE_GET_QR_CODE_ERROR, null, null);
                        }

                        return;
                    }

                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                        Debug.LogError("get qe image rsp = "+jsonData);
                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json, out errCode, out errMsg);

                        if (errCode != TPFCode.TPFCODE_SUCCESS)
                        {
                            if (callback != null)
                            {
                                callback(TPFCode.TPFCODE_GET_QR_CODE_ERROR, null, null);
                            }

                            return;
                        }

                        var dataNode = (Dictionary<string, object>) json["data"];
                        var qrCodeImgByteString = (string) dataNode["qrImg"];
                        var qrId = (string) dataNode["qrId"];

                        if (callback != null) 
                        {
                            callback(TPFCode.TPFCODE_SUCCESS, qrId, qrCodeImgByteString);
                        }
                        /** return raw image data
                        var qrCodeImgBytes = Convert.FromBase64String(qrCodeImgByteString);
                        var texture = new Texture2D(DefaultQRCodeTexWidth, DefaultQRCodeTexHeight);

                        var loadImgRet = texture.LoadImage(qrCodeImgBytes);
                        if (loadImgRet)
                        {
                            if (callback != null)
                            {
								var spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
								callback(TPFCode.TPFCODE_SUCCESS, qrId, spr);
                            }
                        }
                        else
                        {
                            if (callback != null)
                            {
                                callback(TPFCode.TPFCODE_GET_QR_CODE_ERROR, null, null);
                            }
                        }
                        */
                    }
                    catch (Exception e)
                    {
                        TPFLog.Exception(e);
                        if (callback != null)
                        {
                            callback(TPFCode.TPFCODE_GET_QR_CODE_ERROR, null, null);
                        }
                    }
                });

            return true;
        }

        /**
         * 获取扫码登录代理
         */
        public ScanLoginListener StartListen(string qrId, int pollingInterval)
        {
            var gameObject = new GameObject(typeof(ScanLoginListener).Name);
            var listener = gameObject.AddComponent<ScanLoginListener>();
            //if (!listener.Setup(url, appId, appKey, appSecret, TPFSdkProxyConfig.tpfLoginProvideID, TPFSdkProxyConfig.tpfLoginQueryQRCodeStatusMsgID, qrId, pollingInterval))
            if (!listener.Setup(qrId, pollingInterval))
            {
                TPFLog.Error("StartListen failed. please check the params.");
                UnityEngine.Object.Destroy(gameObject);
                return null;
            }
            return listener;
        }
    }
}