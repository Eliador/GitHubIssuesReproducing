using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class Change
    {
        public Change(TextSpan spanToChange, string newText, Document documentToChange)
        {
            SpanToChange = spanToChange;
            NewText = newText;
            DocumentToChange = documentToChange;
            ApplyChange = true;
        }

        public TextSpan SpanToChange { get; }

        public string NewText { get; }

        public Document DocumentToChange { get; }

        public bool ApplyChange { get; set; }
    }
}
