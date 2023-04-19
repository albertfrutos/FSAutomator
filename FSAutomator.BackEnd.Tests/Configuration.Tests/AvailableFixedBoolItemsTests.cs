using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Reflection;

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

            var availableActions = new AvailableFixedBoolItems().GetAvailableItems().Select( i => i.Name).ToList();

            availableActions.Should().BeEquivalentTo(expectedAvailableFixedBookItems);

        }
    }
}