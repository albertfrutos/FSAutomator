using FluentAssertions;
using FSAutomator.BackEnd.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSAutomator.Backend.Utilities.Tests
{
    [TestClass]
    public class AvailableFixedBoolItemsTests
    {
        [TestInitialize()]
        public void TestInitialize()
        {

        }

        [TestMethod]
        public void AvailableFixedBoolItems_AvailableFixedBoolItemsNamesAreRetrieved_ReturnsExistingAvailableFixedBoolItems()
        {
            var expectedAvailableFixedBookItems = new List<string>()
            {
                "IsAuxiliary",
                "StopOnError"
            };

            var availableActions = new AvailableFixedBoolItems().GetAvailableItems().Select(i => i.Name).ToList();

            availableActions.Should().BeEquivalentTo(expectedAvailableFixedBookItems);

        }
    }
}