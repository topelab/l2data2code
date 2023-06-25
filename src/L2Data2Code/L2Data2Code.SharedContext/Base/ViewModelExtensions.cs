using System;

namespace L2Data2Code.SharedContext.Base
{
    public static class ViewModelExtensions
    {
        public static void WorkOnAction(this ViewModelBase @this, Action action)
        {
            @this.Working = true;
            action?.Invoke();
            @this.Working = false;
        }
    }
}
