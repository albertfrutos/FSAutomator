using FluentAssertions;
using FSAutomator.Backend.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSAutomator.Backend.Utilities.Tests
{
    [TestClass]
    public class ApplicationConfigTests
    {
        [TestInitialize()]
        public void TestInitialize()
        {

        }

        [TestMethod]
        public void ApplicationConfig_ConfigurationIsLoaded_IsCorrect()
        {
            var expectedConfig = new ApplicationConfig()
            {
                AutomationsFolder = "Automations",
                ExportFolder = "Exports",
                TempFolder = "Temp",
                LoggerFolder = "Loggers",
                FilesFolder = "Files",
                SchemaFile = "Validators\\JSONValidationSchema.jschema",
                FSPackagesPaths = new FSPackagesPathsConfig()
                {
                    FSPathOfficial = "C:\\Users\\Albert\\AppData\\Roaming\\Microsoft Flight Simulator\\Packages\\Official",
                    FSPathCommunity = "C:\\Users\\Albert\\AppData\\Roaming\\Microsoft Flight Simulator\\Packages\\Community"
                },
                KMLLoggerLog = new KMLLoggerLogConfig()
                {
                    TraceTitle = "Test_Project",
                    TraceColor = "fbc02d"
                }
            };

            var config = ApplicationConfig.GetInstance;

            config.Should().BeEquivalentTo(expectedConfig);

        }
    }
}