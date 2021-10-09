using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace L2Data2CodeWPF.Base
{
    public interface IBaseModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null);
        void SetProperty<T>(ref T field, T newValue, Action onChange, [CallerMemberName] string propertyName = null);
        void OnPropertyChanged([CallerMemberName] string propertyName = null);
    }
}