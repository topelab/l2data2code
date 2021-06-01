using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace L2Data2CodeWPF.Base
{
    public class BaseViewModel : BaseModel, INotifyPropertyChanged, IBaseViewModel
    {
        private bool _working;

        public virtual bool Working
        {
            get { return _working; }
            set { SetProperty(ref _working, value); }
        }
    }
}
