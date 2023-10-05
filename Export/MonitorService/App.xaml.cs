using Core.Base;
using Core.Log;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MonitorService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex = null;
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        MainWindow frm = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "MyAppName";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
                Environment.Exit(1);

            // get the current app style (theme and accent) from the application
            // you can then use the current theme and custom accent instead set a new theme
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent("Steel"),
                                        ThemeManager.GetAppTheme("BaseLight")); // or appStyle.Item1

            string configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            FileInfo configFileInfo = new FileInfo(configPath);
            LogManager.LoadConfigure(configFileInfo);

            ////check service 
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "WindowThermalCameraService");
            if (ctl == null)
            {
                RegisterService();

                View_IsActivated();
            }
            else
            {
                View_IsActivated();
            }
        }

        private void View_IsActivated()
        {
            this.nIcon.Icon = new System.Drawing.Icon(@"workgroup.ico");
            this.nIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.nIcon.ContextMenuStrip.Items.Add("Exit", null, this.ExitService);
            nIcon.Visible = true;
            nIcon.Text = "Monitoring thermal camera service";
            this.nIcon.DoubleClick += NIcon_DoubleClick; 
        }

        private void NIcon_DoubleClick(object sender, EventArgs e)
        {

            WindowBase form= Application.Current.Windows.OfType<WindowBase>().FirstOrDefault(x => x.GetType().Name.Equals("MainWindow"));
            if (form != null)
            {
                form.Visibility = Visibility.Visible;
                form.Activate();
            }
            else
            {
                frm = new MainWindow();
                frm.ShowDialog();
            }

        }

        private void RegisterService()
        {
            string batfile = Path.Combine(Environment.CurrentDirectory, "Service\\Install.bat");
            var process = new Process();
            var psi = new ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = @"cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + batfile;
            process.StartInfo = psi;
            process.Start();
            process.WaitForExit();
        }
        private void ExitService(object obj, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
