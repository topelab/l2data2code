using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using L2Data2Code.SharedLib.Configuration;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Base;

namespace L2Data2CodeWPF.ViewModel
{
    public class MainWindowViewModelDesignTime : BaseViewModel
    {
        public string SelectedTemplate { get; set; }
        public string SelectedArea { get; set; }
        public string SelectedVars { get; set; }
        public string SelectedModule { get; set; }
        public IEnumerable<string> TemplateList { get => new List<string> { "Template 1", "Template 2" }; }
        public IEnumerable<string> AreaList { get => new List<string> { "Area 1", "Area 2" }; }
        public IEnumerable<string> VarsList { get => new List<string> { "Vars 1", "Vars 2" }; }
        public IEnumerable<string> ModuleList { get => new List<string> { "Module 1", "Module 2" }; }
        public bool VarsVisible => true;


        public ObservableCollection<TableViewModel> AllTables { get; set; } = new ObservableCollection<TableViewModel>();

        public MainWindowViewModelDesignTime()
        {
        }

    }
}
