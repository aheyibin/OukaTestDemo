using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business 
{

    internal class GetMailsBriefInfoRequest : ExternalBusinessRequest
    {
        Dictionary<string, object> m_Body;
        public GetMailsBriefInfoRequest(TPFSdkInfo info, TPFSdkEventDelegate callback) 
            : base(MailExtensions.MESSAGE_MAIL_PROVIDRID, MailExtensions.MAIL_MSGID, TPFCode.TPFCODE_FAIL, callback)
        {
            var submitInfo = CoreDataCacher.GetSubmitInfo();
            string serverId = submitInfo.GetData(TPFParamKey.SERVER_ID);
            string roleId = submitInfo.GetData(TPFParamKey.ROLE_ID);
            long offset = info.GetLong("offset");
            var data = new Dictionary<string, object>
             {
                {"appId"   , ITPFSdk.Instance.appId},
                {"regionId", serverId},
                {"ownerId" , ITPFSdk.Instance.channelId},
                {"offset"  , offset},
                {"userId"  , roleId},
            };
            m_Body = BusinessUtils.WarpMessageReq("mail", "clientPullMail", data);
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            Debug.Log("brief mail resp:  " + response);
            int errCode;
            Dictionary<string, object> respContent = BusinessUtils.GetMessageSrvRespMeta(response, out errCode);
            if (errCode != 1)
            {
                Debug.LogError("获取简要邮件失败" + errCode);
                return BusinessUtils.WarpCallbackParam(TPFCode.TPFCODE_FAIL, "get mail brief failed", errCode, "get mail brief failed", null);
            }
            else
            {
                Debug.Log("获取简要邮件 success!");
                var extra = new Dictionary<string, object>();
                extra["offset"] = (long)respContent["offset"];
                if (respContent.ContainsKey("simpleInfoList"))
                {
                    extra["list"] = respContent["simpleInfoList"] as List<object>;
                }
                else
                {
                    extra["list"] = new List<object>();
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
