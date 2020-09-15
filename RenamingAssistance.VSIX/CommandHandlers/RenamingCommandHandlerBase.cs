using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using RenamingAssistance.VSIX.Views;
using Solution = Microsoft.CodeAnalysis.Solution;
using Document = Microsoft.CodeAnalysis.Document;

namespace RenamingAssistance.VSIX.CommandHandlers
{
    internal abstract class RenamingCommandHandlerBase
    {
        public abstract int CommandId { get; }

        public abstract Guid CommandSet { get; }

        protected readonly IServiceProvider ServiceProvider;

        protected RenamingCommandHandlerBase(Package package)
        {
            Debug.Assert(package != null, $"{nameof(package)} should not be null");
            ServiceProvider = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            Debug.Assert(commandService != null, $"{nameof(commandService)} should not be null");

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(InvokeEventHandler, menuCommandId);
            menuItem.BeforeQueryStatus += beforeQueryStatusEventHandler;
            commandService.AddCommand(menuItem);
        }

        private void InvokeEventHandler(object sender, EventArgs e)
        {
            var workspace = GetVisualStudioWorkspace();
            var documents = GetDocumentsToProcess(workspace.CurrentSolution);

            var namespaceRenamingDialogWindow = new NamespaceRenamingDialogWindow
            {
                OnLoaded = () => documents,
                OnApplyingChangesComplete = (Solution solution) =>
                {
                    if (solution != null)
                    {
                        workspace.TryApplyChanges(solution);
                    }
                }
            };

            namespaceRenamingDialogWindow.ShowDialog();
        }

        protected abstract void beforeQueryStatusEventHandler(object sender, EventArgs e);

        private VisualStudioWorkspace GetVisualStudioWorkspace()
        {
            var componentModel = ServiceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            Debug.Assert(componentModel != null, $"{nameof(componentModel)} should not be null");

            return componentModel.GetService<VisualStudioWorkspace>();
        }

        protected abstract ICollection<Document> GetDocumentsToProcess(Solution solution);
    }
}
