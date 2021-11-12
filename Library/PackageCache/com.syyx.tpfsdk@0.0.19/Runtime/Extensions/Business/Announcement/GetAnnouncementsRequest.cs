using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TPFSDK.Business 
{
    internal class GetAnnouncementsRequest : ExternalBusinessRequest
    {
        internal const string ANNOUNCEMENT_JSONREQID = "700016003";

        private Dictionary<string, object> m_Body;
        
        public GetAnnouncementsRequest(TPFSdkInfo info, TPFSdkEventDelegate callback) 
            : base(AnnouncementExtensions.MESSAGE_ANNOUNCEMENT_PROVIDERID, ANNOUNCEMENT_JSONREQID, TPFCode.TPFCODE_FAIL, callback)
        {
            object idMap = info.GetDataObject("announces");
            if (idMap == null)
            {
                idMap = new Dictionary<string, string>();
            }
            var submitInfo = CoreDataCacher.GetSubmitInfo();
            var msgContent = new Dictionary<string, object>()
            {
                {"appId",  ITPFSdk.Instance.appId},
                {"regionId",submitInfo.GetData(TPFParamKey.SERVER_ID)},
                {"channelId",ITPFSdk.Instance.channelId},
                {"idMap",idMap},
            };
            m_Body = BusinessUtils.WarpMessageReq("announcement", "clientPullAnnouncementReq", msgContent);
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            Debug.Log("ann data:  " + response);
            int errCode;
            Dictionary<string, object> respContent = BusinessUtils.GetMessageSrvRespMeta(response, out errCode);
            if (errCode != 1)
            {
                Debug.LogError("获取公告失败" + errCode);
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_FAIL, "fail", errCode, "get announcement failed", null);
            }
            else
            {
                Debug.Log("get announcement success!");
                var extra = new List<object>();
                if (respContent.ContainsKey("announcementList"))
                {
                    extra = respContent["announcementList"] as List<object>;
                }
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_SUCCESS, "success", errCode, "success", extra);
            }
        }

        protected override object GetBody()
        {
            return m_Body;
        }
    }

}

