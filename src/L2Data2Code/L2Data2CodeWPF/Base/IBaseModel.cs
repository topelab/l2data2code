using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace L2Data2CodeWPF.Base
{
    public interface IBaseModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null);
        bool SetProperty<T>(ref T field, T newValue, Action onChange, [CallerMemberName] string propertyName = null);
        void OnPropertyChanged([CallerMemberName] string propertyName = null);
    }
}