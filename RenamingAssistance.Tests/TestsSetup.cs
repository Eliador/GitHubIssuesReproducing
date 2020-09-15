using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace RenamingAssistance.Tests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var slnRelativePath = ConfigurationManager.AppSettings["TestCaseSolutionDir"];

            WorkSpaceProvider.Build(string.Format(slnRelativePath, rootPath));
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            WorkSpaceProvider.Clear();
        }
    }
}
