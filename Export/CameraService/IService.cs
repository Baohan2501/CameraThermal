using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace CameraService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        bool ConnectDevice(string ipAddress, string user, string password);

        [OperationContract]
        TemperatureResponse GetDataLive(int cameraId);

        [OperationContract]
        List<TemperatureInfo> GetData(string cameraId, DateTime startDate, DateTime endDate,int type);

        [OperationContract]
        string GetSiteCameras();

        [OperationContract]
        string GetSqlConnection();

        [OperationContract]
        void Stop();

        [OperationContract]
        void Start(List<SiteCamera> siteCameras, string connectionSql,int interval);

    }

}
