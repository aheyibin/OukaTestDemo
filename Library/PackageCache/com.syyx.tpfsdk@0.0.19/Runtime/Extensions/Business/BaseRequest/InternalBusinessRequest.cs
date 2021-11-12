using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK.Business
{
    // 内部请求不强迫实现对收到应答的数据进行处理
    internal abstract class InternalBusinessRequest : BusinessBaseRequest
    {
        public InternalBusinessRequest(string providerId, string msgId) : base(providerId, msgId)
        {
        }

        public override Dictionary<string, object> OnExceptionReturnData(System.Exception e)
        {
            Debug.LogError($"[TPFSDK] sdk internal error!");
            Debug.LogException(e);
            return null;
        }

        public override Dictionary<string, object> OnFailureReturnData(long code, string msg)
        {
            Debug.LogError($"[TPFSDK] http request fail! code = {code}, msg = {msg}");
            return null;
        }

        public override Dictionary<string, object> OnSuccessReturnData(string response)
        {
            Debug.Log($"[TPFSDK] http request success! response = {response}");
            return null;
        }

        public override void ProcessReturnData(Dictionary<string, object> data)
        {
        }
    }

}
