using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.SimConnectInterface;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class SendEventTests
    {
        SendEvent sendEvent;
        
        Mock<ISimConnectBridge> simConnectBridgeMock;

        [TestMethod]
        public void ExistingEvent_EventHasValue_EventIsSent()
        {
            const string EventName = "AP_MASTER";
            const string EventValue = "1";
            //Arrange
            this.sendEvent = new SendEvent(EventName, EventValue);

            this.simConnectBridgeMock = new Mock<ISimConnectBridge>();
            this.simConnectBridgeMock.Setup(x => x.MapClientEventToSimEvent(It.IsAny<Enum>(), It.IsAny<string>()));
            this.simConnectBridgeMock.Setup(x => x.AddClientEventToNotificationGroup(It.IsAny<Enum>(), It.IsAny<Enum>(), It.IsAny<bool>()));
            this.simConnectBridgeMock.Setup(x => x.SetNotificationGroupPriority(It.IsAny<Enum>(), It.IsAny<uint>()));
            this.simConnectBridgeMock.Setup(x => x.TransmitClientEvent(It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()));
            this.simConnectBridgeMock.Setup(x => x.ClearNotificationGroup(It.IsAny<Enum>()));

            //Act
            var result = this.sendEvent.ExecuteAction(this, simConnectBridgeMock.Object);

            //Assert
            result.ComputedResult.Should().Be(EventValue);
            result.Error.Should().BeFalse();
        }
        
        [TestMethod]
        public void NotExistingVariable_VariableHasTargetValue_NullIsReturned()
        {
            const string EventName = "UNEXISTING_EVENT";
            const string EventValue = "1";

            //Arrange
            this.sendEvent = new SendEvent(EventName, EventValue);

            this.simConnectBridgeMock = new Mock<ISimConnectBridge>();

            //Act
            var result = this.sendEvent.ExecuteAction(this, simConnectBridgeMock.Object);

            //Assert
            result.ComputedResult.Should().BeNull();
            result.Error.Should().BeTrue();
        }
    }
}