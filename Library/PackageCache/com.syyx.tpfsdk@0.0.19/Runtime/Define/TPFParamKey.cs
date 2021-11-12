/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的参数key
** use: demo
** modify:             
*******************************************************************/

namespace TPFSDK
{
    public static class TPFParamKey
    {
        public const string SDK_JSON_DATA = "SdkJsonData";
        public const string ERROR_CODE = "ErrorCode";       //错误码  int
        public const string ERROR_MSG = "ErrorMsg";       //错误信息  int

        ///接口参数 begin
        public const string PRODUCT_ID = "ProductId";       //sdk那里购买物品注册ID         String
        public const string PRODUCT_NAME = "ProductName";  //sdk那里购买物品注册名字        String
        public const string PRODUCT_DESC = "ProductDesc";  //sdk那里购买物品注册解释        String
        public const string RECEIPT = "Receipt";        //AppStore 返回的收据            String
        public const string PRICE = "Price";                 //购买价格                       int
        public const string RATIO = "Ratio";                 //兑换比例                       int
        public const string BUY_NUM = "BuyNum";             //购买数量                        int
        public const string COIN_NUM = "CoinNum";           //金币数量                        int
        public const string SERVER_ID = "ServerId";         //服务器ID                        String
        public const string SERVER_NAME = "ServerName";    //服务器名字                       String
        public const string ROLE_ID = "RoleId";             //角色ID                         String
        public const string ROLE_NAME = "RoleName";         //角色名字                      String
        public const string ROLE_LEVEL = "RoleLevel";      //角色等级                       int
        public const string NOTIFY_URL = "NotifyUrl";      //反馈URL                         String
        public const string VIP = "Vip";                     //VIP等级                        String
        public const string ORDER_ID = "OrderId";          //自主订单ID                      String
        public const string EXTRA = "Extra";                //额外数据                        String

        public const string TOKEN = "Token";               //token                            string
        public const string SSID = "ssid";                   //ssid                           string
        public const string ID = "id";                      //id                            string
        public const string LOGIN_DETAIL = "SdkLoginDetail";  //登陆回馈的详细信息 发送到服务端去解析  string
        public const string SDK_ERRCODE = "SdkErrCode";   //sdk反馈的errorcode                string
        public const string SDK_ERRMSG = "SdkErrMsg";       //sdk反馈的errmsg                string

        public const string SDK_EXTRA = "SdkExtra";         // 微信支付必须参数                 string


        // title标题，在印象笔记、邮箱、信息、微信（包括好友、朋友圈和收藏）、 易信（包括好友、朋友圈）、人人网和QQ空间使用，否则可以不提供
        public const string SHARE_TITLE = "Title";  //分享的标题，最大30个字符  String

        public const string SHARE_IS_USE_TITLE_URL = "UseTitleUrl"; // 是否设置titleurl  bool

        //titleUrl是标题的网络链接，仅在人人网和QQ空间使用，否则可以不提供
        public const string SHARE_TITLE_URL = "TitleUrl";//标题链接  String

        //site是分享此内容的网站名称，仅在QQ空间使用，否则可以不提供
        public const string SHARE_SOURCE_NAME = "SourceName";//分享此内容显示的出处名称 String

        //siteUrl是分享此内容的网站地址，仅在QQ空间使用，否则可以不提供
        public const string SHARE_SOURCE_URL = "SourceUrl";//出处链接  String

        //text是分享文本，所有平台都需要这个字段
        public const string SHARE_CONTENT = "Content";//内容，最大130个字符 String

        //是否是分享链接 如果不是分享链接的话，则不设置，
        public const string SHARE_IS_USE_URL = "useUrl"; //bool

        // url仅在微信（包括好友和朋友圈）中使用
        public const string SHARE_URL = "Url";//链接，微信分享的时候会用到 String

        // 是否使用imgurl
        public const string SHARE_IS_USE_IMG_URL = "useImgUrl"; //bool

        //imageUrl是图片的网络路径，新浪微博、人人网、QQ空间和Linked-In支持此字段
        public const string SHARE_IMG_URL = "ImgUrl";//图片地址 String

        //设置编辑页面的显示模式为Dialog模式
        public const string SHARE_DIALOG_MODE = "DialogMode";//是否全屏还是对话框 bool

        // 分享时Notification的图标和文字  2.5.9以后的版本不调用此方法
        public const string SHARE_NOTIFY_ICON = "NotifyIcon";//Notification的图标 int

        public const string SHARE_NOTIFY_ICON_TEXT = "NotifyIconText";//Notification的文字 String

        //comment是我对这条分享的评论，仅在人人网和QQ空间使用，否则可以不提供
        public const string SHARE_COMMENT = "Comment";//内容的评论，人人网分享必须参数，不能为空  String

        //是否使用imgpath
        public const string SHARE_IS_USE_IMG_PATH = "useImgPath"; //bool

        //imagePath是本地的图片路径，除Linked-In外的所有平台都支持这个字段
        public const string SHARE_IMG_PATH = "ImgPath";//图片的本地路径 String

        // 二维码的内容
        public const string QR_CODE_CONTENT = "qrCodeContent";

        // 登录二维码的状态（这个值对应ScanLoginStatus）
        public const string SCAN_LOGIN_STATUS = "scanLoginStatus";

        // 客户端携带的附加数据
        public const string CLIENT_EXTRA = "clientExtra";
        public const string TASKTYPE_APK = "TaskAPK";
        public const string TASKTYPE_MICRO = "TaskMicro";
        public const string MAXSPEED = "MaxSpeed";
        public const string RETRYNUM = "RetryNum";
        public const string RETRYNUMINTERVAL = "RetryInterval";
        public const string PATH = "Path";
        public const string URL = "Url";
        public const string HEADER = "Header";
        public const string BODY = "Body";
        public const string NAME = "Name";
        public const string MD5 = "Md5";

        // 注册登陆验证码相关参数
        /// <summary>
        /// 手机
        /// </summary>
        public const string PHONE = "phone";
        /// <summary>
        /// 账号
        /// </summary>
        public const string ACCOUNT = "account";
        /// <summary>
        /// 账号类型，0：账户名 1：邮箱 2：手机号
        /// </summary>
        public const string ACCOUNT_TYPE = "accountType"; // 账号类型
        /// <summary>
        /// 账号密码
        /// </summary>
        public const string PASSWORD = "password";
        /// <summary>
        /// 请求类别，不同接口下不同语义
        /// </summary>
        public const string TYPE = "type";
        /// <summary>
        /// 校验方式，0：密码 1：验证码
        /// </summary>
        public const string CHECK_TYPE = "checkType";
        /// <summary>
        /// 验证码
        /// </summary>
        public const string VERIFY_CODE = "msgCode";

        // SDK 回调的 json key
        public const string JSON_DATA_KEY = "JsonData";
        public const string SESSION_ID_KEY = "SessionId";
        public const string COMMON_EVENT_KEY = "CommonEventKey";
        public const string HTTP_RESPOND_CODE = "HttpRespondCode";
    }

}