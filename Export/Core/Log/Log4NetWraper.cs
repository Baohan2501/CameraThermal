using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log
{
    public static class Log4NetWraper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void LoadConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
        }

        public static void Fatal(List<string> message, params string[] param)
        {
            Fatal(message.ToString(), param);
        }


        public static void Fatal(string message, params string[] param)
        {

            if (param.Length > 0)
            {
                message = string.Format(message, param);
            }

            log.Fatal(message);
        }


        public static void Error(List<string> message, params string[] param)
        {
            Error(message.ToString(), param);
        }


        public static void Error(string message, params string[] param)
        {

            if (param.Length > 0)
            {
                message = string.Format(message, param);
            }

            log.Error(message);
        }


        public static void Warn(List<string> message, params string[] param)
        {
            Warn(message.ToString(), param);
        }


        public static void Warn(string message, params string[] param)
        {

            if (param.Length > 0)
            {
                message = string.Format(message, param);
            }

            log.Warn(message);
        }


        public static void Info(List<string> message, params string[] param)
        {
            Info(message.ToString(), param);
        }


        public static void Info(string message, params string[] param)
        {

            if (param.Length > 0)
            {
                message = string.Format(message, param);
            }

            log.Info(message);
        }


        public static void Debug(List<string> message, params string[] param)
        {
            Debug(message.ToString(), param);
        }


        public static void Debug(string message, params string[] param)
        {
            if (param.Length > 0)
            {
                message = string.Format(message, param);
            }

            log.Debug(message);
        }

    }
}
