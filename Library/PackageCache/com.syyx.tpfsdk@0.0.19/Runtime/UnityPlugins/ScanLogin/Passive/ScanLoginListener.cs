using System;
using System.Collections.Generic;
using System.Text;
using TPFSDK.Utils;
using UnityEngine;

namespace TPFSDK.Normal.UnityPlugins.Passive
{
    /**
     * 处理扫码登录状态变化的对象
     */
    public class ScanLoginListener : MonoBehaviour
    {
        private float _pollingInterval = 3f; // 轮询查询状态间隔
        private ScanLoginStatus _scanLoginStatus = ScanLoginStatus.Idle;
        private string _qrCodeStatus = QRCodeStatusCode.Init;
        private string _qrId; // 二维码链接中携带着的唯一id 
        private float _elpasedTime = 0; // 轮询记录流逝的时间，超过_pollingQueryInterval后将触发轮询响应，并重置
        private bool _waitingHttpRsp = false; // 是否等待Http响应，等待Http响应时不进行轮询计时增加

        private string _tpfId; // 登录成功后服务端会带回平台id
        private string _token; // 登录成功后服务端会带回token
        private string _clientExtra; // 登陆成功后服务端回带回手机客户端上传的额外信息
        
        public event Action<ScanLoginStatus> OnStateChange;

		public ScanLoginStatus Status
        {
            get { return _scanLoginStatus; }
        }

        /**
         * 获取登录平台id，只有登录成功后会拿到id
         */
        public string TpfId
        {
            get { return _tpfId; }
        }

        /**
         * 获取登录token，只有登录成功后会拿到token
         */
        public string Token
        {
            get { return _token; }
        }

        /**
         * 获取手机端传入的额外信息
         */
        public string ClientExtra
        {
            get { return _clientExtra; }
        }

        /**
         * 获取/设置轮询间隔
         */
        public float PollingInterval
        {
            get { return _pollingInterval; }
            set { _pollingInterval = value; }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (_waitingHttpRsp)
            {
                return;
            }

            _elpasedTime += Time.deltaTime;
            if (_elpasedTime >= _pollingInterval)
            {
                _elpasedTime = 0;
                DoQueryStatus();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            // 游戏从后台恢复时，立即进行一次轮询
            if (!pause)
            {
                Debug.Log("Force Polling");
                _elpasedTime = PollingInterval;
            }
        }

		public void SetOnStateChange(Action<ScanLoginStatus> callback)
		{
			OnStateChange = callback;
		}

		/**
         * 启动，开始轮询登录状态
         */
		//public bool Setup(string url, string appId, string appKey, string appSecret, string providerId, string msgId, string qrId, int pollingInterval)
        public bool Setup(string qrId, int pollingInterval)

        {
            _qrId = qrId;
            _pollingInterval = pollingInterval;
            return true;
        }

        /**
         * 登录流程结束时（包括发生错误）调用，清理资源
         */
        public void OnFinish()
        {
            Destroy(gameObject);
        }

        /**
         * 改变当前登录状态，同时触发调用注册的事件响应
         */
        private void SetScanLoginStatus(ScanLoginStatus status)
        {
            if (_scanLoginStatus != status)
            {
                _scanLoginStatus = status;
                if (OnStateChange != null)
                {
                    OnStateChange(_scanLoginStatus);
                }
            }
        }

        /**
         * 像服务端查询状态，由客户轮询发起
         */
        private void DoQueryStatus()
        {
            var data = new Dictionary<string, object>
            {
                {"qrId", _qrId},
                {"deviceId", SystemInfo.deviceUniqueIdentifier },
                {"clientTime", DateTimeOffset.Now.ToUnixTimeMilliseconds()}
            };

            //var data_json = MiniJSON.Json.Serialize(data);
            //var headers = TPFHttpUtils.GetTPFHttpHeader(_appId, _appKey, _providerId, _appSecret, _msgId, data_json, "-1"); // 此时并没有确定的渠道，channelId传-1

            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            ITPFSdk.Instance.requestAdapter.DoQueryStatusTPF(ref url, ref header, ref data_json, data);

            _waitingHttpRsp = true;
            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    _waitingHttpRsp = false;

                    if (code != 200)
                    {
                        SetScanLoginStatus(ScanLoginStatus.LoginFailed);
                        OnFinish();
                        return;
                    }

                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json, out errCode, out errMsg);

                        if (errCode == TPFCode.TPFCODE_SUCCESS)
                        {
                            //Debug.LogError($"query qr code status rsp = {jsonData}");
                            // 检测服务端返回的状态
                            var dataNode = (Dictionary<string, object>)json["data"];
                            var remoteQrCodeStatus = (string)dataNode["status"];

//                            Log.Info("QRCode status: " + remoteQrCodeStatus);

                            if (_qrCodeStatus != remoteQrCodeStatus)
                            {
                                _qrCodeStatus = remoteQrCodeStatus;
                                switch (_qrCodeStatus)
                                {
                                    case QRCodeStatusCode.Init:
                                        SetScanLoginStatus(ScanLoginStatus.Idle);
                                        break;
                                    case QRCodeStatusCode.ConfirmLogin:
                                        SetScanLoginStatus(ScanLoginStatus.ConfirmLogin);
                                        break;
                                    case QRCodeStatusCode.CancelLogin:
                                        SetScanLoginStatus(ScanLoginStatus.CancelLogin);
                                        OnFinish();
                                        break;
                                    case QRCodeStatusCode.Timeout:
                                        SetScanLoginStatus(ScanLoginStatus.Timeout);
                                        OnFinish();
                                        break;
                                    case QRCodeStatusCode.VerifyLogin:
                                        _tpfId = (string) dataNode["id"];
                                        _token = (string) dataNode["token"]; 

                                        // 这里做一次Base64的转换对应的扫码一侧也做过Base64编码，目的是为了避免传递的数据中特殊字符影响Json序列化
                                        var clientExtraEncoded = (string) dataNode["clientExtra"];
                                        _clientExtra = string.IsNullOrEmpty(clientExtraEncoded)
                                            ? ""
                                            : Encoding.UTF8.GetString(Convert.FromBase64String(clientExtraEncoded));

                                        SetScanLoginStatus(ScanLoginStatus.LoginSuccess); // 此时表示登录成功
                                        OnFinish();
                                        break;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError(string.Format("DoQueryStatus error({0}), msg:{1}", errCode, errMsg));
                            SetScanLoginStatus(ScanLoginStatus.LoginFailed);
                            OnFinish();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        OnFinish();
                    }
                });
        }
    }
}