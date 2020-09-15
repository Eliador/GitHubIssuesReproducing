using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using RenamingAssistance.Core.CodeAnalysis;
using RenamingAssistance.VSIX.ViewModel.Common;

namespace RenamingAssistance.VSIX.ViewModel
{
    public class NamespaceRenamingDialogViewModel : ViewModelBase
    {
        private const string CalculatingNamespceChanging = "Calculating namespace changing started";
        private const string ApplyChangesStage = "Apply changes";

        private bool _isApplyEnabled = false;
        private int _progressBarMaxValue;
        private int _progressCurrentValue;
        private string _processingStage = "Undefined";
        private CancellationTokenSource cancellationTokenSource;

        private readonly NamespaceChangesCalculator _namespaceRenamingChangesCalculator;
        private readonly SolutionTreeBuilder _solutionTreeBuilder;
        private readonly NamespaceChangesProcessor _namespaceChangesProcessor;

        public NamespaceRenamingDialogViewModel(
            NamespaceChangesCalculator namespaceRenamingChangesCalculator,
            SolutionTreeBuilder solutionTreeBuilder,
            NamespaceChangesProcessor namespaceChangesProcessor)
        {
            Debug.Assert(namespaceRenamingChangesCalculator != null, $"{nameof(namespaceRenamingChangesCalculator)} should not be null");
            Debug.Assert(solutionTreeBuilder != null, $"{nameof(solutionTreeBuilder)} should not be null");
            Debug.Assert(namespaceChangesProcessor != null, $"{nameof(namespaceChangesProcessor)} should not be null");

            _namespaceRenamingChangesCalculator = namespaceRenamingChangesCalculator;
            _solutionTreeBuilder = solutionTreeBuilder;
            _namespaceChangesProcessor = namespaceChangesProcessor;

            cancellationTokenSource = new CancellationTokenSource();
            CancelCommand = new ViewCommandHandler(Cancel);
        }

        public Solution NewSolution { get; private set; } = null;

        public ObservableCollection<SolutionNodeViewModel> Folders { get; } = new ObservableCollection<SolutionNodeViewModel>();

        public ViewCommandHandler CancelCommand { get; }

        public bool IsApplyEnabled
        {
            get => _isApplyEnabled;
            set
            {
                _isApplyEnabled = value;
                RaisePropertyChanged(nameof(IsApplyEnabled));
            }
        }

        public int ProgressBarMaxValue
        {
            get => _progressBarMaxValue;
            set
            {
                _progressBarMaxValue = value;
                RaisePropertyChanged(nameof(ProgressBarMaxValue));
            }
        }

        public int ProgressCurrentValue
        {
            get => _progressCurrentValue;
            set
            {
                _progressCurrentValue = value;
                RaisePropertyChanged(nameof(ProgressCurrentValue));
                RaisePropertyChanged(nameof(ProcessingText));
            }
        }

        public string ProcessingStage
        {
            get => _processingStage;
            set
            {
                _processingStage = value;
                RaisePropertyChanged(nameof(ProcessingStage));
            }
        }

        public string ProcessingText
        {
            get => $"({ProgressCurrentValue} of {ProgressBarMaxValue})";
        }

        public void CalculateChanges(ICollection<Document> documentsToChange)
        {
            CalculateChangesAsync(documentsToChange);
        }

        private async void CalculateChangesAsync(ICollection<Document> documentsToChange)
        {
            if (documentsToChange == null || !documentsToChange.Any())
            {
                return;
            }

            ProgressBarMaxValue = documentsToChange.Count;
            ProgressCurrentValue = 0;
            ProcessingStage = CalculatingNamespceChanging;
            IsApplyEnabled = false;
            var progress = new Progress<ProgressInfo>((progressInfo) =>
            {
                ProcessingStage = progressInfo.Message;
                ProgressCurrentValue = progressInfo.PercentPassed;
            });

            try
            {
                var result = await _namespaceRenamingChangesCalculator.CalculateChangesAsync(documentsToChange, progress, cancellationTokenSource.Token);
                if (result != null)
                {
                    _solutionTreeBuilder.Build(result, Folders);
                    IsApplyEnabled = true;
                }
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled by pressing Cancellation button
            }
        }

        public void ApplyChanges(Action onCompleted, Action onCancelled)
        {
            ApplyChangesAsync(onCompleted, onCancelled);
        }

        private async void ApplyChangesAsync(Action onCompleted, Action onCancelled)
        {
            var documents = Folders.SelectMany(f => f.GetAllDocumentsToChange()).ToList();

            ProgressBarMaxValue = documents.Count;
            ProgressCurrentValue = 0;            
            ProcessingStage = ApplyChangesStage;
            IsApplyEnabled = false;
            try
            {
                NewSolution = await _namespaceChangesProcessor.Apply(documents, () => ProgressCurrentValue++, cancellationTokenSource.Token);
                onCompleted();
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled by pressing Cancellation button
                onCancelled();
            }
        }

        private void Cancel(object parameters)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
