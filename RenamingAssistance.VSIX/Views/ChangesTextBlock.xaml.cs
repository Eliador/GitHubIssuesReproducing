using RenamingAssistance.VSIX.Common;
using RenamingAssistance.Core.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using RenamingAssistance.VSIX.ViewModel.Common;

namespace RenamingAssistance.VSIX.Views
{
    /// <summary>
    /// Interaction logic for ChangesTextBlock.xaml
    /// </summary>
    public partial class ChangesTextBlock : UserControl
    {
        public readonly ViewChangesBuilder _viewChangesBuilder;

        public ChangesTextBlock()
        {
            InitializeComponent();

            _viewChangesBuilder = (ViewChangesBuilder) CompositionUtil.ServiceProvider.GetService(typeof(ViewChangesBuilder));
        }

        public static readonly DependencyProperty DocumentProperty = 
        DependencyProperty.Register("Document", typeof(DocumentChanges), typeof(ChangesTextBlock));

        public DocumentChanges Document
        {
            get => GetValue(DocumentProperty) as DocumentChanges;
            set 
            {
                SetValue(DocumentProperty, value);
                TextSourceView.Inlines.Clear();
                TextSourceView.Inlines.AddRange(_viewChangesBuilder.Build(value));
            }
        }
    }
}
