using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.Base;
using L2Data2CodeWPF.Commands.Interfaces;
using L2Data2CodeWPF.Controls.MessagePanel;
using L2Data2CodeWPF.Main;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace L2Data2CodeWPF.Commands
{
    internal class GenerateCommand : DelegateCommand, IGenerateCommand
    {
        private readonly IProcessManager processManager;
        private readonly IMessagePanelService messagePanelService;
        private readonly IGeneratorAdapter generatorAdapter;

        public GenerateCommand(IProcessManager processManager, IMessagePanelService messagePanelService, IGeneratorAdapter generatorAdapter)
        {
            this.processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
            this.messagePanelService = messagePanelService ?? throw new ArgumentNullException(nameof(messagePanelService));
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
        }

        public override bool CanExecute(object parameter)
        {
            var mainWindowVM = parameter as MainWindowVM;

            if (mainWindowVM?.OutputPath == null)
            {
                return false;
            }
            var existSln = File.Exists(mainWindowVM.SlnFile);
            var runnig = processManager.IsRunning(mainWindowVM.SlnFile);
            var anyItems = mainWindowVM.TablePanelVM.AllDataItems.Any(k => k.Value.IsSelected);

            var result = !mainWindowVM.RunningGenerateCode && (!existSln || existSln && !runnig) && anyItems;

            if (!mainWindowVM.RunningGenerateCode && anyItems && runnig)
            {
                messagePanelService.Add(string.Format(Strings.CannotGenerateCode, mainWindowVM.SlnFile), mainWindowVM.MessagePanelVM.MessagePanelOpened, MessageCodes.CAN_GENERATE_CODE);
            }
            else
            {
                messagePanelService.ClearPinned(MessageCodes.CAN_GENERATE_CODE);
            }
            return result;
        }

        public override void Execute(object parameter)
        {
            var mainWindowVM = parameter as MainWindowVM;

            mainWindowVM.Working = true;
            mainWindowVM.RunningGenerateCode = true;
            mainWindowVM.CheckButtonStates();

            CodeGeneratorDto options = new()
            {
                GenerateReferenced = mainWindowVM.TablePanelVM.SetRelatedTables,
                OutputPath = mainWindowVM.OutputPath.AddPathSeparator(),
                RemoveFolders = mainWindowVM.EmptyFolders,
                TableList = mainWindowVM.TablePanelVM.AllDataItems.Where(k => k.Value.IsSelected).Select(k => k.Key).ToList(),
                GeneratorApplication = mainWindowVM.GeneratorApplication,
                GeneratorVersion = mainWindowVM.GeneratorVersion,
                GeneateOnlyJson = mainWindowVM.GenerateOnlyJson
            };
            Task.Run(() => generatorAdapter.Run(options))
                .ContinueWith((state) =>
                {
                    mainWindowVM.RunningGenerateCode = false;
                    mainWindowVM.CheckButtonStates();
                    mainWindowVM.Working = false;
                });
        }
    }
}
