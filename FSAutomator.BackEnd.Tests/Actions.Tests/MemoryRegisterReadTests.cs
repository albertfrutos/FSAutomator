using FluentAssertions;
using FSAutomator.Backend.Automators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSAutomator.Backend.Actions.Tests
{
    [TestClass]
    public class MemoryRegisterReadTests
    {
        MemoryRegisterRead Mrd;
        Automator automator;

        const string testValue = "TestValue";
        const string testId = "TestId";

        [TestMethod]
        public void ARegister_ReadingExistingIdWithoutDeletingIt_ValueIsReturnedAndKeptInRegister()
        {
            //Arrange
            const bool removeAfterRead = false;

            this.Mrd = new MemoryRegisterRead()
            {
                RemoveAfterRead = removeAfterRead,
                Id = testId
            };

            this.automator = new Automator()
            {
                MemoryRegisters = new Dictionary<string, string>()
                {
                    { testId ,testValue }
                }
            };

            //Act
            var testResult = this.Mrd.ExecuteAction(this.automator, null);

            //Assert
            this.automator.MemoryRegisters.Should().ContainKey(testId);
            this.automator.MemoryRegisters.Should().ContainValue(testValue);

            testResult.ComputedResult.Should().Be(testValue);
            testResult.Error.Should().BeFalse();
        }

        [TestMethod]
        public void ARegister_ReadingExistingIdAndDeletingIt_ValueIsReturnedAndDeletedFromRegister()
        {
            //Arrange
            const bool removeAfterRead = true;

            this.Mrd = new MemoryRegisterRead()
            {
                RemoveAfterRead = removeAfterRead,
                Id = testId
            };

            this.automator = new Automator()
            {
                MemoryRegisters = new Dictionary<string, string>()
                {
                    { testId ,testValue }
                }
            };

            //Act
            var testResult = this.Mrd.ExecuteAction(this.automator, null);

            //Assert
            this.automator.MemoryRegisters.Should().NotContainKey(testId);

            testResult.ComputedResult.Should().Be(testValue);
            testResult.Error.Should().BeFalse();
        }

        [TestMethod]
        public void ARegister_ReadingNotExistingId_ReturnsError()
        {
            //Arrange
            const bool removeAfterRead = true;

            this.Mrd = new MemoryRegisterRead()
            {
                RemoveAfterRead = removeAfterRead,
                Id = testId
            };

            this.automator = new Automator()
            {
                MemoryRegisters = new Dictionary<string, string>()
            };

            //Act
            var testResult = this.Mrd.ExecuteAction(this.automator, null);

            //Assert
            testResult.ComputedResult.Should().Be(null);
            testResult.Error.Should().BeTrue();
        }
    }
}