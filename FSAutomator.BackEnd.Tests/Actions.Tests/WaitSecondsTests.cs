using FluentAssertions;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.BackEnd.Tests
{
    [TestClass]
    public class WaitSecondsTests
    {
        WaitSeconds waiter;
        Automator automator;

        const int numberOfSecondsToWait = 2;
        const long maximumNumberOfSecondsForTestExecution = 4;

        [TestMethod]
        public void NumberOfSeconds_WaitIsTriggered_TheSpecifiedSecondsAreWaited()
        {
            this.automator = new Automator();
            this.automator.ActionList.Add(new FSAutomatorAction()
            {
                Status = FSAutomatorAction.ActionStatus.Running,
                Result = new ActionResult()
            });

            this.waiter = new WaitSeconds(numberOfSecondsToWait);
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var result = this.waiter.ExecuteAction(automator, null);

            stopWatch.Stop();

            stopWatch.ElapsedMilliseconds.Should().BeLessThan(maximumNumberOfSecondsForTestExecution*1000);
        }
    }
}