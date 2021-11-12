using System.Collections;
using System.Collections.Generic;
using TPFSDK.Utils;

namespace TPFSDK
{
    // 中台登录迁到了 proxy 但是不够稳定未能启用，所以登录相关的业务接口都使用 adapter 来兼容旧方式，
    // 后面需要启用 proxy 的登录只需在配置中将 isUseNormalUrl 字段删除即可。
    internal abstract class ITPFHttpRequestAdapter
    {
        public ITPFHttpRequestAdapter(ITPFSdk sdk) { m_SDK = sdk; }
        protected ITPFSdk m_SDK;
        
        // request to official channel
        public abstract void RegisterAsUserNameOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void RegisterAsPhoneOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void RegisterAsTouristOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void LoginOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void LoginByTokenLOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void GetLoginVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void GetRegisterVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void GetChangePwdVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void GetForgetPwdVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void GetBindVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void ForgetPasswordOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void ModifyPasswordOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void RealNameVerifyOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void QueryUserBindOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void TouristBindPhoneOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void TouristBindNewAccountOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void QueryGiftCardOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void PostGiftCardOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);

        // request to tpf
        public abstract void AuthChannelTokenTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void QueryRealNameVerifyTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void AgreeUserProtocolTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void QueryPhoneNumTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void FetchPhoneCodeTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void CheckPhoneCodeTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void BindPhoneTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);

        // TODO: qrcode related
        public abstract void GetQRCodeImageTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void DoQueryStatusTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);
        public abstract void UpdateLoginStateTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict);

    }


    internal class ProxyAdapter : ITPFHttpRequestAdapter
    {
        #region proxy config
        private const string officialProvideID                        = "700021";
        private const string officialRegistByAccountMsgID             = "8001";
        private const string officialRegistByPhoneMsgID               = "8002";
        private const string officialRegistAsTouristMsgID             = "8004";
        private const string officialLoginMsgID                       = "8021";
        private const string officialLoginByTokenLMsgID               = "8023";
        private const string officialForgetPasswordMsgID              = "8048";
        private const string officialModifyPasswordMsgID              = "8047";
        private const string officialGetLoginVerifyCodeMsgID          = "8061";
        private const string officialGetRegistVerifyCodeMsgID         = "8062";
        private const string officialGetBindVerifyCodeMsgID           = "8063";
        private const string officialGetChangePasswordVerifyCodeMsgID = "8064";
        private const string officialGetForgetPasswordVerifyCodeMsgID = "8065";
        private const string officialRealNameVerifyMsgID              = "8007";
        private const string officialQueryAccountBindMsgID            = "8052";
        private const string officialTouristBindPhoneMsgID            = "8044";
        private const string officialTouristBindAccountMsgID          = "8043";
        private const string officialQueryGiftCardMsgID               = "8066";
        private const string officialPostGiftCodeMsgID                = "8067";

        private const string tpfLoginProvideID                        = "700022";
        private const string tpfLoginAuthMsgID                        = "8500";
        private const string tpfLoginQueryPhoneNumMsgID               = "8501";
        private const string tpfLoginFetchPhoneVerifyCodeMsgID        = "8502";
        private const string tpfLoginCheckPhoneVerifyCodeMsgID        = "8503";
        private const string tpfLoginBindPhoneNumMsgID                = "8504";
        private const string tpfLoginQueryRealNameMsgID               = "8507";
        private const string tpfLoginGetQRCodeMsgID                   = "8508";
        private const string tpfLoginQueryQRCodeStatusMsgID           = "8509";
        private const string tpfLoginUpdateQRCodeStatusMsgID          = "8510";
        private const string tpfLoginAgreeLawInfoMsgID                = "8511";
        #endregion

        public ProxyAdapter(ITPFSdk sdk): base(sdk) { }

        public override void AgreeUserProtocolTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginAgreeLawInfoMsgID);
        }

        public override void AuthChannelTokenTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginAuthMsgID);
        }

        public override void BindPhoneTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginBindPhoneNumMsgID);
        }

        public override void CheckPhoneCodeTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginCheckPhoneVerifyCodeMsgID);
        }

        public override void FetchPhoneCodeTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginFetchPhoneVerifyCodeMsgID);
        }

        public override void ForgetPasswordOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialForgetPasswordMsgID);
        }

        public override void GetLoginVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialGetLoginVerifyCodeMsgID);
        }

        public override void GetRegisterVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialGetRegistVerifyCodeMsgID);
        }

        public override void GetChangePwdVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialGetChangePasswordVerifyCodeMsgID);
        }

        public override void GetForgetPwdVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialGetForgetPasswordVerifyCodeMsgID);
        }

        public override void GetBindVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialGetBindVerifyCodeMsgID);
        }

        public override void LoginByTokenLOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialLoginByTokenLMsgID);
        }

        public override void LoginOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialLoginMsgID);
        }

        public override void ModifyPasswordOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialModifyPasswordMsgID);
        }

        public override void PostGiftCardOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialPostGiftCodeMsgID);
        }

        public override void QueryGiftCardOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialQueryGiftCardMsgID);
        }

        public override void QueryPhoneNumTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginQueryPhoneNumMsgID);
        }

        public override void QueryRealNameVerifyTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginQueryRealNameMsgID);
        }

        public override void QueryUserBindOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialQueryAccountBindMsgID);
        }

        public override void RealNameVerifyOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialRealNameVerifyMsgID);
        }

        public override void RegisterAsPhoneOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialRegistByPhoneMsgID);
        }

        public override void RegisterAsTouristOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialRegistAsTouristMsgID);
        }

        public override void RegisterAsUserNameOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialRegistByAccountMsgID);
        }

        public override void TouristBindNewAccountOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialTouristBindAccountMsgID);
        }

        public override void TouristBindPhoneOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, officialProvideID, officialTouristBindPhoneMsgID);
        }

        public override void GetQRCodeImageTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginGetQRCodeMsgID,"-1");// 此时并没有确定的渠道，channelId传-1
        }

        public override void DoQueryStatusTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginQueryQRCodeStatusMsgID, "-1");// 此时并没有确定的渠道，channelId传-1
        }

        public override void UpdateLoginStateTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, tpfLoginProvideID, tpfLoginUpdateQRCodeStatusMsgID);
        }

        private void InternalProxyRequestSetup(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict, string provideId, string msgId)
        {
            InternalProxyRequestSetup(ref url, ref header, ref body, body_dict, provideId, msgId, m_SDK.channelId);
        }

        private void InternalProxyRequestSetup(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict, string provideId, string msgId, string channelId)
        {
            url = m_SDK.networkConfig.tpfProxyUrl;
            body = MiniJSON.Json.Serialize(body_dict);
            header = TPFHttpUtils.GetTPFHttpHeader(m_SDK.appId, m_SDK.appKey, provideId, m_SDK.appSecret, msgId, body, channelId);
        }
        public static void ProxySetUp(out string url, out Dictionary<string, string> header, out string body, Dictionary<string, object> body_dict, string provideId, string msgId)
        {
            ITPFSdk sdkInst = ITPFSdk.Instance;
            url = sdkInst.networkConfig.tpfProxyUrl;
            body = MiniJSON.Json.Serialize(body_dict);
            header = TPFHttpUtils.GetTPFHttpHeader(sdkInst.appId, sdkInst.appKey, provideId, sdkInst.appSecret, msgId, body, sdkInst.channelId);
        }
    }

    internal class NormalUrlAdapter : ITPFHttpRequestAdapter
    {
        #region request url
        private const string official_login_url                  = "/official/authentication/login";
        private const string official_login_by_token_url         = "/official/authentication/check_token";
        private const string official_login_by_tokenL_url        = "/official/authentication/check_tokenL";                                                                 
        private const string official_register_username_url      = "/official/register/register";
        private const string official_register_phone_url         = "/official/register/phone";
        private const string official_register_tourist_url       = "/official/register/tourist";
        private const string official_forgetPassword_url         = "/official/additional/forget_pass";
        private const string official_modifyPassword_url         = "/official/additional/change_pass";
        private const string official_getLoginVerifyCode_url     = "/official/auxiliary/fetch_phone/login";
        private const string official_getRegisterVerifyCode_url  = "/official/auxiliary/fetch_phone/register";
        private const string official_getBindVerifyCode_url      = "/official/auxiliary/fetch_phone/bind";
        private const string official_getChangePwdVerifyCode_url = "/official/auxiliary/fetch_phone/change";
        private const string official_getForgetPwdVerifyCode_url = "/official/auxiliary/fetch_phone/forget_pass";
        private const string official_realNameVerify_url         = "/official/register/identityVerification";
        private const string official_queryUserBind_url          = "/official/additional/q_info_bt";
        private const string official_touristBindAccount_url     = "/official/additional/binding/tourist/account";
        private const string official_touristBindPhone_url       = "/official/additional/binding/tourist/phone";
        private const string official_queryGiftCode_url          = "/official/auxiliary/qActivecode";
        private const string official_useGiftCode_url            = "/official/auxiliary/useActiveCode";
                                                                 
        private const string tpfLogin_channelAuth_url            = "/auth/channelAuth";
        private const string tpfLogin_agreeLawInfo_url           = "/auth/agreeLawInfo";
        private const string tpfLogin_bindPhone_url              = "/fn/collect";
        private const string tpfLogin_checkPhoneCode_url         = "/fn/codeCheck";
        private const string tpfLogin_fetchPhoneCode_url         = "/fn/fetchPhone";
        private const string tpfLogin_queryPhoneNum_url          = "/fn/phoneNum";
        private const string tpfLogin_queryRealNameVerify_url    = "/channel/identityS";
        private const string tpfLogin_updateQrCodeStatus_url     = "/code/updateQrCodeStatus";
        private const string tpfLogin_doQrQueryStatus_url        = "/code/qrCodeStatus";
        private const string tpfLogin_getQrCodeImage_url         = "/code/qrCodeL";

        #endregion

        public NormalUrlAdapter(ITPFSdk sdk) : base(sdk) { }

        public override void AgreeUserProtocolTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(appId={appId}channelId={channelId}id={id}token={token}{appKey})
            string sign = string.Format("appId={0}channelId={1}id={2}token={3}{4}", m_SDK.appId, m_SDK.channelId, body_dict["id"], body_dict["token"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_agreeLawInfo_url, sign);
        }

        public override void AuthChannelTokenTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //baseStr:  appId={appId}channelId={channelId}{extension}{appKey}
            string sign = string.Format("appId={0}channelId={1}{2}{3}", m_SDK.appId, m_SDK.channelId, body_dict["extension"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_channelAuth_url, sign);
        }

        public override void BindPhoneTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //baseStr:  appId={appId}channelId={channelId}id={id}{phoneNum}{appKey}
            string sign = string.Format("appId={0}channelId={1}id={2}{3}{4}", m_SDK.appId, m_SDK.channelId, body_dict["id"], body_dict["phoneNum"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_bindPhone_url, sign);
        }

        public override void CheckPhoneCodeTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            // baseStr:  appId={appId}channelId={channelId}id={id}{phoneNum}{appKey}
            string sign = string.Format("appId={0}channelId={1}id={2}{3}{4}", m_SDK.appId, m_SDK.channelId, body_dict["id"], body_dict["phoneNum"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_checkPhoneCode_url, sign);
        }

        public override void FetchPhoneCodeTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            // baseStr:  appId={appId}channelId={channelId}id={id}{phoneNum}{appKey}
            string sign = string.Format("appId={0}channelId={1}id={2}{3}{4}", m_SDK.appId, m_SDK.channelId, body_dict["id"], body_dict["phoneNum"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_fetchPhoneCode_url, sign);
        }

        public override void ForgetPasswordOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}account={account}newPassword={newPassword}{areakey})
            string sign = string.Format("areaId={0}account={1}newPassword={2}{3}", m_SDK.areaId, body_dict["account"], body_dict["newPassword"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_forgetPassword_url, sign);
        }

        public override void GetBindVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}phone={phone}{areakey})
            string sign = string.Format("areaId={0}phone={1}{2}", m_SDK.areaId, body_dict["phone"], m_SDK.loginConfig.loginAreaKey);
            InternalGetRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_getBindVerifyCode_url, sign);
        }

        public override void GetChangePwdVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}phone={phone}{areakey})
            string sign = string.Format("areaId={0}phone={1}{2}", m_SDK.areaId, body_dict["phone"], m_SDK.loginConfig.loginAreaKey);
            InternalGetRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_getChangePwdVerifyCode_url, sign);
        }

        public override void GetForgetPwdVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}phone={phone}{areakey})
            string sign = string.Format("areaId={0}phone={1}{2}", m_SDK.areaId, body_dict["phone"], m_SDK.loginConfig.loginAreaKey);
            InternalGetRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_getForgetPwdVerifyCode_url, sign);
        }

        public override void GetLoginVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}phone={phone}{areakey})
            string sign = string.Format("areaId={0}phone={1}{2}", m_SDK.areaId, body_dict["phone"], m_SDK.loginConfig.loginAreaKey);
            InternalGetRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_getLoginVerifyCode_url, sign);
        }

        public override void GetRegisterVerificationCodeOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}phone={phone}{areakey})
            string sign = string.Format("areaId={0}phone={1}{2}", m_SDK.areaId, body_dict["phone"], m_SDK.loginConfig.loginAreaKey);
            InternalGetRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_getRegisterVerifyCode_url, sign);
        }

        public override void LoginByTokenLOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}tokenL={tokenL}{areaKey})
            string sign = string.Format("areaId={0}tokenL={1}{2}", m_SDK.areaId, body_dict["tokenL"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_login_by_tokenL_url, sign);
        }

        public override void LoginOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}account={account}password={password}{areakey})
            string sign = string.Format("areaId={0}account={1}password={2}{3}", m_SDK.areaId, body_dict["account"], body_dict["password"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_login_url, sign);
        }

        public override void ModifyPasswordOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}id={account}newPassword={newPassword}{areakey})
            string sign = string.Format("areaId={0}id={1}newPassword={2}{3}", m_SDK.areaId, body_dict["id"], body_dict["newPassword"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_modifyPassword_url, sign);
        }

        public override void PostGiftCardOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}id={id}card={card}{areakey})
            string sign = string.Format("areaId={0}id={1}card={2}{3}", m_SDK.areaId, body_dict["id"], body_dict["card"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_useGiftCode_url, sign);
        }

        public override void QueryGiftCardOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5("areaId={areaId}id={id}{areaKey}")
            string sign = string.Format("areaId={0}id={1}{2}", m_SDK.areaId, body_dict["id"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_queryGiftCode_url, sign);
        }

        public override void QueryPhoneNumTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            // baseStr:  appId={appId}channelId={channelId}id={id}{appKey}
            string sign = string.Format("appId={0}channelId={1}id={2}{3}", m_SDK.appId, m_SDK.channelId, body_dict["id"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_queryPhoneNum_url, sign);
        }

        public override void QueryRealNameVerifyTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            // baseStr:  sign = md5(appId={appId}channelId={channelId}{extension}{appKey})
            string sign = string.Format("appId={0}channelId={1}{2}{3}", m_SDK.appId, m_SDK.channelId, body_dict["extension"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_queryRealNameVerify_url, sign);
        }

        public override void QueryUserBindOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}id={id}token={token}{appkey})
            string sign = string.Format("areaId={0}id={1}token={2}{3}", m_SDK.areaId, body_dict["id"], body_dict["token"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_queryUserBind_url, sign);
        }

        public override void RealNameVerifyOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}id={id}idCard={idCard}realName={realName}{areaKey})
            string sign = string.Format("areaId={0}id={1}idCard={2}realName={3}{4}", m_SDK.areaId, body_dict["id"], body_dict["idCard"], body_dict["realName"],m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_realNameVerify_url, sign);
        }

        public override void RegisterAsPhoneOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}phoneNum={phoneNum}{areakey})
            string sign = string.Format("areaId={0}sphoneNum={1}{2}", m_SDK.areaId, body_dict["phoneNum"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_register_phone_url, sign);
        }

        public override void RegisterAsTouristOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}account={account}{areakey})
            string sign = string.Format("areaId={0}account={1}{2}", m_SDK.areaId, body_dict["account"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_register_tourist_url, sign);
        }

        public override void RegisterAsUserNameOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}account={account}{areakey})
            string sign = string.Format("areaId={0}account={1}{2}", m_SDK.areaId, body_dict["account"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_register_username_url, sign);
        }

        public override void TouristBindNewAccountOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}tokenL={tokenL}{areakey})
            string sign = string.Format("areaId={0}tokenL={1}{2}", m_SDK.areaId, body_dict["tokenL"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_touristBindAccount_url, sign);
        }

        public override void TouristBindPhoneOfficial(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //md5(areaId={areaId}tokenL={tokenL}{areakey})
            string sign = string.Format("areaId={0}tokenL={1}{2}", m_SDK.areaId, body_dict["tokenL"], m_SDK.loginConfig.loginAreaKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.officialLoginHost, official_touristBindPhone_url, sign);
        }


        public override void DoQueryStatusTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            // baseStr:  appId={appId}qrId={qrId}deviceId={deviceId}{clientTime}{appKey}
            string sign = string.Format("appId={0}qrId={1}deviceId={2}{3}{4}", 
                m_SDK.appId, body_dict["qrId"], body_dict["deviceId"], body_dict["clientTime"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_doQrQueryStatus_url, sign);
        }

        public override void GetQRCodeImageTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            // baseStr:  appId={appId}clientExtra={clientExtra}deviceId={deviceId}{clientTime}{appKey}
            string sign = string.Format("appId={0}clientExtra={1}deviceId={2}{3}{4}",
                m_SDK.appId, body_dict["clientExtra"], body_dict["deviceId"], body_dict["clientTime"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_getQrCodeImage_url, sign);
        }

        public override void UpdateLoginStateTPF(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict)
        {
            //  baseStr: appId={appId}channelId={channelId}id={id}token={token}qrId={qrId}deviceId={deviceId}clientExtra={clientExtra}{clientTime}{appKey}
            string sign = string.Format("appId={0}channelId={1}id={2}token={3}qrId={4}deviceId={5}clientExtra={6}{7}{8}",
                m_SDK.appId, m_SDK.channelId, body_dict["id"], body_dict["token"], body_dict["qrId"], body_dict["deviceId"], body_dict["clientExtra"], body_dict["clientTime"], m_SDK.loginConfig.loginAppKey);
            InternalPostRequestSetup(ref url, ref header, ref body, body_dict, m_SDK.networkConfig.tpfLoginHost, tpfLogin_updateQrCodeStatus_url, sign);
        }

        
        private void InternalGetRequestSetup(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict, string host, string request_url, string sign)
        {
            header = TPFHttpUtils.GetCommonHttpHeaders(m_SDK.appId, m_SDK.channelId);
            body = MiniJSON.Json.Serialize(body_dict);
            url = host + request_url + "?";
            foreach(var entry in body_dict)
            {
                url = url + entry.Key + "=" + entry.Value.ToString() + "&";
            }
            sign = MD5Utils.GetMD5(sign);
            body_dict["sign"] = sign;
            url = url + "sign=" + sign;
        }

        private void InternalPostRequestSetup(ref string url, ref Dictionary<string, string> header, ref string body, Dictionary<string, object> body_dict, string host, string request_url, string sign)
        {
            url = host + request_url;
            header = TPFHttpUtils.GetCommonHttpHeaders(m_SDK.appId, m_SDK.channelId);
            sign = MD5Utils.GetMD5(sign);
            body_dict["sign"] = sign;
            body = MiniJSON.Json.Serialize(body_dict);
        }

        
    }

}