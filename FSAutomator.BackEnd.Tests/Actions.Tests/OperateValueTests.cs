using FluentAssertions;
using FSAutomator.Backend.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class OperateValueTests
    {
        OperateValue Operator;

        [TestMethod]
        public void NumericValue_AddNumber_CorrectResultIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("+", 2, "8");

            //Assert
            result.ComputedResult.Should().Be("10");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void NumericValue_SubtractNumber_CorrectResultIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("-", 2, "8");

            //Assert
            result.ComputedResult.Should().Be("6");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void NumericValue_MultiplyNumber_CorrectResultIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("*", 2, "8");

            //Assert
            result.ComputedResult.Should().Be("16");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void NumericValue_DivideNumber_CorrectResultIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("/", 2, "8");

            //Assert
            result.ComputedResult.Should().Be("4");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void BooleanNumericValue1True_NOT_CorrectResultIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("NOT", 0, "1");

            //Assert
            result.ComputedResult.Should().Be("0");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void BooleanNumericValue0False_NOT_CorrectResultIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("NOT", 0, "0");

            //Assert
            result.ComputedResult.Should().Be("1");
            result.Error.Should().BeFalse();
        }

        [TestMethod]
        public void NumericValue_InvalidOperation_ErrorIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("INVALID_OPERATION", 2, "8");

            //Assert
            result.ComputedResult.Should().Be("8");
            result.Error.Should().BeTrue();
        }

        [TestMethod]
        public void NonNumericValue_AnyOperation_ErrorIsReturned()
        {
            //Arrange

            //Act
            var result = GetOperateValueResult("ANY_OPERATION", 2, "ThisIsNotNumeric");

            //Assert
            result.ComputedResult.Should().BeNull();
            result.Error.Should().BeTrue();
        }

        private ActionResult GetOperateValueResult(string operation, double number, string itemToOperateOn)
        {
            this.Operator = new OperateValue()
            {
                Operation = operation,
                Number = number,
                ItemToOperateOver = itemToOperateOn
            };

            var testResult = this.Operator.ExecuteAction(null, null);
            return testResult;
        }
    }
}