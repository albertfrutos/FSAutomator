﻿using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.Actions
{
    public class WaitSeconds : IAction
    {
        public int WaitTime { get; set; }

        private System.Timers.Timer waitTimer;

        AutoResetEvent evento = new AutoResetEvent(false);

        double totalSeconds;

        public WaitSeconds()
        {

        }

        internal WaitSeconds(int time)
        {
            WaitTime = time;
        }

        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
        {
            totalSeconds = Convert.ToDouble(this.WaitTime);

            waitTimer = new System.Timers.Timer(1000);

            waitTimer.Elapsed += delegate { OnTick(sender); };

            waitTimer.Start();

            evento.WaitOne();

            return new ActionResult($"Awaited for {WaitTime} seconds", WaitTime.ToString(), false);
        }

        private void OnTick(object sender)
        {
            TimeSpan t = TimeSpan.FromSeconds(totalSeconds);
            var remainingTime = $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

            var actionsList = (sender as Automator).ActionList;
            var CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == FSAutomatorAction.ActionStatus.Running).First();

            CurrentAction.Result.VisibleResult = remainingTime;

            if (totalSeconds == 0)
            {
                waitTimer.Stop();
                evento.Set();
                return;
            }
            else
            {
                totalSeconds--;
            }
        }
    }
}
