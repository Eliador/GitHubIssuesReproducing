using Microsoft.CodeAnalysis;
using System.Linq;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public static class CodeAnalysisExtensions
    {
        public static string GetExpectedNamespace(this Document document)
        {
            var physicalFolders = document.FilePath
                .Split('\\')
                .SkipWhile(x => x != document.Project.Name)
                .Distinct()
                .ToList();

            return string.Join(".", physicalFolders.Take(physicalFolders.Count - 1));
        }
    }
}
