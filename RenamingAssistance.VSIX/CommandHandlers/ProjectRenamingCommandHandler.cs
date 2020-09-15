using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Solution = Microsoft.CodeAnalysis.Solution;
using Document = Microsoft.CodeAnalysis.Document;
using RenamingAssistance.VSIX.Common.Extensions;

namespace RenamingAssistance.VSIX.CommandHandlers
{
    internal sealed class ProjectRenamingCommandHandler : RenamingCommandHandlerBase
    {
        public override int CommandId => 0x0101;

        public override Guid CommandSet => new Guid("C3EFAA04-6958-4017-8872-87EE61BCCE6D");

        public ProjectRenamingCommandHandler(Package package)
            : base(package)
        {
        }

        public static ProjectRenamingCommandHandler Instance
        {
            get;
            private set;
        }

        public static void Initialize(Package package)
        {
            Instance = new ProjectRenamingCommandHandler(package);
        }

        protected override void beforeQueryStatusEventHandler(object sender, EventArgs e)
        {
            var command = (OleMenuCommand)sender;
            command.Visible = true;
        }

        protected override ICollection<Document> GetDocumentsToProcess(Solution solution)
        {
            var dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            var selectedProject = dte.SelectedItems.Item(1).Project;
            var projectCsFiles = selectedProject.GetAllCsFiles().Select(x => x.GetProperty(ProjectItemPropertiesConstants.FullPath));

            return solution.Projects.Single(x => x.Name == selectedProject.Name).Documents?.Where(x => projectCsFiles.Contains(x.FilePath)).ToList();
        }
    }
}
