using System;
using System.Collections;
using System.Collections.Generic;
using TPFSDK.Utils;
using UnityEngine;

namespace TPFSDK.Business
{
    public static class CustomerServiceExtensions
    {
        internal const string PROVIDER_ID = "700005";
        
        public static void GetCustomerServiceConfig(this ITPFSdk sdk, TPFSdkEventDelegate callback)
        {
            var request = new GetCustomerServiceConfigRequest(callback);
            request.Post();
        }
    }

}

