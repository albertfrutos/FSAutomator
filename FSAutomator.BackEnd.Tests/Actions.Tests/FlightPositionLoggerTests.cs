using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class FlightPositionLoggerTests
    {
        FlightPositionLogger flightPositionLogger;

        Mock<IGetVariable> getVariableMock;

        [TestInitialize]
        public void TestInitialize()
        {
            getVariableMock = new Mock<IGetVariable>();

            this.getVariableMock.SetupSequence(x => x.ExecuteAction(It.IsAny<object>(), It.IsAny<SimConnect>()))
                .Returns(new ActionResult("1", "1", false))
                .Returns(new ActionResult("1", "1", false))
                .Returns(new ActionResult("1", "1", false))
                .Returns(new ActionResult("2", "2", false))
                .Returns(new ActionResult("2", "2", false))
                .Returns(new ActionResult("2", "2", false))
                .Returns(new ActionResult("3", "3", false))
                .Returns(new ActionResult("3", "3", false))
                .Returns(new ActionResult("3", "3", false))
                .Returns(new ActionResult("4", "4", false))
                .Returns(new ActionResult("4", "4", false))
                .Returns(new ActionResult("4", "4", false));
        }

        [TestMethod]
        public void ValidLoggerConfigurationWithBackgroundModeDisabled_StartLogging_StopsAndSavesLoggerAfterSpecifiedTime()
        {
            const int logDuration = 3;
            const int loggingPeriod = 1;
            
            //Arrange

            var stopWatch = new Stopwatch();

            this.flightPositionLogger = new FlightPositionLogger(logDuration, loggingPeriod, getVariableMock.Object, false);

            //Act
            stopWatch.Start();
            var result = flightPositionLogger.ExecuteAction(null, null);
            stopWatch.Stop();

            //Assert
            result.VisibleResult.Should().Contain("Logging finished");
            result.ComputedResult.Should().Contain(DateTime.Now.ToString("yyyyMMdd"));
            File.Exists(Path.Combine(ApplicationConfig.GetInstance.LoggerFolder, $"{result.ComputedResult}.kml")).Should().BeTrue();
            File.Exists(Path.Combine(ApplicationConfig.GetInstance.LoggerFolder, $"{result.ComputedResult}.xml")).Should().BeTrue();
            stopWatch.ElapsedMilliseconds.Should().BeLessThan(4000);
        }
        
        [TestMethod]
        public void ValidLoggerConfigurationWithBackgroundModeDisabled_StartLogging_StopsAndSavesLoggerAfterStopCommandIsSent()
        {
            const int logDuration = 3;
            const int loggingPeriod = 1;
            
            //Arrange
            var stopWatch = new Stopwatch();

            this.flightPositionLogger = new FlightPositionLogger(logDuration, loggingPeriod, getVariableMock.Object, true);

            //Act
            var result = flightPositionLogger.ExecuteAction(null, null);
            flightPositionLogger.continueLogging.Should().BeTrue();
            Thread.Sleep(3000);
            flightPositionLogger.StopBackgroundLogging(null, true);

            //Assert
            result.VisibleResult.Should().Contain("Logger started");
            flightPositionLogger.continueLogging.Should().BeFalse();
            stopWatch.ElapsedMilliseconds.Should().BeLessThan(4000);
        }
    }
}