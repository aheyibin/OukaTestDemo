namespace TPFSDK.Normal.UnityPlugins
{
    /**
     * 扫码登录状态
     * 对应于客户端显示的状态
     */
    public enum ScanLoginStatus
    {
        /**
         * 初始状态
         */
        Idle,

        /**
         * 等待登录确认
         */
        ConfirmLogin,

        /**
         * 用户取消登录
         */
        CancelLogin,

        /**
         * 登录成功
         */
        LoginSuccess,

        /**
         * 登录失败
         */
        LoginFailed,

        /**
         * 本次二维码超时
         */
        Timeout,
    }
}