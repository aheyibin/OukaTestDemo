using System;
//using XLua;

namespace TPFSDK.Utils
{
    //[ReflectionUse]
    public static class ActionExtensions
    {
        public static void SafeInvoke<T>(this Action<T> act, T param)
        {
            try
            {
                if (act != null)
                {
                    act(param);
                }
            }
            catch (Exception e)
            {
                TPFLog.Exception(e);
            }
        }

        public static void SafeInvoke<T1, T2>(this Action<T1, T2> act, T1 p1, T2 p2)
        {
            try
            {
                if (act != null)
                {
                    act(p1, p2);
                }
            }
            catch (Exception e)
            {
                TPFLog.Exception(e);
            }
        }

        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> act, T1 p1, T2 p2, T3 p3)
        {
            try
            {
                if (act != null)
                {
                    act(p1, p2, p3);
                }
            }
            catch (Exception e)
            {
                TPFLog.Exception(e);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> act, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            try
            {
                if (act != null)
                {
                    act(p1, p2, p3, p4);
                }
            }
            catch (Exception e)
            {
                TPFLog.Exception(e);
            }
        }
    }
}