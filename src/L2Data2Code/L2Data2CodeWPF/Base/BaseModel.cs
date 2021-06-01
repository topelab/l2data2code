using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace L2Data2CodeWPF.Base
{
    public class BaseModel : INotifyPropertyChanged, IBaseModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(ref field, newValue, null, propertyName);
        }

        public bool SetProperty<T>(ref T field, T newValue, Action onChange, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                App.Logger.Trace($"{this.GetType().Name}.{propertyName} ({field}) = {newValue}");
                field = newValue;
                onChange?.Invoke();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
