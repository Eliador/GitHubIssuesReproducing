using Microsoft.CodeAnalysis;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public abstract class ChangeBase
    {
        public ChangeBase(string newText, Document documentToChange)
        {
            NewText = newText;
            DocumentToChange = documentToChange;
            ApplyChange = true;
        }

        public string NewText { get; }

        public Document DocumentToChange { get; }

        public bool ApplyChange { get; set; }
    }
}
