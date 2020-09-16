using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System.Threading.Tasks;

namespace RenamingAssistance.Tests
{
    public static class WorkSpaceProvider
    {
        public static MSBuildWorkspace Workspace { get; private set; }

        public static void Build(string slnPath)
        {
            if (Workspace == null)
            {
                MSBuildLocator.RegisterDefaults();
                Workspace = MSBuildWorkspace.Create();
            }
            else 
            {
                Workspace.CloseSolution();
            }

            Task.Run(() => Workspace.OpenSolutionAsync(slnPath)).Wait();
        }

        public static void Clear()
        {
            Workspace.CloseSolution();
            Workspace.Dispose();
        }
    }
}
