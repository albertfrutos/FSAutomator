using FluentAssertions;
using FSAutomator.BackEnd.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSAutomator.Backend.Utilities.Tests
{
    [TestClass]
    public class AvailableActionsTests
    {
        [TestInitialize()]
        public void TestInitialize()
        {

        }

        [TestMethod]
        public void AvailableActions_AvailableActionNamesAreRetrieved_ReturnsExistingActions()
        {
            var expectedAvailableActions = new List<string>()
            {
                "ConditionalAction",
                "ExecuteCodeFromDLL",
                "ExpectVariableValue",
                "GetVariable",
                "MemoryRegisterRead",
                "MemoryRegisterWrite",
                "OperateValue",
                "SendEvent",
                "WaitSeconds",
                "WaitUntilVariableReachesNumericValue",
                "CalculateBearingToCoordinates",
                "CalculateDistanceToCoordinates",
                "FlightPositionLogger",
                "FlightPositionLoggerStop"
            };

            var availableActions = new AvailableActions().GetAvailableActions().FSAutomatorAvailableActions.Select(a => a.Name).ToList();

            availableActions.Should().BeEquivalentTo(expectedAvailableActions);

        }
    }
}