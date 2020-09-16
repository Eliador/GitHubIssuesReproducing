using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class NamespaceChangesProcessor
    {
        public async Task<Solution> Apply(ICollection<DocumentChanges> changes, Action onChange, CancellationToken cancellationToken)
        {
            var result = await Task.Run(() => ApplyInternal(changes, onChange, cancellationToken));

            return result;
        }

        private Solution ApplyInternal(ICollection<DocumentChanges> changes, Action onChange, CancellationToken cancellationToken)
        {
            Solution newSolution = null;
            foreach (var change in changes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var documentToChange = newSolution == null ? change.TargetDocument : newSolution.GetDocument(change.TargetDocument.Id);

                newSolution = ApplyChangesToDocument(documentToChange, change.Changes.ToList());
                onChange();
            }

            return newSolution;
        }

        private Solution ApplyChangesToDocument(Document document, ICollection<Change> changes)
        {
            var oldSourceText = document.GetTextAsync().Result;
            var textChanges = changes.OfType<Change>().Select(x => new TextChange(x.SpanToChange, x.NewText));
            
            var newSourceText = oldSourceText.WithChanges(textChanges);
            var newDocument = document.WithText(newSourceText);

            return newDocument.Project.Solution;
        }
    }
}