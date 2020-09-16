using Microsoft.CodeAnalysis;
using NUnit.Framework;
using RenamingAssistance.Core.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RenamingAssistance.Tests.CodeAnalysis.Common
{
    public abstract class NamespaceChangesCalculatorTestsBase
    {
        protected ICollection<DocumentChanges> results;

        [SetUp]
        public void Setup()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<ProgressInfo>((progressInfo) => { });

            var solution = WorkSpaceProvider.Workspace.CurrentSolution;
            var documents = GetDocumentsToChangesCalculation(solution);

            var calculator = new NamespaceChangesCalculator();

            // Act
            results = calculator.CalculateChangesAsync(documents, progress, cancellationTokenSource.Token).Result;
        }

        protected abstract ICollection<Document> GetDocumentsToChangesCalculation(Solution solution);

        protected void AssertCalculatedChanges(ICollection<Change> changes, ExpectedChange[] expectedChanges)
        {
            var excessChanges = changes.Where(c => !expectedChanges.Any(ec =>
                    ec.Start == c.SpanToChange.Start
                    && ec.End == c.SpanToChange.End
                    && ec.NewText == c.NewText))
                .ToList();

            var missingChanges = expectedChanges.Where(ec => !changes.Any(c =>
                    ec.Start == c.SpanToChange.Start
                    && ec.End == c.SpanToChange.End
                    && ec.NewText == c.NewText))
                 .ToList();

            Assert.AreEqual(0, excessChanges.Count + missingChanges.Count, BuildErrorMessage(excessChanges, missingChanges));
        }

        private string BuildErrorMessage(ICollection<Change> excessChanges, ICollection<ExpectedChange> missingChanges)
        {
            var message = new StringBuilder();
            if (excessChanges.Any())
            {
                message.AppendLine("Excess changes:");
                foreach (var change in excessChanges)
                {
                    message.AppendLine($"    Start: {change.SpanToChange.Start}, End: {change.SpanToChange.End}");
                }
            }

            if (missingChanges.Any())
            {
                message.AppendLine("Missing changes:");
                foreach (var change in missingChanges)
                {
                    message.AppendLine($"    Start: {change.Start}, End: {change.End}");
                }
            }

            return message.ToString();
        }
    }
}
