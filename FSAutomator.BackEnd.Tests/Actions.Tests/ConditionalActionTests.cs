using FluentAssertions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class ConditionalActionTests
    {
        ConditionalAction conditionalAction;

        Automator automator;

        [TestInitialize()]
        public void TestInitialize()
        {
            this.automator = new Automator();

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

        }

        [TestMethod]
        public void StringValue_EqualConditionIsTrue_ExecutesTrueAction()
        {
            //Arrange
            const string firstMember = "member";
            const string secondMember = "member";
            const string comparisonType = "=";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "trueID", "falseID");

            //Assert
            result.ComputedResult.Should().Be("6");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void StringValue_DifferentConditionISFalse_ExecutesFalseAction()
        {
            //Arrange
            const string firstMember = "member";
            const string secondMember = "member";
            const string comparisonType = "<>";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "trueID", "falseID");

            //Assert
            result.ComputedResult.Should().Be("4");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void StringValue_InvalidComparison_ReturnsError()
        {
            //Arrange
            const string firstMember = "member";
            const string secondMember = "member";
            const string comparisonType = "InvalidComparison";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "trueID", "falseID");

            //Assert
            result.ComputedResult.Should().BeNull();
            result.Error.Should().BeTrue();
        }

        [TestMethod]
        public void NumericValue_DifferentConditionIsTrue_ExecutesTrueAction()
        {
            //Arrange
            const string firstMember = "4";
            const string secondMember = "5";
            const string comparisonType = "<>";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "trueID", "falseID");

            //Assert
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void ComparisonValues_TrueAndFalseUniqueIDsAreEmpty_ReturnsError()
        {
            //Arrange
            const string firstMember = "4";
            const string secondMember = "5";
            const string comparisonType = "<>";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "", "");

            //Assert
            result.ComputedResult.Should().BeNull();
            result.VisibleResult.Should().Contain("Both true and false UniqueID for execution are missing");
            result.Error.Should().BeTrue();
        }

        [TestMethod]
        public void ComparisonValues_TrueUniqueIDIsEmpty_ReturnsError()
        {
            //Arrange
            const string firstMember = "4";
            const string secondMember = "5";
            const string comparisonType = "<>";
            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "", "falseID");

            //Assert
            result.ComputedResult.Should().BeNull();
            result.Error.Should().BeTrue();
        }

        [TestMethod]
        public void NumericValue_EqualsConditionIsTrue_ExecutesTrueAction()
        {
            //Arrange
            const string firstMember = "4";
            const string secondMember = "4";
            const string comparisonType = "=";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "trueID", "falseID");

            //Assert
            result.ComputedResult.Should().Be("6");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void NumericValue_HigherThanConditionIsTrue_ExecutesTrueAction()
        {
            //Arrange
            const string firstMember = "5";
            const string secondMember = "4";
            const string comparisonType = ">";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "trueID", "falseID");

            //Assert
            result.ComputedResult.Should().Be("6");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void NumericValue_LowerThanConditionIsFalse_ExecutesTrueAction()
        {
            //Arrange
            const string firstMember = "5";
            const string secondMember = "4";
            const string comparisonType = "<";

            //Act
            ActionResult result = ProcessStringComparisonActions(firstMember, secondMember, comparisonType, "trueID", "falseID");

            //Assert
            result.ComputedResult.Should().Be("4");
            result.Error.Should().BeFalse();
        }

        private ActionResult ProcessStringComparisonActions(string firstMember, string secondMember, string comparisonType, string trueID, string falseID)
        {

            this.conditionalAction = new ConditionalAction(firstMember, comparisonType, secondMember, trueID, falseID);

            var result = this.conditionalAction.ExecuteAction(automator, null);
            return result;
        }

    }
}