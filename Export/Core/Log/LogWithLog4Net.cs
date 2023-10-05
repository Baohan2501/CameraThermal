using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log
{
    public class LogWithLog4Net : ILogger
    {
        public void LoadConfigure(FileInfo fileInfo)
        {
            Log4NetWraper.LoadConfig(fileInfo);
        }

        public void Debug(List<string> message, params string[] param)
        {
            Log4NetWraper.Debug(message, param);
        }

        public void Debug(string message, params string[] param)
        {
            Log4NetWraper.Debug(message, param);
        }

        public void Error(List<string> message, params string[] param)
        {
            Log4NetWraper.Error(message, param);
        }

        public void Error(string message, params string[] param)
        {
            Log4NetWraper.Error(message, param);
        }

        public void Warn(List<string> message, params string[] param)
        {
            Log4NetWraper.Warn(message, param);
        }

        public void Warn(string message, params string[] param)
        {
            Log4NetWraper.Warn(message, param);
        }

        public void Fata(List<string> message, params string[] param)
        {
            Log4NetWraper.Fatal(message, param);
        }

        public void Fatal(string message, params string[] param)
        {
            Log4NetWraper.Fatal(message, param);
        }

        public void Info(List<string> message, params string[] param)
        {
            Log4NetWraper.Info(message, param);
        }

        public void Info(string message, params string[] param)
        {
            Log4NetWraper.Info(message, param);
        }

    }
}
