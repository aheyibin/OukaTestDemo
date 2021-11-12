using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business 
{

    internal class GetMailsDetailInfoRequest : ExternalBusinessRequest
    {
        Dictionary<string, object> m_Body;
        public GetMailsDetailInfoRequest(TPFSdkInfo info, TPFSdkEventDelegate callback) 
            : base(MailExtensions.MESSAGE_MAIL_PROVIDRID, MailExtensions.MAIL_MSGID, TPFCode.TPFCODE_FAIL, callback)
        {
            var submitInfo = CoreDataCacher.GetSubmitInfo();
            string serverId = submitInfo.GetData(TPFParamKey.SERVER_ID);
            string roleId = submitInfo.GetData(TPFParamKey.ROLE_ID);
            string mails = info.GetData("mails");
            string[] mailArray = mails.Split(',');
            var data = new Dictionary<string, object>
             {
                {"appId"     , ITPFSdk.Instance.appId},
                {"regionId"  , serverId},
                {"ownerId"   , ITPFSdk.Instance.channelId},
                {"mailIdList", mailArray},
                {"userId"    , roleId},
            };
            m_Body = BusinessUtils.WarpMessageReq("mail", "clientPullMailDetailInfo", data);
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            Debug.Log("detail mail resp:  " + response);
            int errCode;
            Dictionary<string, object> respContent = BusinessUtils.GetMessageSrvRespMeta(response, out errCode);
            if (errCode != 1)
            {
                Debug.LogError("获取详细邮件失败" + errCode);
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_FAIL, "get mail detail failed", errCode, "get mail detail failed", null);
            }
            else
            {
                Debug.Log("获取详细邮件 success!");
                var extra = new List<object>();
                if (respContent.ContainsKey("detailInfoList"))
                {
                    extra = respContent["detailInfoList"] as List<object>;
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
