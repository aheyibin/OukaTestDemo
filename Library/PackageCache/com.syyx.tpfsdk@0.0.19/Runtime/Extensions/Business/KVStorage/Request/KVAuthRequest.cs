using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business
{
    internal class KVAuthRequest : InternalBusinessRequest
    {
        public const string KV_STORAGE_VERIFY_MSGID = "700023001";

        private bool m_IsAuthSuccess = false;

        private Dictionary<string, object> m_Body;

        public KVAuthRequest() : base(KVExtensions.KV_STORAGE_PROVIDERID, KV_STORAGE_VERIFY_MSGID)
        {
            var dataHeadDic = new Dictionary<string, object>
            {
                { "appId"    , ITPFSdk.Instance.appId      },
                { "regionId" , ITPFSdk.Instance.channelId  },
                { "ownerId"  , CoreDataCacher.GetUserId()   },
                { "token"    , CoreDataCacher.GetUserToken()} // 鉴权使用的 token 是登录的 token，不是 kvToken
            };
            m_Body = new Dictionary<string, object>
            {
                { "header"   , dataHeadDic},
            };
        }

        public override Dictionary<string, object> OnExceptionReturnData(Exception e)
        {
            return BusinessUtils.WarpRequestExceptionParam(TPFCode.TPFCODE_KV_VERIFY_PERMISSION_FAIL);
        }

        public override Dictionary<string, object> OnFailureReturnData(long code, string msg)
        {
            return BusinessUtils.WarpRequestFailureParam(TPFCode.TPFCODE_KV_VERIFY_PERMISSION_FAIL, code, msg);
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(response);
            KVExtensions.GetKVHttpRspMeta(json_dict, out var errCode, out var errMsg);
            Debug.Log($"errCode = {errCode}, errMsg = {errMsg}");
            if (errCode != (int)KVExtensions.TPFKVStorageCode.Success)
            {
                var out_param_error = BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_KV_VERIFY_PERMISSION_FAIL, "token error", errCode, errMsg, null);
                return out_param_error;
            }
            Debug.Log("[TPFSDK] kv verifypermission success! " + response);
            string kvToken = json_dict["token"] as string;
            Dictionary<string, object> header = new Dictionary<string, object>()
            {
                {"appId", ITPFSdk.Instance.appId },
                {"channelId", ITPFSdk.Instance.channelId },
                {"ownerId", CoreDataCacher.GetUserId() },
                {"token", kvToken }
            };
            KVExtensions.UpdateKVInfo(header, kvToken);
            m_IsAuthSuccess = true;
            return null;
        }

        public override void ProcessReturnData(Dictionary<string, object> data)
        {
            if (m_IsAuthSuccess)
            {
                KVExtensions.ExcuteAllQueuedCommands();
            }
            else
            {
                KVExtensions.NotifyAllQueuedCommands(data);
            }
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }

}
