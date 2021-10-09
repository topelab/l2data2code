using System.ComponentModel;

namespace L2Data2CodeWPF.Base
{
    public class BaseControlViewModel : BaseViewModel, INotifyPropertyChanged, IBaseViewModel
    {
        protected readonly IBaseViewModel baseViewModel;

        public override bool Working
        {
            get { return baseViewModel.Working; }
            set { baseViewModel.Working = value; }
        }

        public BaseControlViewModel()
        {

        }

        public BaseControlViewModel(IBaseViewModel baseViewModel)
        {
            this.baseViewModel = baseViewModel;
            this.baseViewModel.PropertyChanged += BaseViewModel_PropertyChanged;
        }

        protected virtual void BaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IBaseViewModel.Working):
                    OnPropertyChanged(nameof(Working));
                    break;
                default:
                    break;
            }
        }
    }
}
