using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Timers;

namespace FSAutomator.Backend.Actions
{
    public class WaitSeconds : IAction
    {
        public string WaitTime { get; set; }

        private System.Timers.Timer waitTimer;
        AutoResetEvent evento = new AutoResetEvent(false);
        double totalSeconds;
        DateTime startTime;


        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            totalSeconds = Convert.ToDouble(this.WaitTime);

            startTime = DateTime.Now;

            waitTimer = new System.Timers.Timer(1000);

            waitTimer.Elapsed += delegate { OnTick(sender); };
            
            waitTimer.Start();

            evento.WaitOne();

            return new ActionResult($"Awaited for {WaitTime} seconds", WaitTime);
        }

        private void OnTick(object sender)
        {
            TimeSpan t = TimeSpan.FromSeconds(totalSeconds);
            var remainingTime = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
            
            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            var CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();

            CurrentAction.Result = remainingTime;

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
