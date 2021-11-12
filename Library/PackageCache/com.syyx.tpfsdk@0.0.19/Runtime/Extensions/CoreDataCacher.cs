namespace TPFSDK
{

    internal static class CoreDataCacher
    {
        static CoreDataCacher()
        {
            CoreExtensions.Subscribe(CoreExtensions.CoreEvent.LOGIN, InvalidateLoginInfo);
            CoreExtensions.Subscribe(CoreExtensions.CoreEvent.SUBMIT_INFO, InvalidateSubmitInfo);
        }

        #region submit info
        private static bool s_IsSubmitInfoValid = false;

        private static TPFSdkInfo s_SubmitInfo;

        private static void InvalidateSubmitInfo()
        {
            s_IsSubmitInfoValid = false;
        }

        public static TPFSdkInfo GetSubmitInfo()
        {
            if (!s_IsSubmitInfoValid)
            {
                s_SubmitInfo = new TPFSdkInfo(ITPFSdk.Instance.GetSubmitInfo());
                s_IsSubmitInfoValid = true;
            }
            return s_SubmitInfo;
        }
        #endregion

        #region user login info
        private static bool s_IsLoginInfoValidate = false;
        
        private static string s_UserId;
        private static string s_UserToken;

        private static void InvalidateLoginInfo()
        {
            s_IsLoginInfoValidate = false;
        }

        private static void ValidateLoginInfo()
        {
            if (!s_IsLoginInfoValidate)
            {
                s_UserId = ITPFSdk.Instance.GetUserId();
                s_UserToken = ITPFSdk.Instance.GetToken();
                s_IsLoginInfoValidate = true;
            }
        }

        public static string GetUserId()
        {
            ValidateLoginInfo();
            return s_UserId;
        }

        public static string GetUserToken()
        {
            ValidateLoginInfo();
            return s_UserToken;
        }


        #endregion
    }

}
