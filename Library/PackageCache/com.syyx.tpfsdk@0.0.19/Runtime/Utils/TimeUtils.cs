﻿using System;

namespace TPFSDK.Utils
{
    public static class TimeUtils
    {
        public static long ToUnixTime(DateTime dateTime)
        {
            var t = dateTime.ToUniversalTime();
            return (t.Ticks - 621355968000000000) / 10000000;
        }

    }
} 
