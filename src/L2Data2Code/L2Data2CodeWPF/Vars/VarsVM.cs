using L2Data2CodeWPF.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace L2Data2CodeWPF.Vars
{
    public class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class VarsVM : BaseVM
    {
        private ObservableCollection<Variable> _vars = new();

        public ObservableCollection<Variable> Variables
        {
            get { return _vars; }
            set { SetProperty(ref _vars, value); }
        }

        public VarsVM(Dictionary<string, object> valuePairs)
        {
            foreach (var item in valuePairs)
            {
                _vars.Add(new Variable { Name = item.Key, Value = item.Value?.ToString() });
            }
            OnPropertyChanged(nameof(Variables));
        }

    }
}