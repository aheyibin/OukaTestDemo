namespace TPFSDK.Normal.UnityPlugins
{
    /**
     * HTTP请求的meta头部错误码
     */
    public enum MetaErrCode 
    {
        /**
         * 成功
         */
        Success = 0,

        /**
         * 二维码对应的QrId已经被其他设备绑定
         */
        FailQrCodeOccupied = 15003,

        /**
         * 二维码已过期
         */
        FailQrCodeExpired = 15014,

        /**
         * Token失效
         */
        FailTokenInvalid = 15110
    }
}