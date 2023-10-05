using Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsCameraService
{
    public partial class Service : ServiceBase
    {
        ServiceHost s = null;
        CameraService.IService service = null;
        ArrayOfCamera data = null;
        public Service()
        {
            InitializeComponent();
            data = Core.Util.Serializable.Deserialize<ArrayOfCamera>(System.IO.Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", ""), "Camera/DataCamerasNew.xml"));
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (s != null)
                {
                    s.Close();
                }
                WSHttpBinding binding = new WSHttpBinding();
                //binding.Security = new WSHttpSecurity { Mode = SecurityMode.None };

                //binding.Security.Transport = new HttpTransportSecurity { ClientCredentialType = HttpClientCredentialType.None };
                binding.MaxBufferPoolSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.ReceiveTimeout = TimeSpan.FromHours(1);
                binding.SendTimeout = TimeSpan.FromHours(1);
                binding.ReaderQuotas.MaxArrayLength = 2147483647;
                binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
                binding.ReaderQuotas.MaxDepth = 2147483647;
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;

                Uri httpUrl = new Uri("http://localhost:9988/Service.svc");
                s = new ServiceHost(typeof(CameraService.Service), httpUrl);
                s.AddServiceEndpoint(typeof(CameraService.IService), binding, "");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();

                smb.HttpGetEnabled = true;
                //smb.HttpsGetEnabled = true;
                s.Description.Behaviors.Add(smb);
                s.Open();
                while (s.State == CommunicationState.Opened)
                {
                    try
                    {
                        if (data != null)
                        {
                            string connection = string.Format("Server={0};Database={1};User Id={2};Password={3};MultipleActiveResultSets = true;", data.Server, data.DBName, data.User, data.Password);
                            Core.Log.LogManager.Debug(connection);
                            service = new CameraService.Service();
                            service.Start(data.SiteCameras, connection, data.Interval);
                            Core.Log.LogManager.Debug("Service Started");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Log.LogManager.Debug(ex.Message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log.LogManager.Debug("Service start :"+ ex.Message);
                throw;
            }
            
        }

        protected override void OnStop()
        {
            if (s != null)
            {
                s.Close();
                s = null;
            }
        }
    }
}
