using Core.Model;
using Core.SqlHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using IronOcr;

namespace CameraService
{
    public class CameraUtil
    {
        private const string api = "http://{0}/axis-cgi/temperature_alarm/getzonestatus.cgi?";
        private const string getSnapShot = "http://{0}/onvif/snapshot/0";

        public Queue<TemperatureResponse> MainDataQueue = new Queue<TemperatureResponse>();
        public Dictionary<int, TemperatureResponse> LiveViewQueue = new Dictionary<int, TemperatureResponse>();

        private Object lockQueue = new object();
        private System.Threading.Thread threadGetTemperature = null;
        private System.Threading.Thread threadSaveDB = null;
        private bool isStart = false;

        #region Instance
        private static CameraUtil instance;
        private CameraUtil()
        {
            threadGetTemperature = new System.Threading.Thread(() =>
            {
                GetThermalInfor();
            });
            threadGetTemperature.IsBackground = true;

            threadSaveDB = new System.Threading.Thread(() =>
            {
                SaveToDB();
            });
            threadSaveDB.IsBackground = true;

        }

        public static CameraUtil Instalce
        {
            get
            {
                if (instance == null)
                    instance = new CameraUtil();
                return instance;
            }
        }
        #endregion
        public int Interval { set; get; } = 60000;//60 mini second
        public SqlHelper SqlHelper { set; get; } = new SqlHelper();
        public List<SiteCamera> SiteCameras { get; set; } = new List<SiteCamera>();

        private List<CameraZone> cameraZones = new List<CameraZone>();
        public List<CameraZone> CameraZones
        {
            get
            {
                if (SiteCameras != null && cameraZones.Count <= 0)
                    cameraZones = SiteCameras.SelectMany(p => p.CameraZones).ToList();
                return cameraZones;
            }
        }
        public TemperatureResponse CurrentTemperatureResponse { get; set; } = new TemperatureResponse();

        #region Download Thermal information
        private async void GetDataByCamera(Camera camera)
        {
            string xmlStr;

            using (var client = new WebClient() { Credentials = new NetworkCredential(camera.User, camera.Password) })
            {
                try
                {
                    xmlStr = await client.DownloadStringTaskAsync(string.Format(api, camera.IPAddress));
                    if (!string.IsNullOrEmpty(xmlStr))
                    {
                        MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlStr));
                        TemperatureResponse data = Core.Util.Serializable.Deserialize<TemperatureResponse>(memStream);
                        if (data != null)
                        {
                            data.Success.CameraId = camera.CameraId;
                            data.Success.DateTime = DateTime.Now;
                            CameraZone cameraZone = CameraZones.FirstOrDefault(p => p.CameraId == camera.CameraId);

                            foreach (var item in data.Success.GetZoneStatusSuccess)
                            {
                                item.IsAlarm = cameraZone.Zones.Any(p => p.ZoneId == item.ZoneId && p.MaxAlarmTemperature <= item.AverageTemperature);
                            }
                            lock (lockQueue)
                            {
                                MainDataQueue.Enqueue(data);

                                if (LiveViewQueue != null)
                                {
                                    if (!LiveViewQueue.ContainsKey(camera.CameraId))
                                        LiveViewQueue.Add(camera.CameraId, data);
                                    else
                                        LiveViewQueue[camera.CameraId] = data;
                                }
                                //Core.Log.LogManager.Info("Infor done ");
                            }
                        }
                    }

                    //TemperatureResponse data = CreateDummyData();
                    //if (data != null)
                    //{
                    //    data.Success.CameraId = camera.CameraId;
                    //    data.Success.DateTime = DateTime.Now;
                    //    Core.Log.LogManager.Info("**********datetime" + data.Success.DateTime.ToString());
                    //    lock (lockQueue)
                    //    {
                    //        MainDataQueue.Enqueue(data);
                    //    }
                    //    if (LiveViewQueue != null)
                    //    {
                    //        if (!LiveViewQueue.ContainsKey(camera.CameraId))
                    //            LiveViewQueue.Add(camera.CameraId, data);
                    //        else
                    //            LiveViewQueue[camera.CameraId] = data;
                    //        Core.Log.LogManager.Info("Infor done ");
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Core.Log.LogManager.Info("Exception GetDataByCamera : " + ex.Message);
                }
            }
        }

        private async void GetDataByCameraOnvif(Camera camera)
        {

            using (var client = new WebClient())
            {
                try
                {
                    Stream imageStream = client.OpenRead(string.Format(getSnapShot, camera.IPAddress));
                    if (imageStream != null)
                    {
                        double temperature = GetTextByImageStream(imageStream);
                        TemperatureResponse data = new TemperatureResponse() { Success = new Success() };

                        data.Success.CameraId = camera.CameraId;
                        data.Success.DateTime = DateTime.Now;
                        data.Success.GetZoneStatusSuccess = new List<AlarmZone>() { new AlarmZone() { ZoneId = 1, ZoneName = "Zone 1", AverageTemperature = temperature, MinimumTemperature=0,MaximumTemperature=0 } };
                        CameraZone cameraZone = CameraZones.FirstOrDefault(p => p.CameraId == camera.CameraId);

                        lock (lockQueue)
                        {
                            MainDataQueue.Enqueue(data);

                            if (LiveViewQueue != null)
                            {
                                if (!LiveViewQueue.ContainsKey(camera.CameraId))
                                    LiveViewQueue.Add(camera.CameraId, data);
                                else
                                    LiveViewQueue[camera.CameraId] = data;
                            }
                            //Core.Log.LogManager.Info("Infor done ");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Core.Log.LogManager.Info("Exception GetDataByCamera : " + ex.Message);
                }
            }
        }

        private double GetTextByImageStream(Stream stream)
        {
            double temperature = 0;
            IronTesseract ocr = new IronTesseract();
            // Configure for speed
            ocr.Configuration.BlackListCharacters = "~`$#^*_}{][|\\";
            ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.Auto;
            ocr.Language = OcrLanguage.EnglishFast;
            using (OcrInput input = new OcrInput(stream))
            {
                OcrResult result = ocr.Read(input);
                stream.Dispose();
                if (result != null)
                {
                    var split = result.Text.Split(':');
                    if (split !=null && split.Length>1)
                    {
                        temperature =double.Parse(split[0].Split(' ')[0]);
                    }
                }   
            }
            return temperature;
        }
        private async void GetThermalInfor()
        {
            var cameraZones = SiteCameras.SelectMany(p => p.CameraZones).ToList();
            while (isStart)
            {
                foreach (var camera in cameraZones)
                {
                    if (camera.Model == "Onvif")
                        GetDataByCameraOnvif(camera);
                    else
                        GetDataByCamera(camera);
                }
                await Task.Delay(Interval);
            }
        }
        #endregion

        #region Save data to databse
        public async void SaveToDB()
        {

            CoreParameter[] coreParameter = new CoreParameter[7];
            string sqlQuery = string.Empty;
            while (isStart)
            {
                try
                {
                    if (MainDataQueue != null && MainDataQueue.Count > 0)
                    {
                        var data = MainDataQueue.Dequeue();
                        if (data != null && data.Success != null)
                        {
                            foreach (var item in data.Success.GetZoneStatusSuccess)
                            {
                                sqlQuery = "InsertThermalCameraInfo";
                                coreParameter[0] = new CoreParameter("@cameraId", data.Success.CameraId);
                                coreParameter[1] = new CoreParameter("@zoneId", item.ZoneId);
                                coreParameter[2] = new CoreParameter("@averageTemperature", item.AverageTemperature);
                                coreParameter[3] = new CoreParameter("@maximumTemperature", item.MaximumTemperature);
                                coreParameter[4] = new CoreParameter("@minimumTemperature", item.MinimumTemperature);
                                coreParameter[5] = new CoreParameter("@datetime", data.Success.DateTime);
                                coreParameter[6] = new CoreParameter("@isAlarm", item.IsAlarm);
                                SqlHelper.ExecuteStore(sqlQuery, coreParameter);
                            }
                        }
                    }
                    await Task.Delay(Interval / 2);
                }
                catch (Exception ex)
                {
                    Core.Log.LogManager.Info("Exception save data to database : " + ex.Message);
                }
            }
        }

        public List<TemperatureInfo> GetData(string cameraId, DateTime startDate, DateTime endDate, int type)
        {
            List<TemperatureInfo> results = new List<TemperatureInfo>();
            try
            {
                CoreParameter[] coreParameter = new CoreParameter[4];
                string sqlQuery = string.Empty;
                coreParameter[0] = new CoreParameter("@cameraId", cameraId);
                coreParameter[1] = new CoreParameter("@startDate", startDate);
                coreParameter[2] = new CoreParameter("@endDate", endDate);
                coreParameter[3] = new CoreParameter("@type", type);
                DataTable data = SqlHelper.LoadTableFromStoreProcedure("GetDataThermalCameraInfo", coreParameter);
                results = Core.Util.Serializable.ConvertDataTable<TemperatureInfo>(data);
                //Core.Log.LogManager.Info("Data count: " + results.Count);
                //Core.Log.LogManager.Info("***** camera: " + cameraId);
                return results;
            }
            catch (Exception ex)
            {
                Core.Log.LogManager.Info("Exception Get data from database  : " + ex.Message);
                return null;
            }
            //finally
            //{
            //    results.Clear();
            //    results = null;
            //    GC.Collect();
            //}
        }


        private TemperatureResponse CreateDummyData()
        {
            Random ran = new Random();
            return new TemperatureResponse()
            {
                Success = new Success()
                {
                    CameraId = 1,
                    DateTime = DateTime.Now,
                    GetZoneStatusSuccess = new List<AlarmZone>() { new AlarmZone() { ZoneId = 0, ZoneName = "zone 1", AverageTemperature = ran.Next(22, 40) },
                            new AlarmZone() { ZoneId = 1, ZoneName = "zone 2", AverageTemperature = ran.Next(20, 40), IsAlarm = ran.Next(20, 40) <25?true:false },
                            new AlarmZone() { ZoneId = 2, ZoneName = "zone 3", AverageTemperature = ran.Next(21, 35),IsAlarm = ran.Next(20, 40) <25?true:false },
                            new AlarmZone() { ZoneId = 3, ZoneName = "zone 4", AverageTemperature = ran.Next(30, 40),IsAlarm = ran.Next(20, 40) <25?true:false },
                            new AlarmZone() { ZoneId = 4, ZoneName = "zone 5", AverageTemperature = ran.Next(25, 36),IsAlarm = ran.Next(20, 40) <25?true:false },
                            new AlarmZone() { ZoneId = 5, ZoneName = "zone 6", AverageTemperature = ran.Next(20, 38),IsAlarm = ran.Next(20, 40) <25?true:false }}
                }
            };
        }


        #endregion

        public void Stop()
        {
            isStart = false;
            if (threadGetTemperature != null)
                threadGetTemperature.Abort();
            if (threadSaveDB != null)
                threadSaveDB.Abort();
            MainDataQueue.Clear();
            LiveViewQueue.Clear();
        }

        public void Start()
        {
            isStart = true;
            threadGetTemperature.Start();
            threadSaveDB.Start();
        }
    }
}