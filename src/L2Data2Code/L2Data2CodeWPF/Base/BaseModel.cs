using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace L2Data2CodeWPF.Base
{
    public class BaseModel : INotifyPropertyChanged, IBaseModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            SetProperty(ref field, newValue, null, propertyName);
        }

        public void SetProperty<T>(ref T field, T newValue, Action onChange, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                App.Logger.Trace($"{this.GetType().Name}.{propertyName} ({field}) = {newValue}");
                field = newValue;
                onChange?.Invoke();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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
