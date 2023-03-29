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
    public class ConditionalActionTests
    {
        ConditionalAction conditionalAction;

        Automator automator;

        Mock<IGetVariable> getVariableMock;

        [TestMethod]
        public void StringValue_EqualConditionIsTrue_ExecutesTrueAction()
        {
            const string firstMember = "member";
            const string secondMember = "member";
            const string comparisonType = "=";
            this.automator = new Automator();

            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType);

            result.ComputedResult.Should().Be("6");
            result.Error.Should().BeFalse();
        }
        
        [TestMethod]
        public void StringValue_DifferentConditionIsTrue_ExecutesTrueAction()
        {
            const string firstMember = "member";
            const string secondMember = "member";
            const string comparisonType = "<>";
            this.automator = new Automator();

            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType);

            result.ComputedResult.Should().Be("4");
            result.Error.Should().BeFalse();
        }

        private ActionResult ProcessStringComparisonActions(string firstMember, string secondMember, string comparisonType)
        {
            this.automator.AuxiliaryActionList.Add(
                new FSAutomatorAction()
                {
                    UniqueID = "trueID",
                    Status = FSAutomatorAction.ActionStatus.Pending,
                    Result = new ActionResult(),
                    ActionObject = new OperateValue()
                    {
                        Operation = "+",
                        Number = 1,
                        ItemToOperateOver = "5"
                    }
                }
            );
            this.automator.AuxiliaryActionList.Add(
                new FSAutomatorAction()
                {
                    UniqueID = "falseID",
                    Status = FSAutomatorAction.ActionStatus.Pending,
                    Result = new ActionResult(),
                    ActionObject = new OperateValue()
                    {
                        Operation = "-",
                        Number = 1,
                        ItemToOperateOver = "5"
                    }
                }
            );

            this.automator.ActionList.Add(
                new FSAutomatorAction()
                {
                    Status = FSAutomatorAction.ActionStatus.Running,
                    Result = new ActionResult()
                }
            );

            this.conditionalAction = new ConditionalAction(firstMember, comparisonType, secondMember, "trueID", "falseID");

            var result = this.conditionalAction.ExecuteAction(automator, null);
            return result;
        }

    }
}