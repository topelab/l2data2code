using System.ComponentModel;

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
