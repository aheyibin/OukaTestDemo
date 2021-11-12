namespace TPFSDK.Business 
{
    public static class WebUrlExtensions 
    {
        internal const string MESSAGE_QUESTIONNAIRE_PROVIDERID = "700060";
        internal const string QUESTIONNAIRE_JSONREQID = "700060100";

        public static void GetWebUrls(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            var request = new GetWebUrlRequest(info, callback);
            request.Post();
        }
    }

}
