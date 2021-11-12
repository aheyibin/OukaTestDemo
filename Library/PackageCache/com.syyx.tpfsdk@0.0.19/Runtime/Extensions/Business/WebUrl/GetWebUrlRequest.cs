using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business
{
    internal class GetWebUrlRequest : ExternalBusinessRequest
    {
        private Dictionary<string, object> m_Body;
        public GetWebUrlRequest(TPFSdkInfo info, TPFSdkEventDelegate callback)
            : base(WebUrlExtensions.MESSAGE_QUESTIONNAIRE_PROVIDERID, WebUrlExtensions.QUESTIONNAIRE_JSONREQID, TPFCode.TPFCODE_FAIL, callback)
        {
            object type = info.GetDataObject("type");
            object registerTime = info.GetDataObject("registerTime");
            if (type == null)
            {
                type = 1;
            }
            if (registerTime == null)
            {
                registerTime = 0;
            }
            var submitInfo = CoreDataCacher.GetSubmitInfo();
            string serverId = submitInfo.GetData(TPFParamKey.SERVER_ID);
            string roleId = submitInfo.GetData(TPFParamKey.ROLE_ID);
            string roleLevel = submitInfo.GetData(TPFParamKey.ROLE_LEVEL);
            var msgContent = new Dictionary<string, object>()
            {
                {"appId",  ITPFSdk.Instance.appId},
                {"regionId",serverId},
                {"ownerId",ITPFSdk.Instance.channelId},
                {"uid",roleId},
                {"level",roleLevel},
                {"registerTime",registerTime},
            };
            m_Body = BusinessUtils.WarpMessageReq("questionnaire", "findUrlLinks", msgContent);
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            Debug.Log("questionnaire data:  " + response);
            int errCode;
            Dictionary<string, object> respContent = BusinessUtils.GetMessageSrvRespMeta(response, out errCode);
            if (errCode != 1)
            {
                Debug.LogError("获取问卷链接失败" + errCode);
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_FAIL, "get questionnaire failed", errCode, "get questionnaire failed", null);
            }
            else
            {
                Debug.Log("get questionnaire success!");
                var extra = new Dictionary<string, object>();
                extra["url"] = respContent["urlLink"];
                extra["serviceType"] = "1";
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_SUCCESS, "success", errCode, "success", extra);
            }
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }

}
