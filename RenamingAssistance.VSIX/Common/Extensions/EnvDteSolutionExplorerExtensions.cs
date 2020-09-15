using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace RenamingAssistance.VSIX.Common.Extensions
{
    internal static class EnvDteSolutionExplorerExtensions
    {
        public static string GetProperty(this ProjectItem item, string propertyName)
        {
            return item.Properties.Item(propertyName).Value.ToString();
        }

        public static ICollection<ProjectItem> GetAllCsFiles(this Project project)
        {
            var items = new List<ProjectItem>();
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.IsCsFile())
                {
                    items.Add(item);
                    continue;
                }

                items.AddRange(GetAllCsFiles(item));
            }

            return items;
        }

        public static ICollection<ProjectItem> GetAllCsFiles(this ProjectItem projectItem)
        {
            return GetFilesInternal(projectItem.ProjectItems, IsCsFile).ToList();
        }

        public static bool IsCsFile(this ProjectItem projectItem)
        {
            return projectItem.Kind == Constants.vsProjectItemKindPhysicalFile
                && projectItem.Properties.Item(ProjectItemPropertiesConstants.Extension).Value.ToString() == ".cs";
        }

        private static IEnumerable<ProjectItem> GetFilesInternal(ProjectItems items, Func<ProjectItem, bool> predicate)
        {
            if (items != null)
            {
                foreach (ProjectItem item in items)
                {
                    foreach (ProjectItem internalItem in GetFilesInternal(item, predicate))
                    {
                        if (predicate(internalItem))
                        {
                            yield return internalItem;
                        }
                    }
                }
            }
        }
        private static IEnumerable<ProjectItem> GetFilesInternal(ProjectItem item, Func<ProjectItem, bool> predicate)
        {
            if (predicate(item))
            {
                yield return item;
            }

            foreach (ProjectItem internalItem in GetFilesInternal(item.ProjectItems, predicate))
            {
                if (predicate(internalItem))
                {
                    yield return internalItem;
                }
            }
        }
    }

}
