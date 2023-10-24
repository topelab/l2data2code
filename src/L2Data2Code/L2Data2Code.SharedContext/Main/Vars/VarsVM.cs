using L2Data2Code.SharedContext.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace L2Data2Code.SharedContext.Main.Vars
{
    public class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class VarsVM : ViewModelBase
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
