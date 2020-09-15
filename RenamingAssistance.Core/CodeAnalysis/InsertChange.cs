using Microsoft.CodeAnalysis;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class InsertChange : ChangeBase
    {
        public InsertChange(int position, string newText, Document documentToChange)
            : base(newText, documentToChange)
        {
            Position = position;
        }

        public int Position { get; }
    }
}
