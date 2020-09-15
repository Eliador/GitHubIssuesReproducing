using Microsoft.Extensions.DependencyInjection;
using RenamingAssistance.Core.CodeAnalysis;
using RenamingAssistance.VSIX.ViewModel;
using RenamingAssistance.VSIX.ViewModel.Common;

namespace RenamingAssistance.VSIX.Common
{
    public static class CompositionUtil
    {
        public static ServiceProvider ServiceProvider { get; }

        static CompositionUtil()
        {
            var serviceCollection = new ServiceCollection();
            SetupDependencies(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void SetupDependencies(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<NamespaceRenamingDialogViewModel>();

            serviceCollection.AddTransient<NamespaceChangesCalculator>();
            serviceCollection.AddTransient<SolutionTreeBuilder>();
            serviceCollection.AddTransient<ViewChangesBuilder>();
            serviceCollection.AddTransient<NamespaceChangesProcessor>();
        }

        public static void Clear()
        {
            ServiceProvider?.Dispose();
        }
    }
}
