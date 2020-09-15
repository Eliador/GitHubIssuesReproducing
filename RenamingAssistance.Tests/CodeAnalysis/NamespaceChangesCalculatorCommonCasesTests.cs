using NUnit.Framework;
using RenamingAssistance.Core.CodeAnalysis;
using System;
using System.Linq;
using System.Threading;

namespace RenamingAssistance.Tests.CodeAnalysis
{
    [TestFixture]
    public class NamespaceChangesCalculatorCommonCasesTests
    {
        private readonly NamespaceChangesCalculator _calculator = new NamespaceChangesCalculator();

        [Test]
        public void Test1()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<ProgressInfo>((progressInfo) => { });

            var solution = WorkSpaceProvider.Workspace.CurrentSolution;
            var document = solution.Projects
                .First(x => x.Name == "TestCases.LibOne")
                .Documents
                    .First(x => x.Name == "Class1_3_1.cs");

            var result = _calculator.CalculateChangesAsync(new[] { document }, progress, cancellationTokenSource.Token).Result;
        }
    }
}
