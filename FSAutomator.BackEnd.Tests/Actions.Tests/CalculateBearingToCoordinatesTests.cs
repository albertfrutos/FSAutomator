using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class CalculateBearingToCoordinatesTests
    {
        Mock<IGetVariable> getVariableMock;

        [TestMethod]
        public void ExecuteAction_FinalCoordinates_OriginCoordinatesAreRetrivied_ReturnsBearingToFinalCoordinates()
        {
            this.getVariableMock = new Mock<IGetVariable>();

            var calculateBearingToCoordinates = new CalculateBearingToCoordinates(41.176307, 1.262329, this.getVariableMock.Object);

            this.getVariableMock.SetupSequence(x => x.ExecuteAction(It.IsAny<object>(), It.IsAny<SimConnect>()))
                .Returns(new ActionResult("41.29219", "41.29219", false))
                .Returns(new ActionResult("2.08371", "2.08371", false));

            var result = calculateBearingToCoordinates.ExecuteAction(null, null);

            result.ComputedResult.Should().StartWith("259.37");
        }
    }
}