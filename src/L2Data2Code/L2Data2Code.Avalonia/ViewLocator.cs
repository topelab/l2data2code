using Avalonia.Controls;
using Avalonia.Controls.Templates;
using L2Data2Code.SharedContext.Base;
using System;

namespace L2Data2Code
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object data)
        {
            var fullName = data.GetType().FullName!;
            var name = fullName.EndsWith("VM") ? fullName.Replace("VM", "View") : fullName.Replace("ViewModel", "View");
#pragma warning disable IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.
            var type = Type.GetType(name);
#pragma warning restore IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
