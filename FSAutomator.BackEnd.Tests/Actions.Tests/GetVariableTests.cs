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
    public class GetVariableTests
    {
        GetVariable getVariable;

        Mock<ISimConnectBridge> simConnectBridgeMock;

        [TestMethod]
        public void ExistingVariable_VariableHasTargetValue_VariableValueIsReturned()
        {
            //Arrange
            this.getVariable = new GetVariable("ATC ID");

            this.simConnectBridgeMock = new Mock<ISimConnectBridge>();
            this.simConnectBridgeMock.Setup(x => x.AddToDataDefinition(It.IsAny<Enum>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SIMCONNECT_DATATYPE>(), It.IsAny<float>()));
            this.simConnectBridgeMock.Setup(x => x.RegisterDataDefineStruct<StringType>(It.IsAny<Enum>()));
            this.simConnectBridgeMock.Setup(x => x.SubscribeToOnRecvSimobjectDataBytypeEventHandler(It.IsAny<Action<SimConnect, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE>>()));
            this.simConnectBridgeMock.Setup(x => x.RequestDataOnSimObjectType(It.IsAny<Enum>(), It.IsAny<Enum>(), It.IsAny<uint>(), It.IsAny<SIMCONNECT_SIMOBJECT_TYPE>()));
            this.simConnectBridgeMock.Setup(x => x.ClearDataDefinition(It.IsAny<Enum>())).Callback(() =>
            {
                this.getVariable.VariableValue = "myValue";
                this.getVariable.retainUntilValueReadyEvent.Set();
            });

            //Act
            var result = this.getVariable.ExecuteAction(this, simConnectBridgeMock.Object);

            //Assert
            result.ComputedResult.Should().Be("myValue");
        }
        
        [TestMethod]
        public void NotExistingVariable_VariableHasTargetValue_NullIsReturned()
        {
            //Arrange
            this.getVariable = new GetVariable("UNEXISTING VARIABLE");

            this.simConnectBridgeMock = new Mock<ISimConnectBridge>();

            //Act
            var result = this.getVariable.ExecuteAction(this, simConnectBridgeMock.Object);

            //Assert
            result.ComputedResult.Should().BeNull();
            result.Error.Should().BeTrue();
        }
    }
}