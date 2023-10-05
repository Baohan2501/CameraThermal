using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log
{
    public static class LogManager
    {
        private static ILogger log = new LogWithLog4Net();

        public static void LoadConfigure(FileInfo fileInfo)
        {
            log.LoadConfigure(fileInfo);
        }

        public static void Debug(string message, params string[] param)
        {
            log.Debug(message, param);
        }

        public static void Debug(List<string> message, params string[] param)
        {
            log.Debug(message, param);
        }

        public static void Error(List<string> message, params string[] param)
        {
            log.Error(message, param);
        }

        public static void Error(string message, params string[] param)
        {
            log.Error(message, param);
        }

        public static void Error(Exception ex, string message = "")
        {
            var str = string.Empty;
            str += message + " Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
            log.Error(str);
        }

        public static void Warn(List<string> message, params string[] param)
        {
            log.Warn(message, param);
        }

        public static void Warn(string message, params string[] param)
        {
            log.Warn(message, param);
        }

        public static void Fata(List<string> message, params string[] param)
        {
            log.Fata(message, param);
        }

        public static void Fatal(string message, params string[] param)
        {
            log.Fatal(message, param);
        }

        public static void Info(List<string> message, params string[] param)
        {
            log.Info(message, param);
        }


        public static void Info(string message, params string[] param)
        {
            log.Info(message, param);
        }
    }
}
