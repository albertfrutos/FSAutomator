using FluentAssertions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace FSAutomator.Backend.Actions.Tests
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
            //Arrange
            this.automator = new Automator();
            this.automator.ActionList.Add(new FSAutomatorAction()
            {
                Status = FSAutomatorAction.ActionStatus.Running,
                Result = new ActionResult()
            });

            this.waiter = new WaitSeconds(numberOfSecondsToWait);
            var stopWatch = new Stopwatch();

            //Act
            stopWatch.Start();

            this.waiter.ExecuteAction(automator, null);

            stopWatch.Stop();

            //Assert
            stopWatch.ElapsedMilliseconds.Should().BeLessThan(maximumNumberOfSecondsForTestExecution * 1000);
        }
    }
}