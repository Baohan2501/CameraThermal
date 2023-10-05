using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log
{
    public interface ILogger
    {
        void LoadConfigure(FileInfo fileInfo);
        void Debug(List<string> message, params string[] param);
        void Debug(string message, params string[] param);
        void Error(List<string> message, params string[] param);
        void Error(string message, params string[] param);
        void Warn(List<string> message, params string[] param);
        void Warn(string message, params string[] param);
        void Fata(List<string> message, params string[] param);
        void Fatal(string message, params string[] param);
        void Info(List<string> message, params string[] param);
        void Info(string message, params string[] param);
    }
}
