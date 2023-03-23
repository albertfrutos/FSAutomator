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
    public class ExpectVariableValueTests
    {
        ExpectVariableValue expectedVariableValue;

        [TestMethod]
        public void NumericValue_VariableHasTargetValue_TrueIsReturned()
        {
            this.expectedVariableValue = new ExpectVariableValue("ATC ID", "MyATCID");
            var result = this.expectedVariableValue.CheckIfVariableHasExpectedValue(null, null, "MyATCID");

            result.Should().Be("true");
        }
    }
}