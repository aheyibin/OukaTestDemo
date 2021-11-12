namespace TPFSDK.Normal.UnityPlugins
{
    /**
     * 二维码状态，用于与服务端进行交互
     * 区别于ScanLoginStatuse，不暴露给业务层
     */
    internal sealed class QRCodeStatusCode
    {
        /**
         * 二维码超时
         */
        public const string Timeout = "-1";

        /**
         * 初始状态
         */
        public const string Init = "0";

        /**
         * 登录校验
         */
        public const string VerifyLogin = "1";

        /**
         * 登录确认
         */
        public const string ConfirmLogin = "10";

        /**
         * 用户取消登录
         */
        public const string CancelLogin = "11";
    }
}