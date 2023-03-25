using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Data2CodeWPF.Main
{
    internal interface IMainWindowVMBindManager
    {
        void Start(MainWindowVM mainWindowVM);
        void Stop();
    }
}
