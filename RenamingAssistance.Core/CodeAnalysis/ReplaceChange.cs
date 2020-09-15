using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class ReplaceChange : ChangeBase
    {
        public ReplaceChange(TextSpan spanToChange, string newText, Document documentToChange)
            : base(newText, documentToChange)
        {
            SpanToChange = spanToChange;
        }

        public TextSpan SpanToChange { get; }
    }
}
