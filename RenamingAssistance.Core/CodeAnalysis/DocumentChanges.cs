using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class DocumentChanges
    {
        public DocumentChanges(Document targetDocument)
        {
            TargetDocument = targetDocument;
        }

        public Document TargetDocument { get; }

        public bool ApplyChangesForDocument { get; set; } = true;

        public ICollection<ChangeBase> Changes { get; } = new List<ChangeBase>();
    }
}
