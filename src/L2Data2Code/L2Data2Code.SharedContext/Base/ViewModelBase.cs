using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace L2Data2Code.SharedContext.Base
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private bool _working;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        public virtual bool Working
        {
            get { return _working; }
            set { SetProperty(ref _working, value); }
        }

        public void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            SetProperty(ref field, newValue, null, propertyName);
        }

        public void SetProperty<T>(ref T field, T newValue, Action onChange, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
                field = newValue;
                onChange?.Invoke();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
