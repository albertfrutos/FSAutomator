using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FSAutomator.BackEnd.Tests
{
    [TestClass]
    public class WaitUntilVariableReachesNumericValueTests
    {
        WaitUntilVariableReachesNumericValue waitUntilVariableReachesNumericValue;
        
        Mock<IGetVariable> getVariableMock;
        
        Automator automator;

        private const string numericThreshold = "778";
        private const string numericHigher = "1000";
        private const string numericLower = "100";


        [TestInitialize()]
        public void TestInitialize() 
        {
            getVariableMock = new Mock<IGetVariable>();

            automator = new Automator();

            this.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Status = FSAutomatorAction.ActionStatus.Running,
                    Result = new ActionResult()
                }
            );

            getVariableMock.SetupSequence(x => x.ExecuteAction(It.IsAny<object>(), It.IsAny<SimConnect>()))
                .Returns(new ActionResult(null, "100", false))
                .Returns(new ActionResult(null, "235", false))
                .Returns(new ActionResult(null, "567", false))
                .Returns(new ActionResult(null, "778", false))
                .Returns(new ActionResult(null, "1000", false));
        }


        [TestMethod]
        public void NumericThreshold_ValidComparisonEquals_WaitsForEqualValue()
        {
            ActionResult result = GetResult("=", numericThreshold);

            getVariableMock.Verify(x => x.ExecuteAction(automator, null), Times.Exactly(4));

            result.ComputedResult.Should().Be(numericThreshold);
            result.VisibleResult.Should().Contain("Value reached");
        }

        [TestMethod]
        public void NumericThreshold_ValidComparisonIsHigher_WaitsForHigherValue()
        {
            ActionResult result = GetResult(">", numericThreshold);

            getVariableMock.Verify(x => x.ExecuteAction(automator, null), Times.Exactly(5));

            result.ComputedResult.Should().Be(numericHigher);
        }

        [TestMethod]
        public void NumericThreshold_ValidComparisonIsLower_WaitsForLowerValue()
        {
            ActionResult result = GetResult("<", numericThreshold);

            getVariableMock.Verify(x => x.ExecuteAction(automator, null), Times.Exactly(1));

            result.ComputedResult.Should().Be(numericLower);
        }

        [TestMethod]
        public void NumericThreshold_InvalidComparison_ReturnError()
        {
            ActionResult result = GetResult("invalid", numericThreshold);

            result.ComputedResult.Should().Be(null);
            result.VisibleResult.Should().Contain("Comparison not supported");
        }
        
        [TestMethod]
        public void NumericThreshold_ValidComparisonNoCurrentAction_ReturnError()
        {
            this.automator.ActionList[0].Status = FSAutomatorAction.ActionStatus.Done;
            ActionResult result = GetResult("<", numericThreshold);

            result.ComputedResult.Should().Be(null);
            result.VisibleResult.Should().Contain("No current action could be found");
        }

        [TestMethod]
        public void NonNumericThreshold_ValidComparison_ReturnError()
        {
            ActionResult result = GetResult("<", "abcd");

            result.ComputedResult.Should().Be(null);
            result.VisibleResult.Should().Contain("ThresholdValue not a number");
        }

        private ActionResult GetResult(string comparison, string thresHold)
        {
            this.waitUntilVariableReachesNumericValue = new WaitUntilVariableReachesNumericValue("VarName", comparison, thresHold, getVariableMock.Object, 100);

            var result = this.waitUntilVariableReachesNumericValue.ExecuteAction(automator, null);
            return result;
        }
    }
}