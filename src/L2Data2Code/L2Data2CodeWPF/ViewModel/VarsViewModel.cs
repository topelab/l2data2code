using L2Data2CodeWPF.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace L2Data2CodeWPF.ViewModel
{
    public class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class VarsViewModel : BaseViewModel
    {
        private ObservableCollection<Variable> _vars = new();

        public ObservableCollection<Variable> Variables
        {
            get { return _vars; }
            set { SetProperty(ref _vars, value); }
        }

        public VarsViewModel(Dictionary<string, object> valuePairs)
        {
            foreach (var item in valuePairs)
            {
                _vars.Add(new Variable { Name = item.Key, Value = item.Value?.ToString() });
            }
            OnPropertyChanged(nameof(Variables));
        }

    }
}
