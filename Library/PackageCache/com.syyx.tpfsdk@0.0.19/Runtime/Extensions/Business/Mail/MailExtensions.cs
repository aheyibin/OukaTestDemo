namespace TPFSDK.Business
{
    public static class MailExtensions
    {
        internal const string MESSAGE_MAIL_PROVIDRID = "700066";
        internal const string MAIL_MSGID = "700066100";

        public static void GetMailsBriefInfo(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            var request = new GetMailsBriefInfoRequest(info, callback);
            request.Post();
        }

        public static void GetMailsDetailInfo(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            var request = new GetMailsDetailInfoRequest(info, callback);
            request.Post();
        }

    }

}

