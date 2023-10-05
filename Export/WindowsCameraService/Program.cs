using Core.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsCameraService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ////Load config log4net
            string configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            FileInfo configFileInfo = new FileInfo(configPath);
            LogManager.LoadConfigure(configFileInfo);
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
