using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base
{
    public class WindowBase : MetroWindow
    {
        public WindowBase()
            : base()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            WindowState = System.Windows.WindowState.Normal;
        }
    }
}
