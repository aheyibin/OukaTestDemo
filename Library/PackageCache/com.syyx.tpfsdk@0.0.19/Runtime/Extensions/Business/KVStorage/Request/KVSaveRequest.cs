using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business
{

    internal class KVSaveRequest : ExternalBusinessRequest
    {
        public const string KV_STORAGE_ONCESAVE_MSGID = "700023002";

        private Dictionary<string, object> m_Body;
        public KVSaveRequest(Dictionary<string, string> data, long version, TPFSdkEventDelegate callback) 
            : base(KVExtensions.KV_STORAGE_PROVIDERID, KV_STORAGE_ONCESAVE_MSGID, TPFCode.TPFCODE_KV_SAVE_FAIL, callback)
        {
            if (version == 0)
            {
                version = Utils.TimeUtils.ToUnixTime(DateTime.Now);
            }
            Dictionary<string, object> dataToSave = new Dictionary<string, object>();
            foreach (string k in data.Keys)
            {
                dataToSave.Add("key", k);
                dataToSave.Add("value", data[k]);
                dataToSave.Add("version", version);
            }

            m_Body = KVExtensions.WarpKVBody("data", dataToSave);
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(response);
            KVExtensions.GetKVHttpRspMeta(json_dict, out var errCode, out var errMsg);
            if (errCode != (int)KVExtensions.TPFKVStorageCode.Success)
            {
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_SAVE_FAIL, "fail", errCode, errMsg, null);
            }
            Debug.Log("kv save success! " + response);
            return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_SAVE_SUCCESS, "success", errCode, errMsg, null);
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }

}
