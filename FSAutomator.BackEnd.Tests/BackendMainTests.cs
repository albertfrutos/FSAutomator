using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using FSAutomator.BackEnd.Validators;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Reflection;
using static FSAutomator.Backend.Entities.InternalMessage;

namespace FSAutomator.Backend.Utilities.Tests
{
    [TestClass]
    public class BackendMainTests
    {
        BackendMain backend;
        GeneralStatus status;
        string currentDir;

        [TestInitialize()]
        public void TestInitialize()
        {
            backend = new BackendMain();
            status = GeneralStatus.GetInstance;
            currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        [TestMethod]
        public void ValidateActions_ActionsWithNoIssuesAreUsed_ReturnsNoIssues()
        {
            //Arrange
            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    ActionObject = new GetVariable("ATC ID"),
                    Parameters = "{\"VariableName\":\"ATC ID\"}"
                }
            );

            //Act
            var isActionValidated = ActionJSONValidator.ValidateActions(backend.automator.ActionList.ToArray());

            //Assert
            status.ValidationIssues.Should().BeNull();
            backend.automator.ActionList[0].IsValidated.Should().BeTrue();
        }
        
        [TestMethod]
        public void ValidateActions_ActionsWithOneIssueIsUsed_ReturnsOneIssue()
        {
            //Arrange
            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    ActionObject = new GetVariable("UNEXISTING VARIABLE"),
                    Parameters = "{\"VariableName\":\"UNEXISTING VARIABLE\"}"
                }
            );

            //Act
            var isActionValidated = ActionJSONValidator.ValidateActions(backend.automator.ActionList.ToArray());

            //Assert
            isActionValidated.Count.Should().Be(1);
            isActionValidated[0].Should().Be("GetVariable [0]: Variable UNEXISTING VARIABLE does not exist.");
            backend.automator.ActionList[0].IsValidated.Should().BeFalse();
        }

        [TestMethod]
        public void SaveAutomation_FileNameIsEmpty_ReturnsError()
        {
            //Arrange
            const string fileName = "";

            //Act
            var result = backend.SaveAutomation(null, fileName);

            //Assert
            result.Message.Should().Be("Please enter an automation name.");
            result.Type.Should().Be(MsgType.Info);
        }
        
        [TestMethod]
        public void SaveAutomation_AutomationIsDLLAutomation_ReturnsError()
        {
            //Arrange
            const string fileName = "testFileName.json";
            const AutomationFile automationFile = new AutomationFile()
            {
                BasePath = "C:\\Users\\Albert\\Source\\Repos\\albertfrutos\\FSAutomator\\FSAutomator.UI\\bin\\Debug\\net6.0-windows\\Automations",
                FileName = "ExternalAutomationExample.dll",
                FilePath = "Automations\\ExternalAutomationExample.dll",
                IsPackage = false,
                PackageName = "",
                VisibleName = "ExternalAutomationExample [.dll]"
            };

            //Act
            var result = backend.SaveAutomation(null, fileName);

            //Assert
            result.Message.Should().Be("Please enter an automation name.");
            result.Type.Should().Be(MsgType.Info);
        }
    }
}