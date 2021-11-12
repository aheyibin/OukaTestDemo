using System.Collections.Generic;

namespace TPFSDK.Business 
{
    /// <summary>
    /// 需要以 TPFSdk Event 形式返回给上层的请求
    /// </summary>
    internal abstract class ExternalBusinessRequest : BusinessBaseRequest
    {
        private TPFSdkEventDelegate m_Callback;
        private int m_RequestFailCode;
        public ExternalBusinessRequest(string providerId, string msgId, int requestFailTPFCode, TPFSdkEventDelegate callback) : base(providerId, msgId)
        {
            m_Callback = callback;
            m_RequestFailCode = requestFailTPFCode;
        }

        public override Dictionary<string, object> OnExceptionReturnData(System.Exception e)
        {
            return BusinessUtils.WarpRequestExceptionParam(m_RequestFailCode);
        }

        public override Dictionary<string, object> OnFailureReturnData(long code, string msg)
        {
            return BusinessUtils.WarpRequestFailureParam(m_RequestFailCode, code, msg);
        }

        public sealed override void ProcessReturnData(Dictionary<string, object> data)
        {
            m_Callback.BusinessInvoke(ref data);
        }

    }

}
