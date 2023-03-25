using L2Data2Code.Main.Interfaces;
using System.Collections.Generic;

namespace L2Data2Code.Main
{
    internal class MainWindowInitializer : IMainWindowInitializer
    {
        public void Initialize(MainWindowViewModel viewModel)
        {
            viewModel.TemplateList = new List<string>() { "Hola", "Amigo" };

        }
    }
}
