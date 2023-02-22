using FluentAssertions;
using FSAutomator.BackEnd.Configuration;

namespace FSAutomator.BackEnd.Tests
{
    public class ApplicationConfigTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ConfigurationIsCorrect()
        {
            var config = ApplicationConfig.GetInstance;

            config.AutomationsFolder.Should().Be("Automations");
            config.ExportFolder.Should().Be("Exports");
        }
    }
}