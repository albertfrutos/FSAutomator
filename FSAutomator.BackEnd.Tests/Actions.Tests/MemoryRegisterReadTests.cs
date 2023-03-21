using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FSAutomator.BackEnd.Tests
{
    [TestClass]
    public class MemoryRegisterReadTests
    {
        MemoryRegisterRead mrd;
        Automator automator;

        const string testValue = "TestValue";
        const string testId = "TestId";

        [TestMethod]
        public void ARegister_ReadingExistingIdWithoutDeletingIt_ValueIsReturnedAndKeptInRegister()
        {
            const bool removeAfterRead = false;

            this.mrd = new MemoryRegisterRead()
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

            var testResult = this.mrd.ExecuteAction(this.automator, null);

            this.automator.MemoryRegisters.Should().ContainKey(testId);
            this.automator.MemoryRegisters.Should().ContainValue(testValue);

            testResult.ComputedResult.Should().Be(testValue);
            testResult.Error.Should().BeFalse();
        }

        [TestMethod]
        public void ARegister_ReadingExistingIdAndDeletingIt_ValueIsReturnedAndDeletedFromRegister()
        {
            const bool removeAfterRead = true;

            this.mrd = new MemoryRegisterRead()
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

            var testResult = this.mrd.ExecuteAction(this.automator, null);

            this.automator.MemoryRegisters.Should().NotContainKey(testId);

            testResult.ComputedResult.Should().Be(testValue);
            testResult.Error.Should().BeFalse();
        }

        [TestMethod]
        public void ARegister_ReadingNotExistingId_ReturnsError()
        {
            const bool removeAfterRead = true;

            this.mrd = new MemoryRegisterRead()
            {
                RemoveAfterRead = removeAfterRead,
                Id = testId
            };

            this.automator = new Automator()
            {
                MemoryRegisters = new Dictionary<string, string>()
            };

            var testResult = this.mrd.ExecuteAction(this.automator, null);

            testResult.ComputedResult.Should().Be(null);
            testResult.Error.Should().BeTrue();
        }
    }
}