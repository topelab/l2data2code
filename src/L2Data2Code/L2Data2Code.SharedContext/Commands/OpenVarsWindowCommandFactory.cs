using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Events;
using L2Data2CodeUI.Shared.Adapters;
using Prism.Events;
using System;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenVarsWindowCommandFactory : DelegateCommandFactory, IOpenVarsWindowCommandFactory
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IGeneratorAdapter adapter;

        public OpenVarsWindowCommandFactory(IEventAggregator eventAggregator, IGeneratorAdapter adapter)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        protected override void Execute()
        {
            eventAggregator.GetEvent<OpenVarsWindowEvent>().Publish(adapter.CompiledVars);
        }
    }
}
