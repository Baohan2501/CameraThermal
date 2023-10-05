using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CameraService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service : IService
    {
        public Service()
        {
        }

        #region Intserface service
        public bool ConnectDevice(string ipAddress, string user, string password)
        {
            throw new NotImplementedException();
        }

        public string GetSiteCameras()
        {
            return JsonConvert.SerializeObject(CameraUtil.Instalce.SiteCameras);
        }

        public List<TemperatureInfo> GetData(string cameraId, DateTime startDate, DateTime endDate, int type)
        {
            return CameraUtil.Instalce.GetData(cameraId, startDate, endDate, type);
        }

        public TemperatureResponse GetDataLive(int cameraId)
        {
            try
            {
                TemperatureResponse data = null;
                if (CameraUtil.Instalce.LiveViewQueue != null)
                {
                    //Core.Log.LogManager.Info("start get data live");
                    if (CameraUtil.Instalce.LiveViewQueue.ContainsKey(cameraId))
                        data = CameraUtil.Instalce.LiveViewQueue[cameraId];
                    if (data.GuiID != CameraUtil.Instalce.CurrentTemperatureResponse.GuiID)
                    {
                        CameraUtil.Instalce.CurrentTemperatureResponse = data;
                        //Core.Log.LogManager.Info("***********data time: " + data.Success.DateTime.ToString());
                    }
                    else
                        data = null;
                    //Core.Log.LogManager.Info("start get data done : ");
                }
                //return data != null ? JsonConvert.SerializeObject(data) : null;
                return data;
            }
            catch (Exception ex)
            {
                Core.Log.LogManager.Error("Live view: " + ex.Message);
                return null;
            }

        }
        public void Start(List<SiteCamera> siteCameras, string connectionSql, int interval)
        {
            CameraUtil.Instalce.SiteCameras = siteCameras;
            CameraUtil.Instalce.SqlHelper = new Core.SqlHelper.SqlHelper(connectionSql);
            CameraUtil.Instalce.Interval = interval;
            Core.Log.LogManager.Info("Interval : " + interval.ToString());
            Core.Log.LogManager.Info("Connection : " + connectionSql);
            CameraUtil.Instalce.Start();
        }

        public void Stop()
        {
            CameraUtil.Instalce.Stop();
        }

        public string GetSqlConnection()
        {
            return CameraUtil.Instalce.SqlHelper.ConnectionString;
        }

        #endregion
    }
}
