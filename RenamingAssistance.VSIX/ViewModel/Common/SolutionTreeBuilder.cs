using System.Collections.ObjectModel;
using System.Linq;
using RenamingAssistance.VSIX.Resources;
using RenamingAssistance.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RenamingAssistance.VSIX.ViewModel.Common
{
    public class SolutionTreeBuilder
    {
        public void Build(ICollection<DocumentChanges> documentsChanges, ObservableCollection<SolutionNodeViewModel> rootFolders)
        {
            foreach (var changes in documentsChanges)
            {
                var rootFolder = GetDocumentRootFolder(changes.TargetDocument, rootFolders);
                AddDocumentToTree(changes, rootFolder);
            }
        }

        private SolutionNodeViewModel GetDocumentRootFolder(
            Document document,
            ObservableCollection<SolutionNodeViewModel> rootFolders)
        {
            var projectName = document.Project.Name;

            var rootFolder = rootFolders.FirstOrDefault(x => x.Name == projectName);

            if (rootFolder == null)
            {
                rootFolder = new SolutionNodeViewModel(projectName, ExtensionResources.ProjectIcon, ExtensionResources.ProjectIcon, null);
                rootFolders.Add(rootFolder);
            }

            return rootFolder;
        }

        private void AddDocumentToTree(DocumentChanges changes, SolutionNodeViewModel rootFolder)
        {
            var documentRoot = GetDocumentNode(changes.TargetDocument, rootFolder);

            documentRoot.Folders.Add(new SolutionNodeViewModel(
                changes.TargetDocument.Name,
                ExtensionResources.CsIcon,
                ExtensionResources.CsIcon,
                documentRoot,
                changes));
        }

        private SolutionNodeViewModel GetDocumentNode(Document document, SolutionNodeViewModel rootFolder)
        {
            if (document.Folders == null || !document.Folders.Any())
            {
                return rootFolder;
            }

            SolutionNodeViewModel nextFolder = null;
            var currentFolder = rootFolder;
            foreach (var folder in document.Folders)
            {
                nextFolder = currentFolder.Folders.FirstOrDefault(x => x.Name == folder);

                if (nextFolder == null)
                {
                    var newFolder = new SolutionNodeViewModel(
                        folder,
                        ExtensionResources.FolderOpenedIcon,
                        ExtensionResources.FolderIcon,
                        currentFolder);
                    currentFolder.Folders.Add(newFolder);
                    nextFolder = newFolder;
                }

                currentFolder = nextFolder;
            }

            return nextFolder;
        }
    }
}
