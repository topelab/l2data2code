using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace L2Data2Code.Base
{
    public class ViewModelBase : ReactiveObject
    {
        public void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            SetProperty(ref field, newValue, null, propertyName);
        }

        public void SetProperty<T>(ref T field, T newValue, Action onChange, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                this.RaisePropertyChanging(propertyName);
                field = newValue;
                onChange?.Invoke();
                this.RaisePropertyChanged(propertyName);
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }
    }
}
