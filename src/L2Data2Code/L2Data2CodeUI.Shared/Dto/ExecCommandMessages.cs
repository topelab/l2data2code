using L2Data2CodeUI.Shared.Localize;

namespace L2Data2CodeUI.Shared.Dto
{
    public class ExecCommandMessages
    {
        public string SlowMessage { get; set; }
        public string KillingMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string ContinueMessage { get; set; }

        public ExecCommandMessages(string name)
        {
            SlowMessage = string.Format(Messages.ParametrizedSlowMessage, name);
            KillingMessage = string.Format(Messages.ParametrizedKillingMessage, name);
            ErrorMessage = string.Format(Messages.ParametrizedErrorMessage, name);
            ContinueMessage = string.Format(Messages.ParametrizedContinueMessage, name);
        }
    }
}
