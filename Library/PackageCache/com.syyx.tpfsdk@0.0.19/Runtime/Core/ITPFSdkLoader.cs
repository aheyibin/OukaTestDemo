using UnityEngine;

namespace TPFSDK
{
    //业务侧可实现定制，在资源热更后，采用同步加载
    public abstract class ITPFSdkLoader
    {
        //加载某个配置内容
        public abstract string LoadConfig();

        /// <summary>
        /// 须在调用 ITPFSdk.Instance 前调用
        /// </summary>
        /// <param name="customLoader"></param>
        public static void SetCustomLoader(ITPFSdkLoader customLoader)
        {
            if (customLoader != null)
            {
                s_Loader = customLoader;
            }
        }

        //tpfsdk用到的资源加载器
        private static ITPFSdkLoader s_Loader;

        internal static ITPFSdkLoader GetLoader()
        {
            if(s_Loader == null)
            {
                s_Loader = new DefaultLoader();
            }
            return s_Loader;
        }

    }

    //默认加载器,加载resource下资源
    public class DefaultLoader : ITPFSdkLoader
    {
        public const string CONFIG_PATH = "tpf_sdk_config";
        public override string LoadConfig()
        {
            var configAsset = Resources.Load<TextAsset>(CONFIG_PATH);
            if (configAsset == null) return null;
            return configAsset.text;
        }
    }
}


