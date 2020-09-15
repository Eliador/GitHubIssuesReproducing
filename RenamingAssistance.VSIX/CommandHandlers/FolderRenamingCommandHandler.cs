using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using RenamingAssistance.VSIX.Common.Extensions;
using Solution = Microsoft.CodeAnalysis.Solution;
using Document = Microsoft.CodeAnalysis.Document;

namespace RenamingAssistance.VSIX.CommandHandlers
{
    internal sealed class FolderRenamingCommandHandler : RenamingCommandHandlerBase
    {
        public override int CommandId => 0x0100;

        public override Guid CommandSet => new Guid("26e931ef-4bcd-473c-b7db-432fc4a20d22");

        private FolderRenamingCommandHandler(Package package)
            : base(package)
        {
        }

        public static FolderRenamingCommandHandler Instance
        {
            get;
            private set;
        }

        public static void Initialize(Package package)
        {
            Instance = new FolderRenamingCommandHandler(package);
        }

        protected override void beforeQueryStatusEventHandler(object sender, EventArgs e)
        {
            var command = (OleMenuCommand)sender;
            var dte = (DTE)ServiceProvider.GetService(typeof(DTE));

            if (dte.SelectedItems.Count != 1)
            {
                command.Visible = false;
                return;
            }

            var selectedItem = dte.SelectedItems.Item(1);
            var isFolder = selectedItem.ProjectItem.Kind == Constants.vsProjectItemKindPhysicalFolder;

            command.Visible = isFolder;
        }

        protected override ICollection<Document> GetDocumentsToProcess(Solution solution)
        {
            var allDocuments = solution.Projects.SelectMany(p => p.Documents).ToList();

            var dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            var selectedFolder = dte.SelectedItems.Item(1).ProjectItem;
            var allCsFilesInFolder = selectedFolder.GetAllCsFiles();

            return allCsFilesInFolder
                .Select(f => allDocuments.SingleOrDefault(d => d.FilePath == f.GetProperty(ProjectItemPropertiesConstants.FullPath)))
                .ToList();
        }
    }
}
