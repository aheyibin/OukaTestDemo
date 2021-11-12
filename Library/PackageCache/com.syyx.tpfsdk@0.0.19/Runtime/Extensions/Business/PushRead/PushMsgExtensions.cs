using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business
{
    public static class PushMsgExtensions
    {
        internal const string PROVIDER_ID = "700020";

        public static void OnPushMsgRead(this ITPFSdk sdk, TPFSdkInfo info)
        {
            var request = new OnPushMsgReadRequest(info);
            request.Post();
        }
    }

}
