using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.CodeAnalysis.Text;
using RenamingAssistance.Core.CodeAnalysis;

namespace RenamingAssistance.VSIX.ViewModel.Common
{
    public class ViewChangesBuilder
    {
        private readonly Color _notChangedPartOfInsertText = Color.FromRgb(220, 250, 220);
        private readonly Color _changedPartOfInsertText = Color.FromRgb(150, 255, 150);
        private readonly Color _notChangedPartOfDeletedText = Color.FromRgb(255, 220, 220);
        private readonly Color _changedPartOfDeletedText = Color.FromRgb(255, 150, 150);

        public ICollection<Inline> Build(DocumentChanges documentChanges)
        {
            var lines = new List<Inline>();
            var textSource = documentChanges.TargetDocument.GetTextAsync().Result;
            if (textSource != null)
            {
                var changesToApply = documentChanges.Changes.OfType<Change>().OrderBy(x => x.SpanToChange.Start).ToList();
                var linesWithChanges = textSource.Lines.Select(l => new
                {
                    Line = l,
                    Changes = changesToApply.Where(c => IsLineContainsChange(l, c.SpanToChange)).ToList()
                });

                var newLinePartsFromPrevStep = new List<Inline>();
                foreach (var lineWithChanges in linesWithChanges)
                {
                    if (!lineWithChanges.Changes.Any())
                    {
                        lines.Add(CreateRun(lineWithChanges.Line.ToString(), Color.FromRgb(255, 255, 255)));
                        lines.Add(new LineBreak());
                        continue;
                    }

                    var result = GetLineParts(lineWithChanges.Line, lineWithChanges.Changes, newLinePartsFromPrevStep);
                    lines.AddRange(result.OldLineParts);
                    lines.Add(new LineBreak());
                    if (result.NewLineIsBuilded)
                    {
                        lines.AddRange(result.NewLineParts);
                        lines.Add(new LineBreak());
                        newLinePartsFromPrevStep.Clear();
                    }
                }
            }

            return lines;
        }

        private bool IsLineContainsChange(TextLine line, TextSpan change)
        {
            return (change.Start >= line.Start && change.Start < line.End)
                || (change.End >= line.Start && change.End < line.End)
                || (change.Start <= line.Start && change.End >= line.End);
        }

        private LinePartsWithChanges GetLineParts(TextLine textLine, ICollection<Change> changes, List<Inline> newLinePartsFromPrevStep)
        {
            var lineStartedFrom = 0;
            var text = textLine.ToString();
            var oldLineParts = new List<Inline>();
            var newLineParts = newLinePartsFromPrevStep;
            var newLineIsBuilded = true;
            foreach (var change in changes.OrderBy(x => x.SpanToChange.Start))
            {
                var spanChange = change.SpanToChange;
                var changeStart = spanChange.Start - textLine.Start;
                var changeEnd = spanChange.End - textLine.Start;

                if (changeStart > lineStartedFrom)
                {
                    var textBeforeChange = text.Substring(lineStartedFrom, changeStart - lineStartedFrom);
                    oldLineParts.Add(CreateRun(textBeforeChange, _notChangedPartOfDeletedText));
                    newLineParts.Add(CreateRun(textBeforeChange, _notChangedPartOfInsertText));
                }
                else
                {
                    changeStart = lineStartedFrom;
                }

                if (changeEnd < text.Length)
                {
                    var textToChange = text.Substring(changeStart, changeEnd - changeStart);
                    lineStartedFrom = changeEnd;
                    oldLineParts.Add(CreateRun(textToChange, _changedPartOfDeletedText));
                }
                else
                {
                    var textToChange = text.Substring(changeStart, text.Length - changeStart);
                    lineStartedFrom = text.Length - 1;
                    oldLineParts.Add(CreateRun(textToChange, _changedPartOfDeletedText));
                }

                newLineIsBuilded = textLine.End >= spanChange.End;
                if (newLineIsBuilded)
                {
                    newLineParts.Add(CreateRun(change.NewText, _changedPartOfInsertText));
                }
            }

            if (lineStartedFrom > 0 && lineStartedFrom < text.Length - 1)
            {
                var textAfterChange = text.Substring(lineStartedFrom, text.Length - lineStartedFrom);
                oldLineParts.Add(CreateRun(textAfterChange, _notChangedPartOfDeletedText));
                newLineParts.Add(CreateRun(textAfterChange, _notChangedPartOfInsertText));
            }

            return new LinePartsWithChanges
            {
                OldLineParts = oldLineParts,
                NewLineParts = newLineParts,
                NewLineIsBuilded = newLineIsBuilded & newLineParts.OfType<Run>().Any(x => !string.IsNullOrWhiteSpace(x.Text))
            };
        }

        private Run CreateRun(string text, Color background)
        {
            return new Run
            {
                FontFamily = new FontFamily("Verdana"),
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Background = new SolidColorBrush(background),
                Text = text
            };
        }

        private class LinePartsWithChanges
        {
            public List<Inline> OldLineParts { get; set; }

            public List<Inline> NewLineParts { get; set; }

            public bool NewLineIsBuilded { get; set; }
        }
    }
}
