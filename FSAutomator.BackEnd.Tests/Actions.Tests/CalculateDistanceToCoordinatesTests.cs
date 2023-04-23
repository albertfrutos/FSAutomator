using FluentAssertions;
using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class CalculateDistanceToCoordinatesTests
    {
        Mock<IGetVariable> getVariableMock;

        [TestMethod]
        public void ExecuteAction_FinalCoordinates_OriginCoordinatesAreRetrivied_ReturnsDistanceToFinalCoordinates()
        {
            //Arrange
            this.getVariableMock = new Mock<IGetVariable>();

            var calculateBearingToCoordinates = new CalculateDistanceToCoordinates(41.176307, 1.262329, this.getVariableMock.Object);

            this.getVariableMock.SetupSequence(x => x.ExecuteAction(It.IsAny<object>(), It.IsAny<ISimConnectBridge>()))
                .Returns(new ActionResult("41.29219", "41.29219", false))
                .Returns(new ActionResult("2.08371", "2.08371", false));

            //Act
            var result = calculateBearingToCoordinates.ExecuteAction(null, null);

            //Assert
            result.ComputedResult.Should().Be("69.88");
        }
    }
}