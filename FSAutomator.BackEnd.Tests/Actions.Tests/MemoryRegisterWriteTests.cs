using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FSAutomator.BackEnd.Tests
{
    [TestClass]
    public class MemoryRegisterWriteTests
    {
        [TestMethod]
        public void ARegister_AddValueWithNewId_ValueIsStored()
        {
            const string testValue = "TestValue";
            const string testId = "TestId";

            MemoryRegisterWrite mrd = new MemoryRegisterWrite()
            {
                Value = testValue,
                Id = testId
            };

            var automator = new Automator()
            {
                MemoryRegisters = new Dictionary<string, string>()
            };

            var testResult = mrd.ExecuteAction(automator, null);

            automator.MemoryRegisters.Should().ContainKey(testId);
            automator.MemoryRegisters.Should().ContainValue(testValue);

            testResult.ComputedResult.Should().Be(testValue);
            testResult.Error.Should().BeFalse();
        }
        
        [TestMethod]
        public void ARegister_AddValueWithExistingId_AnErrorIsReported()
        {
            const string testValue = "TestValue";
            const string testId = "TestId";

            MemoryRegisterWrite mrd = new MemoryRegisterWrite()
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

            var testResult = mrd.ExecuteAction(automator, null);

            testResult.ComputedResult.Should().BeNull();
            testResult.Error.Should().BeTrue();
        }
    }
}