using FluentAssertions;
using FSAutomator.Backend.Automators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class MemoryRegisterWriteTests
    {
        MemoryRegisterWrite Mrw;

        [TestMethod]
        public void ARegister_AddValueWithNewId_ValueIsStored()
        {
            //Arrange
            const string testValue = "TestValue";
            const string testId = "TestId";

            this.Mrw = new MemoryRegisterWrite()
            {
                Value = testValue,
                Id = testId
            };

            var automator = new Automator()
            {
                MemoryRegisters = new Dictionary<string, string>()
            };

            //Act
            var testResult = Mrw.ExecuteAction(automator, null);

            //Assert
            automator.MemoryRegisters.Should().ContainKey(testId);
            automator.MemoryRegisters.Should().ContainValue(testValue);

            testResult.ComputedResult.Should().Be(testValue);
            testResult.Error.Should().BeFalse();
        }

        [TestMethod]
        public void ARegister_AddValueWithExistingId_AnErrorIsReported()
        {
            //Arrange
            const string testValue = "TestValue";
            const string testId = "TestId";

            this.Mrw = new MemoryRegisterWrite()
            {
                Value = testValue,
                Id = testId
            };

            var automator = new Automator()
            {
                MemoryRegisters = new Dictionary<string, string>()
                {
                    { testId ,testValue }
                }
            };

            //Act
            var testResult = this.Mrw.ExecuteAction(automator, null);

            //Assert
            testResult.ComputedResult.Should().BeNull();
            testResult.Error.Should().BeTrue();
        }
    }
}