using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business 
{
    internal class KVRemoveRequest : ExternalBusinessRequest
    {
        public const string KV_STORAGE_REMOVE_MSGID = "700023006";

        private Dictionary<string, object> m_Body;

        public KVRemoveRequest(List<string> data, TPFSdkEventDelegate callback) 
            : base(KVExtensions.KV_STORAGE_PROVIDERID, KV_STORAGE_REMOVE_MSGID, TPFCode.TPFCODE_KV_REMVOE_FAIL, callback)
        {
            m_Body = KVExtensions.WarpKVBody("keys", data);
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(response);
            KVExtensions.GetKVHttpRspMeta(json_dict, out var errCode, out var errMsg);
            if (errCode != (int)KVExtensions.TPFKVStorageCode.Success)
            {
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_REMVOE_FAIL, "fail",errCode, errMsg, null);

            }
            Debug.Log("kv remove success! " + response);
            return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_REMVOE_SUCCESS, "success", errCode, errMsg, null);
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }
}
