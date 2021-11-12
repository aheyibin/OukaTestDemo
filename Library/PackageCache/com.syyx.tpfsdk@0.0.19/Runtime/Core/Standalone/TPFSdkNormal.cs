/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的PC 和编辑器接口实现（未实现）
** use: demo
** modify:             
*******************************************************************/

#if UNITY_EDITOR || UNITY_STANDLONE || UNITY_STANDALONE_WIN
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TPFSDK.Utils;
using UnityEngine;
using System.Linq;
using System.IO;

namespace TPFSDK
{

    public class TPFSdkNormal : ITPFSdk
    {
        private enum OfficialChannelMetaErrCode
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success = 0,

            /// <summary>
            /// 游戏不存在
            /// </summary>
            GameNotExist = 1001,

            /// <summary>
            /// 系统错误
            /// </summary>
            SystemError = 1002,

            /// <summary>
            /// 签名认证失败
            /// </summary>
            FailAuthSign = 1003,

            /// <summary>
            /// 必须参数为空
            /// </summary>
            NecessaryParameterIsNull = 1004,

            /// <summary>
            /// 帐户不存在
            /// </summary>
            AccountNotExist = 1005,

            /// <summary>
            /// 参数错误（超出参数范围的参数--ex 帐号类型只有0 1 2 参数传入4)
            /// </summary>
            ParameterError = 1006,

            /// <summary>
            /// 注册失败，验证码校验失败
            /// </summary>
            FailRegister_WrongAuthCode = 1011,

            /// <summary>
            /// 登陆失败，密码错误
            /// </summary>
            FailLogin_WrongPassword = 1027,

            /// <summary>
            /// 注册失败账号已存在
            /// </summary>
            FailRegister_AccountAlreadyExist = 1037,

        }
        public enum OfficalVerifyCodeType
        {
            Login = 0,
            Register = 1,
            Bind = 2,
            ChangePwd = 3,
            ForgetPwd = 4,
        }
        public enum QueryBindingState
        {
            HasBindedPhone = 1,
            HasNoBindedPhone = 0,
        }
        private enum TPFLoginMetaErrCode
        {
            /// <summary>
            /// 操作成功
            /// </summary>
            Success = 0,
            /// <summary>
            /// 操作失败
            /// </summary>
            FailOperation = 15000,

            /// <summary>
            /// 系统错误
            /// </summary>
            SystemError = 15001,

            /// <summary>
            /// 必须参数为空
            /// </summary>
            NecessaryParameterIsNull = 15002,

            /// <summary>
            /// 非法用户
            /// </summary>
            Illegal = 15003,

            /// <summary>
            /// 应用不存在
            /// </summary>
            AppNotExist = 15010,

            /// <summary>
            /// 渠道不存在
            /// </summary>
            ChannelNotExist = 15011,

            /// <summary>
            /// 应用暂未开放使用
            /// </summary>
            NotOpenApp = 15012,

            /// <summary>
            /// 渠道暂未开放使用
            /// </summary>
            NotOpenChannel = 15013,

            /// <summary>
            /// 渠道校验不通过
            /// </summary>
            FailChannelAuth = 15100,
        }
        private enum TPFKVStorageCode
        {
            /// <summary>
            /// 操作成功
            /// </summary>
            Success = 200,

            /// <summary>
            /// token错误
            /// </summary>
            SystemError = 1040,

        }

        public TPFSdkNormal() { }

        private string m_UserId;
        private string m_Token;

        //QQ一键加群
        internal override bool JoinQQGroup()
        {
            return false;
        }
        public override void GetQQConfig()
        {
            
        }

        public override string GetSubChannelID()
        {
#if !UNITY_EDITOR && UNITY_STANDALONE && SHUN_WANG
            // 顺网端
            return "13";
#endif
            return "0";
        }

        private const string ENV_CONFIG_FILE_NAME = "tpfSdk_Env.cfg";

        /// <summary>
        /// win 端 environment 配置放在 streamingAssets 目录下
        /// </summary>
        void ReadEnvConfig()
        {
            // Load Test Environment
            string envPath = PathUtil.Combine(Application.streamingAssetsPath, ENV_CONFIG_FILE_NAME);
            if (File.Exists(envPath))
            {
                string envCfgText = File.ReadAllText(envPath);

                if (envCfgText != null)
                {
                    var config = MiniJSON.Json.Deserialize(envCfgText) as Dictionary<string, object>;
                    Debug.Log(string.Format("TPFSdkAndroid tpfSdk_Env {0}", envCfgText));

                    if (config == null) return;

                    foreach (var item in config)
                    {
                        environConfig[item.Key] = item.Value;
                    }

                    if (environConfig.ContainsKey("channelId"))
                    {
                        channelId = environConfig["channelId"] as string;
                    }
                }
            }
        }


        Dictionary<string, object> environConfig = new Dictionary<string, object>();
        public override string getEnvironConfig()
        {
            return MiniJSON.Json.Serialize(environConfig);
        }

        internal override void Init()
        {
            base.Init();
            ReadEnvConfig();
            SwitchEnv(getEnvironConfig());
        }

        public override string GetAppID()
        {
            return appId;
        }

        public override string GetAppKey()
        {
            return appKey;
        }

        public override string GetAppSecret()
        {
            return appSecret;
        }

        public override string GetChannelID()
        {
            return channelId;
        }

        public override string ReadDeviceIdentify()
        {
            return UnityEngine.SystemInfo.deviceUniqueIdentifier;
        }

        public override bool IsOfficial()
        {
            return true;
        }

        #region "user"

        public override bool GetUserInfo()
        {
            //  windows do not support
            return false;
        }

        internal override bool Register(TPFSdkInfo info)
        {
            RegisterOfficial(info);
            return true;
        }

        internal override bool Login(TPFSdkInfo info)
        {
            if (info.ContainsKey("id") && !string.IsNullOrEmpty(info.GetData("id")) && info.ContainsKey("tokenL") && !string.IsNullOrEmpty(info.GetData("tokenL")))
            {
                LoginByTokenLOfficial(info.GetData("id"), info.GetData("tokenL"));
            }
            else
            {
                LoginOfficial(info.GetData("account"), info.GetInt("accountType"), info.GetData("password"), info.GetInt("type"), info.GetInt("checkType"));
            }
            return true;
        }

        internal override bool Logout()
        {
            return false;
        }

        internal override bool Exit()
        {
            return false;

        }

        internal override void VerifyCode(TPFSdkInfo info)
        {
            GetVerificationCodeOfficial(info.GetInt("type"), info.GetData("phoneNum"));
        }

        internal override void RealNameVerify(TPFSdkInfo info)
        {
            RealNameVerifyOfficial(info.GetData("id"), info.GetData("tokenL"), info.GetData("idCard"), info.GetData("realName"), info.GetData("cardType"), info.GetData("userId"), info.GetData("token"));
        }

        internal override void QueryRealName(TPFSdkInfo info)
        {
            QueryRealNameVerifyTPF(info.GetData("userId"), info.GetData("token"));
        }

        internal override void ForgetPwd(TPFSdkInfo info)
        {
            ForgetPasswordOfficial(info.GetData("account"), info.GetData("phoneNum"), info.GetData("newPassword"), info.GetData("msgCode"));
        }

        internal override void ChangePwd(TPFSdkInfo info)
        {
            ModifyPasswordOfficial(info.GetData("id"), info.GetData("password"), info.GetData("newPassword"));
        }

        internal override void QueryUserBind(TPFSdkInfo info)
        {
            QueryUserBindOfficial(info.GetData("id"), info.GetData("guestToken"));
        }

        // 绑定手机
        internal override void BindAccount(TPFSdkInfo info)
        {
            TouristBindPhoneOfficial(info.GetData("phoneNum"), info.GetData("msgCode"), info.GetData("id"), info.GetData("tokenL"), info.GetData("password"));
        }

        // 目前只有 win 端实现了，暂不提到上层接口
        public bool BindNewAccount(TPFSdkInfo info)
        {
            TouristBindNewAccountOfficial(info.GetData("newAccount"), info.GetData("password"), info.GetData("id"), info.GetData("tokenL"));
            return true;
        }

        // 礼品包
        internal override bool QueryGiftCode(TPFSdkInfo info)
        {
            QueryGiftCardOfficial(info.GetData("id"), info.GetData("token"));
            return true;
        }

        internal override bool PostGiftCode(TPFSdkInfo info)
        {
            PostGiftCardOfficial(info.GetData("id"), info.GetData("token"), info.GetData("card"), int.Parse(info.GetData("cardType")), info.GetData("playerId"), info.GetData("serverId"));
            return true;
        }

        internal override void AgreeProtocol(TPFSdkInfo info)
        {
            AgreeUserProtocolTPF(info.GetData("userId"), info.GetData("token"));
        }

        #region 角色信息
        private string _roleId;
        private string _serverId;
        private string _subType;
        private string _roleName;
        private string _roleLevel;
        private string _serverName;
        private string _subVip;            //角色会员等级
        public enum SubType
        {
            CreateRole = 1,
            LevelUp    = 2,
            EnterGame  = 3
        }

        private string _submitInfo = string.Empty;
        #endregion
        internal override bool SubmitInfo(TPFSdkInfo info)
        {
            _submitInfo = info.ToJson();
            _roleId = info.GetData(TPFParamKey.ROLE_ID);
            _roleName = info.GetData(TPFParamKey.ROLE_NAME);
            _roleLevel = info.GetData(TPFParamKey.ROLE_LEVEL);
            _serverId= info.GetData(TPFParamKey.SERVER_ID);
            _serverName= info.GetData(TPFParamKey.SERVER_NAME);
            _subVip= info.GetData(TPFParamKey.VIP);
            _subType= info.GetData("Type");
            if(String.IsNullOrEmpty(_roleId)
                || String.IsNullOrEmpty(_serverId)
                || String.IsNullOrEmpty(_subType))
            {
                Debug.LogError("submitInfo param not enough");
                return false;
            }
            return true;
        }

        public override bool CoinTradeEvent(TPFSdkInfo info)
        {
            return false;
        }

        public override bool PropTradeEvent(TPFSdkInfo info)
        {
            return false;
        }

        public override bool GameTaskEvent(TPFSdkInfo info)
        {
            return false;
        }

        public override bool EventReport(string param)
        {
            return false;
        }

        public override bool AdEvent(TPFSdkInfo info)
        {
            return false;
        }

        internal override bool PrePay(TPFSdkInfo info)
        {
            return false;
        }

        internal override bool QueryOrder(TPFSdkInfo info)
        {
            return false;
        }

        internal override bool QueryOrderList(TPFSdkInfo info)
        {
            return false;
        }

        internal override bool ConfirmOrder(TPFSdkInfo info)
        {
            return false;
        }

        internal override void LoadAd(int type, TPFSdkInfo extra)
        {
        }

        internal override void ShowAd(int type, TPFSdkInfo extra)
        {
        }
        #endregion

        internal override void DoSinglePay(TPFSdkInfo info)
        {
        }

        internal override void DoPay(string parameters, string payMethod)
        {
        }

        internal override void QueryOrderAndConfirm(TPFSdkInfo info)
        {
        }

        internal override bool Pay(TPFSdkInfo info) { return false; }

        internal override bool InstallAPK(TPFSdkInfo info) { return false; }

        internal override bool Download(TPFSdkInfo info) { return false; }

        internal override bool PauseDownload(TPFSdkInfo info) { return false; }

        internal override bool ScanQRCode(TPFSdkInfo info)
        {
            Dictionary<string, object> out_param = new Dictionary<string, object>();
            out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_FAIL, 0, "Windows do not support scan QR code!", null);
            var out_json = MiniJSON.Json.Serialize(out_param);
            TPFSdkCallback.Instance.OnScanQRCodeResult(out_json);
            return false;
        }


        public override bool EventReport(string eventId, string eventType, string extra)
        {
            return true;
        }

        public override void OpenWeb(TPFSdkInfo info)
        {
            object type = info.GetDataObject("type");
            object url = info.GetDataObject("url");
            if (type == null)
            {
                type = 1;
            }
            if (url == null)
            {
                url = "www.baidu.com";
            }
            Application.OpenURL((String)url);
        }

        private bool RegisterOfficial(TPFSdkInfo info)
        {
            int type = info.GetInt("type");
            string account;
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            switch (type)
            {
                case 0:
                    account = info.GetData("account");
                    var password = info.GetData("password");
                    password = MD5Utils.GetMD5(password);
                    var data = new Dictionary<string, object>
                    {
                        { "areaId"     , areaId    },
                        { "account"    , account   },
                        { "password"   , password  },
                    };
                    requestAdapter.RegisterAsUserNameOfficial(ref url, ref header, ref data_json, data);
                    break;
                case 1:
                    var phoneNum = info.GetData("phoneNum");
                    var msgCode = info.GetData("msgCode");
                    var password_phone = info.GetData("password");
                    password_phone = MD5Utils.GetMD5(password_phone);
                    account = phoneNum;

                    var data_phone = new Dictionary<string, object>
                    {
                        { "areaId"      , areaId     },
                        { "phoneNum"    , phoneNum   },
                        { "msgCode"     , msgCode    },
                        { "password"    , password_phone },
                        { "phone"       , "windows" },
                    };
                    requestAdapter.RegisterAsPhoneOfficial(ref url, ref header, ref data_json, data_phone);
                    break;
                case 3:
                    account = $"{SystemInfo.deviceUniqueIdentifier}_{Time.time}";
                    var data_tourist = new Dictionary<string, object>
                    {
                        { "areaId"      , areaId     },
                        { "account"     , account},
                    };
                    requestAdapter.RegisterAsTouristOfficial(ref url, ref header, ref data_json, data_tourist);
                    break;
                default:
                    Debug.LogError("unsupported regist type = " + type);
                    return false;
            }
            //Debug.Log("register Post: header: " + MiniJSON.Json.Serialize(header) + " \n, data_json: " + data_json);
            TPFHttpUtils.Post(url, header, data_json, (code, outputData) => {
                if (code != 200)
                {
                    Debug.LogError("Official Channel http respond code: " + code);
                    Dictionary<string, object> out_param = new Dictionary<string, object>();
                    out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_REGISTER_FAIL, code, "http request failed", null);
                    var out_json = MiniJSON.Json.Serialize(out_param);
                    TPFSdkCallback.Instance.OnRegisterResult(out_json);
                    return;
                }
                try
                {
                    var jsonData = Encoding.UTF8.GetString(outputData);
                    //Debug.LogError("register data:  " + jsonData);
                    var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                    Dictionary<string, object> out_param;
                    int errCode;
                    string errMsg;
                    ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);

                    int tpfcode;
                    Dictionary<string, object> extra = null;
                    if (errCode != (int)OfficialChannelMetaErrCode.Success)
                    {
                        if (errCode == (int)OfficialChannelMetaErrCode.FailRegister_AccountAlreadyExist)
                        {
                            tpfcode = TPFCode.TPFCODE_REGISTER_FAIL_ACCOUNT_ALREADY_EXIST;
                            extra = new Dictionary<string, object>();
                            extra["Account"] = account;
                            extra["AccountType"] = type;
                        }
                        else
                        {
                            tpfcode = TPFCode.TPFCODE_REGISTER_FAIL;
                        }
                        //Debug.Log("Regist Offical account failed, meta.errCode = "+errCode+", errMs = " + errMsg);
                    }
                    else
                    {
                        //Debug.Log("Regist Official account success");
                        extra = new Dictionary<string, object>();
                        var data = json_dict["data"] as Dictionary<string, object>;
                        extra["Account"] = data["account"] as string;
                        extra["AccountType"] = Convert.ToInt32(data["type"]).ToString();
                        tpfcode = TPFCode.TPFCODE_REGISTER_SUCCESS;
                    }
                    out_param = ParamUtils.WarpTPFCallbackParam(tpfcode, errCode, errMsg, extra);
                    var out_json = MiniJSON.Json.Serialize(out_param);
                    //Debug.Log("on regist result "+out_json);
                    TPFSdkCallback.Instance.OnRegisterResult(out_json);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
            return true;
        }

        #region 全面屏适配
        public override string GetNotchInfo()
        {
            return "Windows平台不适配";
        }
        #endregion

        #region Offical Login implement
        private void LoginOfficial(string account, int accountType, string password, int type, int checkType)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = account; // for tourist password have to be a non-empty string
            }
            if (checkType == 0)
            {
                password = MD5Utils.GetMD5(password);
            }
            var data = new Dictionary<string, object>
            {
                { "areaId"     , areaId                           },
                { "account"    , account                          },
                { "accountType", accountType                      },
                { "password"   , password                         },
                { "type"       , type                             },
                { "checkType"  , checkType                        },
                { "extra"      , ""                               },
                { "imei"       , SystemInfo.deviceUniqueIdentifier}
            };

            string url = null;
            string body = null;
            Dictionary<string, string> header = null;

            requestAdapter.LoginOfficial(ref url, ref header, ref body, data);
            Debug.Log($"url = {url}, header = {MiniJSON.Json.Serialize(header)}, body = {MiniJSON.Json.Serialize(body)}");

            TPFHttpUtils.Post(url, header, body,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("Official Channel http respond code: "+code);
                        Dictionary<string, object> out_param = new Dictionary<string, object>();
                        out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_FAIL, code, "channel http request failed", null);
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnLoginResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                        if (errCode != (int)OfficialChannelMetaErrCode.Success)
                        {
                            //Debug.Log("Get Offical token failed, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnLoginResult(MiniJSON.Json.Serialize(out_param));
                            return;
                        }
                        var dataNode = (Dictionary<string, object>)json_dict["data"];

                        //Debug.Log("Get official token success! " + jsonData);
                        AuthChannelTokenTPF(MiniJSON.Json.Serialize(dataNode)); // 传入渠道拿到的信息
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }

        private void LoginByTokenLOfficial(string id, string tokenL)
        {
            var data = new Dictionary<string, object>
            {
                { "areaId"     , areaId                           },
                { "id"         , id                               },
                { "tokenL"     , tokenL                           },
                { "imei"       , SystemInfo.deviceUniqueIdentifier}
            };

            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.LoginByTokenLOfficial(ref url, ref header, ref data_json, data);
            Debug.Log($"{url}, {header}, {data_json}");
            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("Official Channel http respond code: "+code);
                        Dictionary<string, object> out_param = new Dictionary<string, object>();
                        out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_FAIL, code, "channel http request failed", null);
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnLoginResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                        if (errCode != (int)OfficialChannelMetaErrCode.Success)
                        {
                            //Debug.Log("Get Offical token failed, meta.errCode: "+errCode+", errCode, errMsg: "+errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnLoginResult(MiniJSON.Json.Serialize(out_param));
                            return;
                        }
                        var dataNode = (Dictionary<string, object>)json_dict["data"];

                        //Debug.Log("Get official token success! "+jsonData);
                        AuthChannelTokenTPF(MiniJSON.Json.Serialize(dataNode)); // 传入渠道拿到的信息
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }

        private void AuthChannelTokenTPF(string channelData)
        {
            var data = new Dictionary<string, object>
            {
                {"extension", channelData  },
                {"extra"    , ""           },
                {"extraFlg" , 4            }
            };

            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.AuthChannelTokenTPF(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("TPF Login http respond code: "+code);
                        Dictionary<string, object> out_param = new Dictionary<string, object>();
                        out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_FAIL, code, "tpf_login http request failed", null);
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnLoginResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                        Dictionary<string, object> out_param;
                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json_dict, out errCode, out errMsg);
                        if (errCode != (int)TPFLoginMetaErrCode.Success)
                        {
                            //Debug.Log("Get tpf_uid failed, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_FAIL, errCode, errMsg, null);
                        }
                        else
                        {
                            var dataNode = (Dictionary<string, object>)json_dict["data"];
                            var channel_data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(channelData);
                            //Debug.Log("Get tpf uid success! " + jsonData);
                            var extra = new Dictionary<string, object>();
                            m_UserId = dataNode["id"] as string;
                            m_Token = dataNode["token"] as string;
                            extra["UserId"] = m_UserId;
                            extra["Token"] = m_Token;
                            extra["ChannelId"] = channelId;
                            if (dataNode.ContainsKey("lawInfo"))
                            {
                                extra["LawInfo"] = Convert.ToString(dataNode["lawInfo"]);
                            }
                            if (channel_data.ContainsKey("ssFlg"))
                            {
                                extra["SSFlg"] = Convert.ToString(channel_data["ssFlg"]);
                            }
                            if (channel_data.ContainsKey("account"))
                            {
                                extra["Account"] = channel_data["account"];
                            }
                            extra["TokenL"] = channel_data["tokenL"];
                            extra["Id"] = channel_data["id"];
                            extra["GuestToken"] = channel_data["token"];
                            extra["Actived"] = Convert.ToString(channel_data["actived"]);
                            extra["RelatePhone"] = channel_data["relatePhone"];

                            out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_LOGIN_SUCCESS, errCode, errMsg, extra);
                        }
                        TPFSdkCallback.Instance.OnLoginResult(MiniJSON.Json.Serialize(out_param));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }



        #endregion

        #region Offical get verification code implement
        private void GetVerificationCodeOfficial(int type, string phoneNum)
        {
            var data = new Dictionary<string, object>
            {
                {"areaId", areaId },
                {"phone", phoneNum },
            };

            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;

            switch (type)
            {
                case (int)OfficalVerifyCodeType.Login:
                    requestAdapter.GetLoginVerificationCodeOfficial(ref url, ref header, ref data_json, data);
                    break;
                case (int)OfficalVerifyCodeType.Register:
                    requestAdapter.GetRegisterVerificationCodeOfficial(ref url, ref header, ref data_json, data);
                    break;
                case (int)OfficalVerifyCodeType.ChangePwd:
                    requestAdapter.GetChangePwdVerificationCodeOfficial(ref url, ref header, ref data_json, data);
                    break;
                case (int)OfficalVerifyCodeType.ForgetPwd:
                    requestAdapter.GetForgetPwdVerificationCodeOfficial(ref url, ref header, ref data_json, data);
                    break;
                case (int)OfficalVerifyCodeType.Bind:
                    requestAdapter.GetBindVerificationCodeOfficial(ref url, ref header, ref data_json, data);
                    break;
                default:
                    Debug.LogError("unknow type: " + type);
                    return;
            }

            TPFHttpUtils.Post(url, header, data_json, (code, outputData) =>
            {
                if (code != 200)
                {
                    Debug.LogError("Official Channel http respond code: "+code);
                    Dictionary<string, object> output_param = new Dictionary<string, object>();
                    output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_VERIFY_CODE_FAIL,code, "http request failed", null);
                    var output_json = MiniJSON.Json.Serialize(output_param);
                    TPFSdkCallback.Instance.OnVerifyCodeResult(output_json);
                    return;
                }
                else
                {
                    var jsonData = Encoding.UTF8.GetString(outputData);
                    var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                    int errCode;
                    string errMsg;
                    ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                    Dictionary<string, object> output_param;
                    if (errCode != (int)OfficialChannelMetaErrCode.Success)
                    {
                        //Debug.Log("Get VerifyCode failed, meta.errCode: "+errCode+", errCode, errMsg: "+errMsg);
                        output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_VERIFY_CODE_FAIL, errCode, errMsg, null);
                    }
                    else
                    {
                        //Debug.Log("Get VerifyCode success, meta.errCode: "+errCode+", errCode, errMsg: "+errMsg);
                        output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_VERIFY_CODE_SUCCESS, errCode, errMsg, null);
                    }
                    var out_json = MiniJSON.Json.Serialize(output_param);
                    //Debug.Log("vc callback param: "+out_json);
                    TPFSdkCallback.Instance.OnVerifyCodeResult(out_json);
                }
            });
        }

        #endregion

        #region forget password implement

        private void ForgetPasswordOfficial(string account, string phoneNum, string newPassword, string msgCode)
        {
            newPassword = MD5Utils.GetMD5(newPassword);
            var data = new Dictionary<string, object>
            {
                { "areaId"        , areaId        },
                { "account"       , account       },
                { "phone"         , phoneNum      },
                { "newPassword"   , newPassword   },
                { "msgCode"       , msgCode       }
            };

            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.ForgetPasswordOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("Official Channel http respond code: " + code);
                        Dictionary<string, object> out_param = new Dictionary<string, object>();
                        out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_PWD_CHANGE_FAIL, code, "http request failed", null);
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnPasswordResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                        if (errCode != (int)OfficialChannelMetaErrCode.Success)
                        {
                            //Debug.Log("Change Password Fail, meta.errCode: "+errCode+" errMsg: "+errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_PWD_CHANGE_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnPasswordResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("Change Password Success!");
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_PWD_CHANGE_SUCCESS, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnPasswordResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }

        private void ModifyPasswordOfficial(string id, string password, string newPassword)
        {
            password = MD5Utils.GetMD5(password);
            newPassword = MD5Utils.GetMD5(newPassword);
            var data = new Dictionary<string, object>
            {
                { "areaId"        , areaId        },
                { "id"            , id            },
                { "password"      , password      },
                { "newPassword"   , newPassword   },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.ModifyPasswordOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("Official Channel http respond code: "+code);
                        Dictionary<string, object> out_param = new Dictionary<string, object>();
                        out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_PWD_CHANGE_FAIL, code, "http request failed", null);
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnPasswordResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                        if (errCode != (int)OfficialChannelMetaErrCode.Success)
                        {
                            //Debug.Log("Change Password Fail, meta.errCode: "+errCode+", errCode, errMsg: "+errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_PWD_CHANGE_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnPasswordResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("Change Password Success!");
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_PWD_CHANGE_SUCCESS, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnPasswordResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }
        #endregion

        #region real name verify

        private void RealNameVerifyOfficial(string id, string tokenL, string idCard, string realName, string cardType, string userId, string token)
        {
            var data = new Dictionary<string, object>
            {
                { "areaId"        , areaId      },
                { "id"            , id          },
                { "tokenL"        , tokenL      },
                { "imei"          , SystemInfo.deviceUniqueIdentifier        },
                { "idCard"        , idCard      },
                { "realName"      , realName    },
                //{ "cardType"      , cardType    }, //1-身份证, 默认为身份证
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.RealNameVerifyOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("Official Channel http respond code: "+code);
                        Dictionary<string, object> out_param = new Dictionary<string, object>();
                        out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_REAL_NAME_FAIL, code, "http request failed", null);
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnRealNameResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                        //Debug.Log("RealNameVerify : "+jsonData);

                        int errCode;
                        string errMsg;
                        ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                        if (errCode != (int)OfficialChannelMetaErrCode.Success)
                        {
                            //Debug.Log("Real name verify fail, meta.errCode: "+errCode+", errCode, errMsg: "+errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_REAL_NAME_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnRealNameResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("Real name verify success!");
                            QueryRealNameVerifyTPF(userId, token, true);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }

        private void QueryRealNameVerifyTPF(string userId, string token, bool isPureQuery = false)
        {
            var data = new Dictionary<string, object>
            {
                { "id"          , userId  },
                { "token"       , token   },
                { "extension"   , "{}"    },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.QueryRealNameVerifyTPF(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("tpf login http respond code: "+code);
                        Dictionary<string, object> out_param;
                        if (isPureQuery)
                        {
                            out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_REAL_NAME_FAIL, code, "http request failed", null);
                        }
                        else
                        {
                            out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_CHECK_VERIFY_FAIL, code, "http request failed", null);
                        }
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnRealNameResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                        //Debug.Log("QueryRealNameVerify : "+jsonData);
                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json_dict, out errCode, out errMsg);
                        if (errCode != (int)TPFLoginMetaErrCode.Success)
                        {
                            Dictionary<string, object> out_param;

                            if (isPureQuery)
                            {
                                out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_REAL_NAME_FAIL, errCode, errMsg, null);
                            }
                            else
                            {
                                out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_CHECK_VERIFY_FAIL, errCode, errMsg, null);
                            }
                            //Debug.Log("Real name verify fail, meta.errCode: "+errCode+", errCode, errMsg: "+errMsg);
                            TPFSdkCallback.Instance.OnRealNameResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("Real name verify success!");
                            var dataNode = (Dictionary<string, object>)json_dict["data"];
                            Dictionary<string, object> out_param;

                            var extra = new Dictionary<string, object>();
                            extra["Status"] = dataNode["status"]; // 1-成年 2-未成年 3-未认证
                            if (dataNode.ContainsKey("realName"))
                            {
                                extra["RealName"] = Convert.ToString(dataNode["realName"]);
                            }
                            else
                            {
                                extra["RealName"] = string.Empty;
                            }
                            extra["IdCard"] = dataNode["idCard"];
                            extra["CardType"] = dataNode["cardType"];
                            extra["LeftTime"] = dataNode["overS"];
                            if (isPureQuery)
                            {
                                // if is a internal pure query for sync with tpf server, callback with a TPFCode.TPFCODE_REAL_NAME_SUCCESS
                                out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_REAL_NAME_SUCCESS, errCode, errMsg, extra);
                            }
                            else
                            {
                                out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_CHECK_VERIFY_SUCCESS, errCode, errMsg, extra);
                            }
                            TPFSdkCallback.Instance.OnRealNameResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }

        #endregion

        #region bind account

        private void QueryUserBindOfficial(string id, string token)
        {
            var data = new Dictionary<string, object>
            {
                { "areaId"       , areaId     },
                { "id"           , id         },
                { "token"        , token      },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.QueryUserBindOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("Official Channel http respond code: "+code);
                        Dictionary<string, object> out_param = new Dictionary<string, object>();
                        out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_QUERY_FAIL, code, "http request failed", null);
                        var out_json = MiniJSON.Json.Serialize(out_param);
                        TPFSdkCallback.Instance.OnBindAccountResult(out_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        //Debug.Log("query binding Data: "+jsonData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                        int errCode;
                        string errMsg;
                        ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                        Dictionary<string, object> out_param = null;
                        if (errCode != (int)OfficialChannelMetaErrCode.Success)
                        {
                            //Debug.Log("Query user bind account fail, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_QUERY_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnBindAccountResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("Query user bind account success!");
                            var body = (Dictionary<string, object>)json_dict["data"];
                            //Debug.Log("query binding body: {MiniJSON.Json.Serialize(body)}");
                            int binding_state = System.Convert.ToInt32(body["phonce"]);
                            if (binding_state == (int)QueryBindingState.HasBindedPhone)
                            {
                                out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_BINDED, errCode, errMsg, null);
                            }
                            else if (binding_state == (int)QueryBindingState.HasNoBindedPhone)
                            {
                                out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_NOT_BIND, errCode, errMsg, null);
                            }
                            else
                            {
                                Debug.LogError("unknow query binding code: {binding_state}");
                                out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_QUERY_FAIL, errCode, errMsg, null);
                            }
                            TPFSdkCallback.Instance.OnBindAccountResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }

        private void TouristBindPhoneOfficial(string phone, string msgcode, string id, string tokenL, string password)
        {
            var data = new Dictionary<string, object>
            {
                { "areaId"       , areaId },
                { "id"           , id     },
                { "tokenL"       , tokenL },
                { "imei"         , SystemInfo.deviceUniqueIdentifier },
                { "password"     ,password},
                { "phone"        , phone  },
                { "msgCode"      , msgcode},
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.TouristBindPhoneOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
               (code, outputData) =>
               {
                   if (code != 200)
                   {
                       Debug.LogError("Official Channel http respond code: "+code);
                       Dictionary<string, object> output_param = new Dictionary<string, object>();
                       output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_FAIL, code, "http request failed", null);
                       var output_json = MiniJSON.Json.Serialize(output_param);
                       TPFSdkCallback.Instance.OnBindAccountResult(output_json);
                       return;
                   }
                   try
                   {
                       var jsonData = Encoding.UTF8.GetString(outputData);
                       var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                       int errCode;
                       string errMsg;
                       ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                       if (errCode != (int)OfficialChannelMetaErrCode.Success)
                       {
                           Debug.Log("bind account fail, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                           var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_FAIL, errCode, errMsg, null);
                           TPFSdkCallback.Instance.OnBindAccountResult(MiniJSON.Json.Serialize(out_param));
                       }
                       else
                       {
                           Debug.Log("bind account success!");
                           var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_SUCCESS, errCode, errMsg, null);
                           TPFSdkCallback.Instance.OnBindAccountResult(MiniJSON.Json.Serialize(out_param));
                       }
                   }
                   catch (Exception e)
                   {
                       Debug.LogError(e);
                   }
               }
           );
        }

        private void TouristBindNewAccountOfficial(string account, string password, string id, string tokenL)
        {
            password = MD5Utils.GetMD5(password);
            var data = new Dictionary<string, object>
            {
                { "areaId"       , areaId },
                { "id"           , id     },
                { "tokenL"       , tokenL },
                { "imei"         , SystemInfo.deviceUniqueIdentifier },

                { "newAccount"        , account },
                { "password"          , password},
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.TouristBindNewAccountOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
               (code, outputData) =>
               {
                   if (code != 200)
                   {
                       Debug.LogError("Official Channel http respond code: "+code);
                       Dictionary<string, object> output_param = new Dictionary<string, object>();
                       output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_FAIL, code, "http request failed", null);
                       var output_json = MiniJSON.Json.Serialize(output_param);
                       TPFSdkCallback.Instance.OnBindAccountResult(output_json);
                       return;
                   }
                   try
                   {
                       var jsonData = Encoding.UTF8.GetString(outputData);
                       var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                       int errCode;
                       string errMsg;
                       ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                       if (errCode != (int)OfficialChannelMetaErrCode.Success)
                       {
                           Debug.Log("bind account fail, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                           var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_FAIL, errCode, errMsg, null);
                           TPFSdkCallback.Instance.OnBindAccountResult(MiniJSON.Json.Serialize(out_param));
                       }
                       else
                       {
                           //Debug.Log("bind account success!");
                           var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_ACCOUNT_SUCCESS, errCode, errMsg, null);
                           TPFSdkCallback.Instance.OnBindAccountResult(MiniJSON.Json.Serialize(out_param));
                       }
                   }
                   catch (Exception e)
                   {
                       Debug.LogError(e);
                   }
               }
           );
        }
        #endregion

        #region tpf bind 

        internal override bool QueryPhoneNum(TPFSdkInfo info)
        {
            string id = info.GetData("id");
            string token = info.GetData("token");

            var data = new Dictionary<string, object>
            {
                { "id"           , id         },
                { "token"        , token      },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.QueryPhoneNumTPF(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("tpf login http respond code: "+code);
                        Dictionary<string, object> output_param = new Dictionary<string, object>();
                        output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_QUERY_PHONE_FAIL, code, "http request failed", null);
                        var output_json = MiniJSON.Json.Serialize(output_param);
                        TPFSdkCallback.Instance.OnQueryPhoneResult(output_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);
                        var dataNode = (Dictionary<string, object>)json_dict["data"];
                        
                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json_dict, out errCode, out errMsg);
                        //Debug.Log("On TPF login query phone number rsp: " + jsonData);
                        if (errCode != (int)TPFLoginMetaErrCode.Success)
                        {
                            //Debug.Log("Query tpf phone num fail, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_QUERY_PHONE_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnQueryPhoneResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("Query tpf phone num success! ");
                            var extra = new Dictionary<string, object>();
                            extra["id"] = (string)dataNode["id"];
                            extra["phoneNum"] = (string)dataNode["phoneNum"];
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_QUERY_PHONE_SUCCESS, errCode, errMsg, extra);
                            TPFSdkCallback.Instance.OnQueryPhoneResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
            return true;
        }

        internal override bool FetchPhoneCode(TPFSdkInfo info)
        {
            string id = info.GetData("id");
            string token = info.GetData("token");
            string phoneNum = info.GetData("phoneNum");

            var data = new Dictionary<string, object>
            {
                { "id"           , id         },
                { "token"        , token      },
                { "phoneNum"     , phoneNum   },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.FetchPhoneCodeTPF(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("tpf login http respond code: "+code);
                        Dictionary<string, object> output_param = new Dictionary<string, object>();
                        output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_PHONE_VERIFT_CODE_FAIL, code, "http request failed", null);
                        var output_json = MiniJSON.Json.Serialize(output_param);
                        TPFSdkCallback.Instance.OnVerifyCodeResult(output_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json_dict, out errCode, out errMsg);
                        //Debug.Log("On TPF login fetch verify code rsp: " + jsonData);
                        if (errCode != (int)TPFLoginMetaErrCode.Success)
                        {
                            //Debug.Log("Query tpf phone num fail, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_PHONE_VERIFT_CODE_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnVerifyCodeResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("Query tpf fetch verify code success! ");
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_PHONE_VERIFT_CODE_SUCCESS, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnVerifyCodeResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
            return true;
        }

        internal override bool CheckPhoneCode(TPFSdkInfo info)
        {
            string id = info.GetData("id");
            string token = info.GetData("token");
            string phoneNum = info.GetData("phoneNum");
            string msgCode = info.GetData("msgCode");

            var data = new Dictionary<string, object>
            {
                { "id"           , id         },
                { "token"        , token      },
                { "phoneNum"     , phoneNum   },
                { "msgCode"      , msgCode    },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.CheckPhoneCodeTPF(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("tpf login http respond code: "+code);
                        Dictionary<string, object> output_param = new Dictionary<string, object>();
                        output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_VERIFY_CODE_CHECK_FAIL, code, "http request failed", null);
                        var output_json = MiniJSON.Json.Serialize(output_param);
                        TPFSdkCallback.Instance.OnVerifyCodeCheckResult(output_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json_dict, out errCode, out errMsg);
                        //Debug.Log("On TPF login check verify code rsp: " + jsonData);
                        if (errCode != (int)TPFLoginMetaErrCode.Success)
                        {
                            //Debug.Log("CheckTPFVerifyCode, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_VERIFY_CODE_CHECK_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnVerifyCodeCheckResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("CheckTPFVerifyCode success! ");
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_VERIFY_CODE_CHECK_SUCCESS, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnVerifyCodeCheckResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
            return true;
        }

        internal override bool BindPhone(TPFSdkInfo info)
        {
            string id = info.GetData("id");
            string token = info.GetData("token");
            string phoneNum = info.GetData("phoneNum");
            string msgCode = info.GetData("msgCode");

            var data = new Dictionary<string, object>
            {
                { "id"           , id         },
                { "token"        , token      },
                { "phoneNum"     , phoneNum   },
                { "msgCode"      , msgCode    },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.BindPhoneTPF(ref url, ref header, ref data_json, data);


            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("tpf login http respond code: "+code);
                        Dictionary<string, object> output_param = new Dictionary<string, object>();
                        output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_PHONE_FAIL, code, "http request failed", null);
                        var output_json = MiniJSON.Json.Serialize(output_param);
                        TPFSdkCallback.Instance.OnBindPhoneResult(output_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json_dict, out errCode, out errMsg);
                        //Debug.Log("BindTPFPhoneNum rsp: " + jsonData);
                        if (errCode != (int)TPFLoginMetaErrCode.Success)
                        {
                            //Debug.Log("BindTPFPhoneNum, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_PHONE_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnBindPhoneResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("BindTPFPhoneNum success! ");
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_BIND_PHONE_SUCCESS, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnBindPhoneResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
            return true;
        }
        #endregion

        #region Confirm User Protocol

        private void AgreeUserProtocolTPF(string userId, string token)
        {
            var data = new Dictionary<string, object>
            {
                { "id"           , userId         },
                { "token"        , token      },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.AgreeUserProtocolTPF(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json,
                (code, outputData) =>
                {
                    if (code != 200)
                    {
                        Debug.LogError("tpf login http respond code: "+code);
                        Dictionary<string, object> output_param = new Dictionary<string, object>();
                        output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_USER_PROTOCOL_FAIL, code, "http request failed", null);
                        var output_json = MiniJSON.Json.Serialize(output_param);
                        TPFSdkCallback.Instance.OnProtocolResult(output_json);
                        return;
                    }
                    try
                    {
                        var jsonData = Encoding.UTF8.GetString(outputData);
                        var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                        int errCode;
                        string errMsg;
                        TPFHttpUtils.GetCommonHttpRspMeta(json_dict, out errCode, out errMsg);
                        //Debug.Log("AgreeUserProtocol rsp: " + jsonData);
                        if (errCode != (int)TPFLoginMetaErrCode.Success)
                        {
                            //Debug.Log("AgreeUserProtocol, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_USER_PROTOCOL_FAIL, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnProtocolResult(MiniJSON.Json.Serialize(out_param));
                        }
                        else
                        {
                            //Debug.Log("AgreeUserProtocol success! ");
                            var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_USER_PROTOCOL_SUCCESS, errCode, errMsg, null);
                            TPFSdkCallback.Instance.OnProtocolResult(MiniJSON.Json.Serialize(out_param));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            );
        }

        #endregion

        #region giftCard
        private void QueryGiftCardOfficial(string id, string token)
        {
            var data = new Dictionary<string, object>
            {
                { "areaId"       , areaId     },
                { "id"           , id         },
                { "token"        , token      },
            };
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.QueryGiftCardOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json, (code, outputData) =>
            {
                if (code != 200)
                {
                    Debug.LogError("httprespond code: " + code);
                    Dictionary<string, object> output_param = new Dictionary<string, object>();
                    output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_QUERY_GIFTCODE_FAIL, code, "http request failed", null);
                    var output_json = MiniJSON.Json.Serialize(output_param);
                    TPFSdkCallback.Instance.OnGiftCodeResult(output_json);
                    return;
                }
                try
                {
                    var jsonData = Encoding.UTF8.GetString(outputData);
                    var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                    int errCode;
                    string errMsg;
                    ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                    //Debug.Log("QueryGiftCard rsp: " + jsonData);
                    //Debug.Log("QueryGiftCard, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                    if (errCode != (int)TPFLoginMetaErrCode.Success)
                    {
                        var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_QUERY_GIFTCODE_FAIL, errCode, errMsg, null);
                        TPFSdkCallback.Instance.OnGiftCodeResult(MiniJSON.Json.Serialize(out_param));
                    }
                    else
                    {
                        //Debug.Log("QueryGiftCard success! ");
                        //todo extra: gift code
                        Dictionary<string, object> extra = new Dictionary<string, object>();
                        Dictionary<string, object> dataNode = (Dictionary<string, object>)json_dict["data"];
                        extra["card"] = dataNode["card"];
                        var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_QUERY_GIFTCODE_SUCCESS, errCode, errMsg, extra);
                        TPFSdkCallback.Instance.OnGiftCodeResult(MiniJSON.Json.Serialize(out_param));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });

        }
        private void PostGiftCardOfficial(string id, string token, string card, int cardType, string playerId , string serverId )
        {
            var data = new Dictionary<string, object>
            {
                { "areaId"       , areaId     },
                { "id"           , id         },
                { "token"        , token      },
                { "card"         , card       },
                { "cardType"     , cardType   },
            };
            if (playerId != null && playerId != "")
            {
                data["playerId"] = playerId;
            }
            if (serverId != null && serverId != "")
            {
                data["serverId"] = serverId;
            }
            string url = null;
            string data_json = null;
            Dictionary<string, string> header = null;
            requestAdapter.PostGiftCardOfficial(ref url, ref header, ref data_json, data);

            TPFHttpUtils.Post(url, header, data_json, (code, outputData) =>
            {
                if (code != 200)
                {
                    Debug.LogError("PostGiftCard http respond code: " + code);
                    Dictionary<string, object> output_param = new Dictionary<string, object>();
                    output_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_POST_GIFTCODE_FAIL, code, "http request failed", null);
                    var output_json = MiniJSON.Json.Serialize(output_param);
                    TPFSdkCallback.Instance.OnGiftCodeResult(output_json);
                    return;
                }
                try
                {
                    var jsonData = Encoding.UTF8.GetString(outputData);
                    var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonData);

                    int errCode;
                    string errMsg;
                    ParamUtils.GetOfficialHttpRspMeta(json_dict, out errCode, out errMsg);
                    //Debug.Log("PostGiftCard rsp: " + jsonData);
                    if (errCode != (int)TPFLoginMetaErrCode.Success)
                    {
                        //Debug.Log("PostGiftCard, meta.errCode: " + errCode + ", errCode, errMsg: " + errMsg);
                        var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_POST_GIFTCODE_FAIL, errCode, errMsg, null);
                        TPFSdkCallback.Instance.OnGiftCodeResult(MiniJSON.Json.Serialize(out_param));
                    }
                    else
                    {
                        //Debug.Log("PostGiftCard success! ");
                        var out_param = ParamUtils.WarpTPFCallbackParam(TPFCode.TPFCODE_POST_GIFTCODE_SUCCESS, errCode, errMsg, null);
                        TPFSdkCallback.Instance.OnGiftCodeResult(MiniJSON.Json.Serialize(out_param));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
        }
        #endregion


        internal override string GetSubmitInfo()
        {
            return _submitInfo;
        }

        internal override string GetUserId()
        {
            return m_UserId;
        }

        internal override string GetToken()
        {
            return m_Token;
        }

        internal override void SendGameRequest(TPFSdkInfo info)
        {
            int sessionId = info.GetInt(TPFParamKey.SESSION_ID_KEY);
            string url = info.GetData(TPFParamKey.URL);
            string headerJson = info.GetData(TPFParamKey.HEADER);
            string body = info.GetData(TPFParamKey.BODY);
            //Debug.Log($"url = {url}, header = {headerJson}, body = {body}");

            var headerObj = (Dictionary<string, object>)MiniJSON.Json.Deserialize(headerJson);
            var headers = headerObj.ToDictionary(p => p.Key, p => p.Value.ToString());
            TPFHttpUtils.Post(url, headers, body, (code, rsp) =>
            {
                string rspData = Encoding.UTF8.GetString(rsp);
                Dictionary<string, string> data = new Dictionary<string, string>();
                data[TPFParamKey.HTTP_RESPOND_CODE] = code.ToString();
                data[TPFParamKey.SESSION_ID_KEY] = sessionId.ToString();
                data[TPFParamKey.JSON_DATA_KEY] = rspData;
                var jsonData = MiniJSON.Json.Serialize(data);
                TPFSdkCallback.Instance.OnGameResult(jsonData);
            });
        }

    }

}

#endif