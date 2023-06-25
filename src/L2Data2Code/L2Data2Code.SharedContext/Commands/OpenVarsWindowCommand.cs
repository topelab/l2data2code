using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Events;
using L2Data2Code.SharedContext.Main;
using L2Data2Code.SharedContext.Main.Vars;
using Prism.Events;
using System;

namespace L2Data2Code.SharedContext.Commands
{
    internal class OpenVarsWindowCommand : ReactiveBaseCommand, IOpenVarsWindowCommand
    {
        private readonly IEventAggregator eventAggregator;

        public OpenVarsWindowCommand(IEventAggregator eventAggregator, ICommandManager commandManager) : base(commandManager)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public override void Execute(object parameter)
        {
            eventAggregator.GetEvent<SimpleWindowEvent>().Publish(new SimpleWindowEventArgs { Target = nameof(VarsVM), Action = SimpleWindowEventActions.Open });
        }
    }
}
