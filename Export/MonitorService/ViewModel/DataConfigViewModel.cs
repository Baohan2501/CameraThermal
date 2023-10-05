using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorService.ViewModel
{
    public class DataConfigViewModel:ViewModelBaseExtend
    {

        #region Property
        private object currentItem;
        public object CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }

        #endregion

        #region Contructor
        public DataConfigViewModel()
        {
        }

        #endregion

        #region ICommand
        


        #endregion

        #region Public function

        #endregion

        #region Private function
       

        #endregion
    }
}
