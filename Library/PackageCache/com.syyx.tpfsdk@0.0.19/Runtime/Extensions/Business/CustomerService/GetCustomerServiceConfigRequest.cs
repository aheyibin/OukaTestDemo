using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TPFSDK.Business
{
    internal class GetCustomerServiceConfigRequest : ExternalBusinessRequest
    {
        public const string MSG_ID = "4001";

        private Dictionary<string, object> m_Body;

        public GetCustomerServiceConfigRequest(TPFSdkEventDelegate callback) 
            : base(CustomerServiceExtensions.PROVIDER_ID, MSG_ID, TPFCode.TPFCODE_CUSTOMERCFG_FAIL, callback)
        {
            var sdk = ITPFSdk.Instance;
            m_Body = new Dictionary<string, object>()
            {
                { "appId"   , sdk.appId    },
                { "regionId", sdk.channelId},
                { "version" , "1.0.0"      },
                { "cfgId"   , "kefu"       },
                { "cfgType" , "string"     }
            };
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            var json_dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(response);
            int errCode = Convert.ToInt32(json_dict["errorCode"]);
            if (errCode == 1)
            {
                string json = json_dict["content"] as string;
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_CUSTOMERCFG_SUCCESS, "success", errCode, "success", json);
            }
            return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_CUSTOMERCFG_FAIL, "fail", errCode, "fail", null);
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }
}
