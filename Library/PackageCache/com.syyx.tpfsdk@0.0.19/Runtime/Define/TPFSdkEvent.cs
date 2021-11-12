/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的事件
** use: demo
** modify:             
*******************************************************************/
using System;


namespace TPFSDK
{

    /// <summary>
    /// SDK发过来的事件回馈///
    /// </summary>
    public enum TPFSdkEventType
    {
        EVENT_TYPE_INIT = 0,                       //初始化
        EVENT_TYPE_VERIFYCODE,                 //验证码
        EVENT_TYPE_REGISTER,                   //注册
        EVENT_TYPE_LOGIN,                      //登录      
        EVENT_TYPE_LOGOUT,                     //登录出
        EVENT_TYPE_PASSWORD,                   //密码发生变化回调
        EVENT_TYPE_REALNAME,                   //查询实名认证回调
        EVENT_TYPE_BIND_ACCOUNT,               //账号绑定回调

        EVENT_TYPE_QUERY_PHONE,                //查询手机号回调
        EVENT_TYPE_VERIFY_CODE_CHECK,          //手机验证码校验回调
        EVENT_TYPE_BIND_PHONE,                 //绑定手机号回调
        EVENT_TYPE_PROTOCOL,                   //用户协议回调
        EVENT_TYPE_AD,                         //广告回调
        EVENT_TYPE_PAY,                        //支付
        EVENT_TYPE_EXIT,                       //退出
        EVENT_TYPE_SHARE,                      //分享

        EVENT_TYPE_ADDPAYMENT,                 // 添加订单

        EVENT_TYPE_SCAN_QR_CODE_RESULT,       //扫码结果回调
        EVENT_TYPE_SCAN_LOGIN_STATUS_CHANGE,  //扫码登录状态改变回调
        
        EVENT_TYPE_GIFTCODE,         //查询/使用礼物卡回调

        /// <summary>
        /// SDK 底层扩展通用回调，对应 TPFCommonEventKey
        /// </summary>
        EVENT_TYPE_COMMON_RESULT,

        /// <summary>
        /// SDK 游戏业务透传回调，对应 TPFBusiness
        /// </summary>
        EVENT_TYPE_TPF_GAME_BUSSINESS,



        /// <summary>
        /// 预留数组大小，放在最后！
        /// </summary>
        __MAX__
    }


    public class TPFSdkEvent : EventArgs
    {
        public TPFSdkEventType m_eventType;

        public TPFSdkInfo m_eventInfo;

        public int m_nErrCode;
        public string m_strErrMsg;

        public TPFSdkEventType EventType { get => m_eventType; }
        public TPFSdkInfo EventInfo { get => m_eventInfo; }
        public int ErrorCode { get => m_nErrCode; }
        public string ErrorMessage { get => m_strErrMsg; }

        public TPFSdkEvent(TPFSdkEventType eventType, TPFSdkInfo eventInfo)
        {
            m_eventType = eventType;
            m_eventInfo = eventInfo;

            if (m_eventInfo.ContainsKey(TPFParamKey.ERROR_CODE))
            {
                m_nErrCode = m_eventInfo.GetInt(TPFParamKey.ERROR_CODE);
            }
            else
            {
                m_nErrCode = TPFCode.TPFCODE_FAIL;
            }

            if (m_eventInfo.ContainsKey(TPFParamKey.ERROR_MSG))
            {
                m_strErrMsg = m_eventInfo.GetData(TPFParamKey.ERROR_MSG);
            }
            else
            {
                m_strErrMsg = "";
            }
        }

        /*
        public TPFSdkEvent(TPFSDK_EVENT_TYPE eventType)
        {
            m_eventType = eventType;
            m_eventInfo = null;
        }
        */
    }




    /// 委托（delegate）
    public delegate void TPFSdkEventDelegate(TPFSdkEvent evt);


}

