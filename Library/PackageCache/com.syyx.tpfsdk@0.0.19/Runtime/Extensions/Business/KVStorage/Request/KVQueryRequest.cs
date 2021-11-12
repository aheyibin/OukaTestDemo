using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TPFSDK.Business
{
    internal class KVQueryRequest : ExternalBusinessRequest
    {
        public const string KV_STORAGE_QUERY_MSGID = "700023003";
        public const string KV_STORAGE_QUERYALL_MSGID = "700023005";

        private Dictionary<string, object> m_Body;
        public KVQueryRequest(List<string> data, TPFSdkEventDelegate callback) 
            : base(KVExtensions.KV_STORAGE_PROVIDERID, KV_STORAGE_QUERY_MSGID, TPFCode.TPFCODE_KV_QUERY_FAIL, callback)
        {
            if(data == null)
            {
                SetMsgId(KV_STORAGE_QUERYALL_MSGID);
                m_Body = KVExtensions.WarpKVBody(string.Empty, null);
            }
            else
            {
                SetMsgId(KV_STORAGE_QUERY_MSGID);
                m_Body = KVExtensions.WarpKVBody("keys", data);
            }
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(response);
            KVExtensions.GetKVHttpRspMeta(json_dict, out var errCode, out var errMsg);
            if (errCode != (int)KVExtensions.TPFKVStorageCode.Success)
            {
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_QUERY_FAIL, "fail", errCode, errMsg, null);
            }
            var dataNode = (Dictionary<string, object>)json_dict["datas"];
            List<object> dataList = (List<object>)dataNode["datas"];

            Dictionary<string, object> dataResult = new Dictionary<string, object>();
            for (int i = 0; i < dataList.Count; ++i)
            {
                var tmp = (Dictionary<string, object>)dataList[i];
                string key = (string)tmp["key"];
                string value = (string)tmp["value"];
                dataResult.Add(key, value);
            }
            Debug.Log("kvquery success! " + response);
            return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_QUERY_SUCCESS, "success", errCode, errMsg, dataResult);
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }

}
