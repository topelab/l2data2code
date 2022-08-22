using System.ComponentModel;

namespace L2Data2CodeWPF.Base
{
    public class BaseControlVM : BaseVM
    {
        protected readonly IBaseVM baseVM;

        public override bool Working
        {
            get { return baseVM.Working; }
            set { baseVM.Working = value; }
        }

        public BaseControlVM()
        {

        }

        public BaseControlVM(IBaseVM baseVM)
        {
            this.baseVM = baseVM;
            this.baseVM.PropertyChanged += BaseVM_PropertyChanged;
        }

        protected virtual void BaseVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IBaseVM.Working):
                    OnPropertyChanged(nameof(Working));
                    break;
                default:
                    break;
            }
        }
    }
}
