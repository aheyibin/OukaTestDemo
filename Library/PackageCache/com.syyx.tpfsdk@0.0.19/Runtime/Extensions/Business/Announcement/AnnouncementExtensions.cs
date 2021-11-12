using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TPFSDK.Utils;
using UnityEngine;

namespace TPFSDK.Business
{
    public static class AnnouncementExtensions 
    {
        internal const string MESSAGE_ANNOUNCEMENT_PROVIDERID = "700016";

        public static void GetAnnouncements(this ITPFSdk sdk, TPFSdkInfo info, TPFSdkEventDelegate callback)
        {
            var request = new GetAnnouncementsRequest(info, callback);
            request.Post();
        }

    }

}
