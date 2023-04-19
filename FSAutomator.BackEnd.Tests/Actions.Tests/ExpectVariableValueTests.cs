using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.SimConnectInterface;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class ExpectVariableValueTests
    {
        ExpectVariableValue expectedVariableValue;
        
        Mock<IGetVariable> getVariableMock;

        [TestMethod]
        public void NumericValue_VariableHasTargetValue_TrueIsReturned()
        {
            getVariableMock = new Mock<IGetVariable>();
            getVariableMock.Setup(x => x.ExecuteAction(It.IsAny<object>(), It.IsAny<ISimConnectBridge>())).Returns(new ActionResult(null, "MyATCID", false));

            this.expectedVariableValue = new ExpectVariableValue("ATC ID", "MyATCID", getVariableMock.Object);

            var result = this.expectedVariableValue.ExecuteAction(null, null);

            result.ComputedResult.Should().Be("True");
        }
    }
}