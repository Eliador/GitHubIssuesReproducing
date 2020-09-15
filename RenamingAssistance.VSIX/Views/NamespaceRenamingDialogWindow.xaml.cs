using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.CodeAnalysis;
using RenamingAssistance.VSIX.ViewModel;

namespace RenamingAssistance.VSIX.Views
{
    public partial class NamespaceRenamingDialogWindow : DialogWindowBase<NamespaceRenamingDialogViewModel>
    {
        public Action<Solution> OnApplyingChangesComplete { get; set; }
        public Func<ICollection<Document>> OnLoaded { get; set; }

        public NamespaceRenamingDialogWindow()
        {
            InitializeComponent();
        }

        private void DialogWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            var documentsToChange = OnLoaded();
            ViewModel.CalculateChanges(documentsToChange);
        }

        private void OnSelectedSolutionItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is SolutionNodeViewModel selectedNode && selectedNode.DocumentChanges != null)
            {
                SourceCodeArea.Document = selectedNode.DocumentChanges;
            }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyChanges(
                () => 
                {
                    OnApplyingChangesComplete?.Invoke(ViewModel.NewSolution);
                    DialogResult = true;
                },
                () => DialogResult = false);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
