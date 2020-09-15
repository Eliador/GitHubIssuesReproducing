using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using RenamingAssistance.VSIX.CommandHandlers;
using RenamingAssistance.VSIX.Common;

namespace RenamingAssistance.VSIX
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class RenamingPackage : AsyncPackage
    {
        public const string PackageGuidString = "247c49fa-ec84-40c2-968a-4099602a01f2";

        public RenamingPackage()
        {
            FolderRenamingCommandHandler.Initialize(this);
            ProjectRenamingCommandHandler.Initialize(this);
        }

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            CompositionUtil.Clear();

            base.Dispose(disposing);
        }
    }
}
