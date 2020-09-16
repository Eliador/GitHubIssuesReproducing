using Microsoft.CodeAnalysis;
using NUnit.Framework;
using RenamingAssistance.Tests.CodeAnalysis.Common;
using System.Collections.Generic;
using System.Linq;

namespace RenamingAssistance.Tests.CodeAnalysis
{
    [TestFixture]
    public class NamespaceChangesCalculatorCommonCasesTests : NamespaceChangesCalculatorTestsBase
    {
        private const string TargetFileName = "Class1_3_1.cs";
        private const string UsagesFileName = "CommonTestCases.cs";
        private const string ExpectedNamespace = "TestCases.LibOne.Folder1_1.Folder1_2.Folder1_3";

        private readonly ExpectedChange[] _tergetFileExpectedChanges = new[]
        {
            new ExpectedChange(133, 176, ExpectedNamespace),
            new ExpectedChange(411, 454, ExpectedNamespace),
        };

        private readonly ExpectedChange[] _usagesFileExpectedChanges = new[]
        {
            new ExpectedChange(122, 165, ExpectedNamespace),
            new ExpectedChange(315, 358, ExpectedNamespace),
            new ExpectedChange(396, 412, ExpectedNamespace),
            new ExpectedChange(499, 542, ExpectedNamespace),
            new ExpectedChange(592, 608, ExpectedNamespace),
            new ExpectedChange(936, 979, ExpectedNamespace),
            new ExpectedChange(1020, 1063, ExpectedNamespace),
            new ExpectedChange(1085, 1128, ExpectedNamespace),
            new ExpectedChange(1171, 1214, ExpectedNamespace),
            new ExpectedChange(1255, 1298, ExpectedNamespace),
            new ExpectedChange(1336, 1379, ExpectedNamespace),
            new ExpectedChange(1408, 1451, ExpectedNamespace),
            new ExpectedChange(1509, 1525, ExpectedNamespace),
            new ExpectedChange(1566, 1582, ExpectedNamespace),
            new ExpectedChange(1604, 1620, ExpectedNamespace),
            new ExpectedChange(1663, 1679, ExpectedNamespace),
            new ExpectedChange(1720, 1736, ExpectedNamespace),
            new ExpectedChange(1774, 1790, ExpectedNamespace),
            new ExpectedChange(1819, 1835, ExpectedNamespace)
        };

        protected override ICollection<Document> GetDocumentsToChangesCalculation(Solution solution)
        {
            var document = solution.Projects
                .First(x => x.Name == "TestCases.LibOne")
                .Documents.First(x => x.Name == TargetFileName);

            return new[] { document };
        }

        [Test]
        public void TargetFileChangesTest()
        {
            //Assert
            var documentChanges = results.FirstOrDefault(x => x.TargetDocument.Name == TargetFileName);
            Assert.IsNotNull(documentChanges, $"In {TargetFileName} should be changes");

            AssertCalculatedChanges(documentChanges.Changes, _tergetFileExpectedChanges);
        }

        [Test]
        public void FileWithUsagesOfChangedNamespaceChangesTest()
        {
            //Assert
            var documentChanges = results.FirstOrDefault(x => x.TargetDocument.Name == UsagesFileName);
            Assert.IsNotNull(documentChanges, $"In {UsagesFileName} should be changes");

            AssertCalculatedChanges(documentChanges.Changes, _usagesFileExpectedChanges);
        }
    }
}
