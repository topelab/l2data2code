using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConvertTemplates
{
    internal class ChangeInfo
    {
        public Regex Search { get; set; }
        public string Replace { get; set; }
    }
}
