using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business
{
    internal class KVBatchSaveRequest : ExternalBusinessRequest
    {
        public const string KV_STORAGE_BATCHSAVE_MSGID = "700023004";
        
        private Dictionary<string, object> m_Body;

        public KVBatchSaveRequest(Dictionary<string, string> data, long version, TPFSdkEventDelegate callback) 
            : base(KVExtensions.KV_STORAGE_PROVIDERID, KV_STORAGE_BATCHSAVE_MSGID, TPFCode.TPFCODE_KV_SAVE_FAIL, callback)
        {
            List<object> dataToS = new List<object>();
            if (version == 0)
            {
                version = Utils.TimeUtils.ToUnixTime(DateTime.Now);
            }
            foreach (KeyValuePair<string, string> kvp in data)
            {
                Dictionary<string, object> tmp = new Dictionary<string, object>();
                tmp.Add("key", kvp.Key);
                tmp.Add("value", kvp.Value);
                tmp.Add("version", version);
                dataToS.Add(tmp);
            }

            Dictionary<string, object> dataToService = new Dictionary<string, object>() { { "datas", dataToS } };
            m_Body = KVExtensions.WarpKVBody("data", dataToService);

        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(response);
            KVExtensions.GetKVHttpRspMeta(json_dict, out var errCode, out var errMsg);
            if (errCode != (int)KVExtensions.TPFKVStorageCode.Success)
            {
                //Debug.Log("Get Offical token failed, meta.errCode: "+errCode+", errCode, errMsg: "+errMsg);
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_SAVE_FAIL, "fail", errCode, errMsg, null);
            }
            var dataNode = (List<object>)json_dict["keys"];
            Debug.Log("kv batchsave success! " + response);
            return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_SAVE_SUCCESS, "success", errCode, errMsg, new Dictionary<string, object>() { { "keys", dataNode } });
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }

}
