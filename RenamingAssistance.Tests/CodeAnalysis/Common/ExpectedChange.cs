namespace RenamingAssistance.Tests.CodeAnalysis.Common
{
    public class ExpectedChange
    {
        public ExpectedChange(int start, int end, string newText)
        {
            Start = start;
            End = end;
            NewText = newText;
        }

        public int Start { get; }

        public int End { get; }

        public string NewText { get; }
    }
}
