using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace GFW
{
    public class Logger
    {
        public static int logLevel = 0;

        private static long lastLogTime;

        public const int LEVEL_TEMP = 0;
        public const int LEVEL_TEST = 1;
        public const int LEVEL_LOG = 2;
        public const int LEVEL_WARN = 3;

        private static void LogWithColor(string message, Color color, int level)
        {
#if UNITY_ANDROID
             if (level >= LEVEL_LOG)
            {
                if (LogTcpManager.Instance != null)
                    LogTcpManager.Instance.AddLog(message);
            }
#endif


            if (level < logLevel) return;

            int colorInt = ((int)(color.r * 255f) << 16) | ((int)(color.g * 255f) << 8) | (int)(color.b * 255f);
            long nowTime = DateTime.Now.Ticks / 10000; //ms
            long offsetTime = lastLogTime > 0 ? nowTime - lastLogTime : 0;
            lastLogTime = nowTime;

            StringBuilder sb = new StringBuilder();
            sb.Append("<color=#")
                .Append(colorInt.ToString("X6"))
                .Append(">")
                .Append("[")
                .Append(offsetTime)
                .Append("]")
                .Append("-->")
                .Append(message)
                .Append("</color>");

            UnityEngine.Debug.Log(sb.ToString());
        }

        public static void Log(string message)
        {
            LogWithColor(message, Color.green, LEVEL_LOG);
        }

        public static void LogError(string message)
        {
            long nowTime = DateTime.Now.Ticks / 10000; //ms
            long offsetTime = lastLogTime > 0 ? nowTime - lastLogTime : 0;
            lastLogTime = nowTime;
#if UNITY_ANDROID
             StackTrace st = new StackTrace(true);
            UnityEngine.Debug.LogError("[" + offsetTime + "]-->" + message + st.ToString());
            return;
#endif         
            UnityEngine.Debug.LogError("[" + offsetTime + "]-->" + message);
        }

        public static void LogWarn(string message)
        {
            LogWithColor(message, Color.yellow, LEVEL_WARN);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogTest(string message)
        {
#if UNITY_EDITOR
            LogWithColor(message, Color.grey, LEVEL_TEST);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogTemp(string message)
        {
#if UNITY_EDITOR
            LogWithColor(message, Color.white, LEVEL_TEMP);
#endif

        }
    }
}