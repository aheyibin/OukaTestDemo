/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF错误码
** use: demo
** modify:             
*******************************************************************/

namespace TPFSDK
{

    public static class TPFCode
    {
        /**
        * 参数 错误
        */
        public const int TPFCODE_PARAM_ERROR = -4;

        /**
         * 参数不全
         */
        public const int TPFCODE_PARAM_NOT_COMPLETE = -3;

        /**
         * 没有网络连接
         */
        public const int TPFCODE_NO_NETWORK = -2;

        /**
         * 通用失败码
         */
        public const int TPFCODE_FAIL = -1;

        /**
         * 通用成功码
         */
        public const int TPFCODE_SUCCESS = 0;

        /**
         * 初始化成功
         */
        public const int TPFCODE_INIT_SUCCESS = 1;

        /**
         * 初始化失败
         */
        public const int TPFCODE_INIT_FAIL = 2;

        /**
         * 没有初始化
         */
        public const int TPFCODE_UNINIT = 3;

        /**
         * 登录成功
         */
        public const int TPFCODE_LOGIN_SUCCESS = 4;

        /**
         * 登录失败
         */
        public const int TPFCODE_LOGIN_FAIL = 5;

        /**
         * 登录超时
         */
        public const int TPFCODE_LOGIN_TIMEOUT = 6;

        /**
         * 取消登录
         */
        public const int TPFCODE_UNLOGIN = 7;

        /**
         * 登出成功
         */
        public const int TPFCODE_LOGOUT_SUCCESS = 8;

        /**
         * 登出失败
         */
        public const int TPFCODE_LOGOUT_FAIL = 9;

        /**
         * 支付成功
         */
        public const int TPFCODE_PAY_SUCCESS = 10;

        /**
         * 支付失败
         */
        public const int TPFCODE_PAY_FAIL = 11;

        /**
         * 分享成功
         */
        public const int TPFCODE_SHARE_SUCCESS = 12;

        /**
         * 分享失败
         */
        public const int TPFCODE_SHARE_FAILED = 13;

        /**
         * 分享成功
         */
        public const int TPFCODE_PUSH_SUCCESS = 14;

        /**
         * 分享失败
         */
        public const int TPFCODE_PUSH_FAILED = 15;


        /**
         * 实名注册成功
         */
        public const int TPFCODE_REAL_NAME_REG_SUC = 16;

        /**
         * 实名注册失败
         */
        public const int TPFCODE_REAL_NAME_REG_FAILED = 17;


        /**
         * 更新成功
         */
        public const int TPFCODE_UPDATE_SUCCESS = 18;

        /**
         * 更新失败
         */
        public const int TPFCODE_UPDATE_FAILED = 19;

        /**
         * 提交礼包兑换码成功
         */
        public const int TPFCODE_POST_GIFT_SUC = 20;

        /**
         * 提交礼包兑换码失败
         */
        public const int TPFCODE_POST_GIFT_FAILED = 21;



        /**
         * 防沉迷查询结果
         */
        public const int TPFCODE_ADDICTION_ANTI_RESULT = 22;


        /**
         * 添加Tag成功
         */
        public const int TPFCODE_TAG_ADD_SUC = 23;

        /**
         * 添加Tag失败
         */
        public const int TPFCODE_TAG_ADD_FAIL = 24;

        /**
         * 删除Tag成功
         */
        public const int TPFCODE_TAG_DEL_SUC = 25;

        /**
         * 删除Tag失败
         */
        public const int TPFCODE_TAG_DEL_FAIL = 26;

        /**
         * 添加Alias成功
         */
        public const int TPFCODE_ALIAS_ADD_SUC = 27;

        /**
         * 添加Alias失败
         */
        public const int TPFCODE_ALIAS_ADD_FAIL = 28;

        /**
         * 删除Alias成功
         */
        public const int TPFCODE_ALIAS_REMOVE_SUC = 29;

        /**
         * 删除Alias失败
         */
        public const int TPFCODE_ALIAS_REMOVE_FAIL = 30;

        /**
         * Push 收到msg
         */
        public const int TPFCODE_PUSH_MSG_RECIEVED = 31;

        /**
         * 推送enable成功的回调，携带一个参数，比如友盟推送，这参数是Device Token
         */
        public const int TPFCODE_PUSH_ENABLED = 32;

        public const int TPFCODE_EXIT_SUCCESS = 33;

        public const int TPFCODE_EXIT_CANCEL = 34;

        /**
        * 支付重复
        */
        public const int TPFCODE_PAY_REPEAT = 35;

        /**
         * 退出失败
         */
        public const int TPFCODE_EXIT_FAILED = 36;

        #region 下载
        /**
        * 下载失败
        */
        public const int TPFCODE_DOWNLOAD_ERROR = 37;
        /**
        * md5校验失败
        */
        public const int TPFCODE_DOWNLOAD_FILE_MD5_ERROR = 38;
        /**
        * 文件操作错误
        */
        public const int TPFCODE_DOWNLOAD_FILE_COPY_ERROR = 39;
        #endregion

        /**
        * 权限申请
        */
        public const int TPFCODE_PERMISSION_GRANTED = 40;
        public const int TPFCODE_PERMISSION_DENIED = 41;

        /**
        * 广告
        */
        public const int TPFCODE_AD_READY = 10030;
        public const int TPFCODE_AD_SHOW = 10031;
        public const int TPFCODE_AD_CLICK = 10032;
        public const int TPFCODE_AD_COMPLETE = 10033;
        public const int TPFCODE_AD_REWARD = 10034;
        public const int TPFCODE_AD_CLOSE = 10035;
        public const int TPFCODE_AD_ERROR = 10036;
        public const int TPFCODE_AD_FAILED = 10037;
        public const int TPFCODE_AD_CLOSE_FAILED = 10038;

        /**
        * 预下单
        */
        public const int TPFCODE_PREPAY_SUCCESS = 47;
        public const int TPFCODE_PREPAY_FAIL = 48;


        /**
         * 订单查询
         */
        public const int TPFCODE_ORDER_QUERY_SUCCESS = 49;
        public const int TPFCODE_ORDER_QUERY_FAIL = 50;


        /**
         * 订单发货
         */
        public const int TPFCODE_ORDER_CONFIRM_SUCCESS = 51;
        public const int TPFCODE_ORDER_CONFIRM_FAIL = 52;

        #region 二维码(Windows only)
        /**
         * 获取二维码失败
         */
        public const int TPFCODE_GET_QR_CODE_ERROR = 40;

        /**
         * 扫码登录服务端响应状态错误
         */
        public const int TPFCODE_SCAN_LOGIN_RSP_STATUS_ERROR = 41;

        /**
         * 二维码失效（包括二维码过期，二维码已被其他设备绑定）
         */
        public const int TPFCODE_QR_CODE_INVALID = 42;

        /**
        * 其他二维码错误
        */
        public const int TPFCODE_QR_CODE_ERROR = 43;

        /**
         * Token校验不通过
         */
        public const int TPFCODE_TOKEN_INVALID = 44;
        #endregion

        /**
        * 注册
        */
        public const int TPFCODE_REGISTER_SUCCESS = 10010;
        public const int TPFCODE_REGISTER_FAIL = 10011;
        public const int TPFCODE_REGISTER_FAIL_ACCOUNT_ALREADY_EXIST = 10012;

        public const int PLUGIN_INFO_NOT_FOUND = 104;


        /**
         * 获取手机验证码
         */
        public const int TPFCODE_VERIFY_CODE_SUCCESS = 10020;
        public const int TPFCODE_VERIFY_CODE_FAIL = 10021;

        #region 手机号绑定验证码
        /// <summary>
        /// 绑定手机号时，获取验证码成功
        /// </summary>
        public const int TPFCODE_BIND_PHONE_VERIFT_CODE_SUCCESS = 10022;

        /// <summary>
        /// 绑定手机号时，获取验证码失败
        /// </summary>
        public const int TPFCODE_BIND_PHONE_VERIFT_CODE_FAIL = 10023;
        #endregion

        #region 修改密码
        /// <summary>
        /// 修改密码成功
        /// </summary>
        public const int TPFCODE_PWD_CHANGE_SUCCESS = 10040;
        
        /// <summary>
        /// 修改密码失败
        /// </summary>
        public const int TPFCODE_PWD_CHANGE_FAIL = 10041;
        #endregion

        #region 绑定账号
        /// <summary>
        /// 账号绑定成功
        /// </summary>
        public const int TPFCODE_BIND_ACCOUNT_SUCCESS = 10050;

        /// <summary>
        /// 账号绑定失败
        /// </summary>
        public const int TPFCODE_BIND_ACCOUNT_FAIL = 10051;

        /// <summary>
        /// 账号已绑定
        /// </summary>
        public const int TPFCODE_BIND_ACCOUNT_BINDED = 10052;

        /// <summary>
        /// 账号未绑定
        /// </summary>
        public const int TPFCODE_BIND_ACCOUNT_NOT_BIND = 10053;

        /// <summary>
        /// 账号绑定查询失败
        /// </summary>
        public const int TPFCODE_BIND_ACCOUNT_QUERY_FAIL = 10054;
        #endregion

        #region 实名认证
        /// <summary>
        /// 实名认证成功(游戏需根据返回的具体信息来判断，这里的成功只代表接口调用成功)
        /// </summary>
        public const int TPFCODE_REAL_NAME_SUCCESS = 10060;

        /// <summary>
        /// 实名认证失败
        /// </summary>
        public const int TPFCODE_REAL_NAME_FAIL = 10061;

        /// <summary>
        /// 查询实名认证信息成功
        /// </summary>
        public const int TPFCODE_CHECK_VERIFY_SUCCESS = 10062;

        /// <summary>
        /// 查询实名认证信息失败
        /// </summary>
        public const int TPFCODE_CHECK_VERIFY_FAIL = 10063;

        /// <summary>
        /// 实名认证失败，暂不提供此接口(请前往用户中心进行实名认证)
        /// </summary>
        public const int TPFCODE_REAL_NAME_UNABLE = 10064;
        #endregion

        /**
        * 获取用户信息
        */
        public const int TPFCODE_GET_USER_INFO_SUCCESS = 10070;
        public const int TPFCODE_GET_USER_INFO_FAIL = 10071;

        /**
         * 下单错误码(防沉迷新增)
         */
        public const int TPFCODE_PAY_CANCEL = 10082;
        public const int TPFCODE_PAY_UNDER_8 = 10083;
        public const int TPFCODE_PAY_UNDER_16 = 10084;
        public const int TPFCODE_PAY_UNDER_18 = 10085;
        public const int TPFCODE_PAY_UNDER_200 = 10086;
        public const int TPFCODE_PAY_UNDER_400 = 10087;
        public const int TPFCODE_PAY_UNKNOWN = 10088;

        #region 查询手机号
        /// <summary>
        /// 查询手机号成功
        /// </summary>
        public const int TPFCODE_QUERY_PHONE_SUCCESS = 10090;

        /// <summary>
        /// 查询手机号失败
        /// </summary>
        public const int TPFCODE_QUERY_PHONE_FAIL = 10091;
        #endregion

        #region 验证码校验
        /// <summary>
        /// 手机验证码校验成功
        /// </summary>
        public const int TPFCODE_VERIFY_CODE_CHECK_SUCCESS = 10110;

        /// <summary>
        /// 手机验证码校验失败
        /// </summary>
        public const int TPFCODE_VERIFY_CODE_CHECK_FAIL = 10111;
        #endregion

        #region 绑定手机号
        /// <summary>
        /// 绑定手机号成功
        /// </summary>
        public const int TPFCODE_BIND_PHONE_SUCCESS = 10120;

        /// <summary>
        /// 绑定手机号失败
        /// </summary>
        public const int TPFCODE_BIND_PHONE_FAIL = 10121;
        #endregion

        #region 礼包码相关错误码
        /// <summary>
        /// 查询礼包卡成功
        /// </summary>
        public const int TPFCODE_QUERY_GIFTCODE_SUCCESS = 10130;

        /// <summary>
        /// 查询礼包卡失败
        /// </summary>
        public const int TPFCODE_QUERY_GIFTCODE_FAIL = 10131;

        /// <summary>
        /// 使用礼包卡成功
        /// </summary>
        public const int TPFCODE_POST_GIFTCODE_SUCCESS = 10132;

        /// <summary>
        /// 使用礼包卡失败
        /// </summary>
        public const int TPFCODE_POST_GIFTCODE_FAIL = 10133;
        #endregion

        #region 用户协议
        /// <summary>
        /// 同意用户协议成功
        /// </summary>
        public const int TPFCODE_USER_PROTOCOL_SUCCESS = 10140;

        /// <summary>
        /// 同意用户协议错误
        /// </summary>
        public const int TPFCODE_USER_PROTOCOL_FAIL = 10141;
        #endregion

        #region KV云存档
        public const int TPFCODE_KV_SUCCESS = 200;

        public const int TPFCODE_KV_ERROR_TOKEN = 1040;

        public const int TPFCODE_KV_SAVE_SUCCESS = 10170;
        public const int TPFCODE_KV_SAVE_FAIL = 10171;
        public const int TPFCODE_KV_QUERY_SUCCESS = 10172;
        public const int TPFCODE_KV_QUERY_FAIL = 10173;
        public const int TPFCODE_KV_VERIFY_PERMISSION_SUCCESS = 10174;
        public const int TPFCODE_KV_VERIFY_PERMISSION_FAIL = 10175;
        public const int TPFCODE_KV_REMVOE_SUCCESS = 10176;
        public const int TPFCODE_KV_REMVOE_FAIL = 10177;



        #endregion

        //客服获取配置
        public const int TPFCODE_CUSTOMERCFG_SUCCESS = 10180;
        public const int TPFCODE_CUSTOMERCFG_FAIL = 10181;
        //消息推送通知
        public const int TPFCODE_PUSHMESSAGE_NOTIFY = 10190;
        //qq相关
        public const int TPFCODE_QQ_ACTIVATE        = 10200;
        public const int TPFCODE_QQ_NOT_ACTIVATE    = 10201;
        public const int TPFCODE_QQ_JUMP_SUCCESS    = 10202;
        public const int TPFCODE_QQ_NOT_INSTALL     = 10203;

    }
}
