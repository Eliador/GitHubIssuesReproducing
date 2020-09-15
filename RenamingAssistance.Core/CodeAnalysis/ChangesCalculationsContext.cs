using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class ChangesCalculationsContext
    {
        public ChangesCalculationsContext(
            ICollection<Document> documentsToChange)
        {
            Changes = new List<DocumentChanges>();
            Solution = documentsToChange.First().Project.Solution;
            DocumentsToChange = documentsToChange;
        }

        public Solution Solution { get; }

        public ICollection<Document> DocumentsToChange { get; }

        public ICollection<DocumentChanges> Changes { get; }

        public void AddReplaceChange(TextSpan spanToChange, Document documentToChange, string newText)
        {
            var documentChanges = GetDocumentChangesToAddChange(documentToChange);

            documentChanges.Changes.Add(new ReplaceChange(spanToChange, newText, documentToChange));
        }

        public void AddInsertChange(int position, Document documentToChange, string newText)
        {
            var documentChanges = GetDocumentChangesToAddChange(documentToChange);

            documentChanges.Changes.Add(new InsertChange(position, newText, documentToChange));
        }

        private DocumentChanges GetDocumentChangesToAddChange(Document documentToChange)
        {
            var documentChanges = Changes.SingleOrDefault(x => x.TargetDocument == documentToChange);
            if (documentChanges == null)
            {
                documentChanges = new DocumentChanges(documentToChange);
                Changes.Add(documentChanges);
            }

            return documentChanges;
        }
    }
}
