#define TEST_ENV

namespace TPFSDK.Utils
{
    public class UrlDefine
    {
        public const string HttpPrefix = "http://";
        public const string HttspPrefix = "https://";

        public static string UrlHost
        {
#if TEST_ENV
            get { return "192.168.10.197:996"; }
#else
            get { return "192.168.10.197:996"; }
#endif
        }

        public static string GetQRCodeUrl
        {
            get { return HttpPrefix + UrlHost + "/tpf-login/code/qrCodeL"; }
        }

        public static string QueryQRCodeStatusUrl
        {
            get { return HttpPrefix + UrlHost + "/tpf-login/code/qrCodeStatus"; }
        }

        public static string PluginConfigUrl
        {
            get { return HttpPrefix + "10.100.0.216:20010"; }
        }
    }
}