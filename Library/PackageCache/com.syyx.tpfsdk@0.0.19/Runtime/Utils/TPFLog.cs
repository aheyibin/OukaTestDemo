using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK
{
    public class TPFLog
    {
        public TPFLog() { }

        //[Conditional("LUNAR_DEVELOPMENT")]
        public static void dev(string format, params object[] args) { }
        public static void e(string message) { }
        public static void e(Exception exception) { }
        public static void e(string format, params object[] args) { }
        public static void e(Exception exception, string message) { }
        public static void e(Exception exception, string format, params object[] args) { }
        public static void EnableOutput(LogOutput output, bool value) { }
        public static void Error(object obj) { }
        public static void Error(string fmt, params object[] param) { }
        public static void ErrorObj(object obj) { }
        public static void Exception(Exception e) { }
        public static void Info(object obj) { }
        public static void Info(string fmt, params object[] param) { }
        public static void InfoObj(object obj) { }
        public static void SetLogLevel(LogLevel level) { }
        public static void w(string message) { }
        public static void w(string format, params object[] args) { }
        public static void Warning(object obj) { }
        public static void Warning(string fmt, params object[] param) { }
        public static void WarningObj(object obj) { }

        public enum LogLevel
        {
            INFO = 0,
            WARNING = 1,
            ERROR = 2
        }
        public enum LogOutput
        {
            DebugConsole = 1,
            GUIConsole = 2,
            File = 4
        }
    }
}