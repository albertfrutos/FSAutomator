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

namespace FSAutomator.Backend.Utilities.Tests
{
    [TestClass]
    public class BackendMainTests
    {
        BackendMain backend;
        GeneralStatus status;

        [TestInitialize()]
        public void TestInitialize()
        {
            backend = new BackendMain();
            status = GeneralStatus.GetInstance;
        }

        [TestMethod]
        public void ValidateActions_ActionsWithNoIssuesAreUsed_ReturnsNoIssues()
        {
            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    ActionObject = new GetVariable("ATC ID"),
                    Parameters = "{\"VariableName\":\"ATC ID\"}"
                }
            );

            var isActionValidated = ActionJSONValidator.ValidateActions(backend.automator.ActionList.ToArray());

            status.ValidationIssues.Should().BeNull();
            backend.automator.ActionList[0].IsValidated.Should().BeTrue();
        }
        
        [TestMethod]
        public void ValidateActions_ActionsWithOneIssueIsUsed_ReturnsOneIssue()
        {
            backend.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Name = "GetVariable",
                    ActionObject = new GetVariable("UNEXISTING VARIABLE"),
                    Parameters = "{\"VariableName\":\"UNEXISTING VARIABLE\"}"
                }
            );

            var isActionValidated = ActionJSONValidator.ValidateActions(backend.automator.ActionList.ToArray());

            isActionValidated.Count.Should().Be(1);
            backend.automator.ActionList[0].IsValidated.Should().BeFalse();
        }
    }
}