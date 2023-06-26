using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Events;
using L2Data2CodeUI.Shared.Adapters;
using Prism.Events;
using System;

namespace L2Data2Code.SharedContext.Commands
{
    public class OpenVarsWindowCommand : ReactiveBaseCommand, IOpenVarsWindowCommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IGeneratorAdapter adapter;

        public OpenVarsWindowCommand(IEventAggregator eventAggregator, ICommandManager commandManager, IGeneratorAdapter adapter) : base(commandManager)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public override void Execute(object parameter)
        {
            eventAggregator.GetEvent<OpenVarsWindowEvent>().Publish(adapter.CompiledVars);
        }
    }
}
