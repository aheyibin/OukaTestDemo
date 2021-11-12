using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business 
{

    internal class OnPushMsgReadRequest : InternalBusinessRequest
    {
        public class ServiceType 
        {
            /// <summary>
            /// 问卷
            /// </summary>
            public const int QUESTION = 1;

            /// <summary>
            /// 客服
            /// </summary>
            public const int CUSTOMER = 2;
        }


        private static Dictionary<int, string> s_Type2Key;
        static OnPushMsgReadRequest()
        {
            s_Type2Key = new Dictionary<int, string>()
            {
                { ServiceType.CUSTOMER, "kefu" }
            };
        }

        private string GetTypeKey(int type)
        {
            if (s_Type2Key.TryGetValue(type, out var key))
            {
                return key;
            }
            Debug.LogError($"[TPFSDK] unknown OnPushMsgRead type = {type}");
            return string.Empty;
        }

        internal const string MSG_ID = "700020002";

        private Dictionary<string, object> m_Body;
        public OnPushMsgReadRequest(TPFSdkInfo info) 
            : base(PushMsgExtensions.PROVIDER_ID, MSG_ID)
        {
            int type = info.GetInt("type");
            string key = GetTypeKey(type);
            m_Body = new Dictionary<string, object>()
            {
                { "appId", ITPFSdk.Instance.appId },
                { "id", CoreDataCacher.GetUserId() },
                { "key",  key  }
            };
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }

}
