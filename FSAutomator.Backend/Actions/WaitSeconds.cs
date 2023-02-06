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


        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            totalSeconds = Convert.ToDouble(this.WaitTime);

            waitTimer = new System.Timers.Timer(1000);

            waitTimer.Elapsed += delegate { OnTick(sender); };
            
            waitTimer.Start();

            evento.WaitOne();

            return new ActionResult($"Awaited for {WaitTime} seconds", WaitTime);
        }

        private void OnTick(object sender)
        {
            TimeSpan t = TimeSpan.FromSeconds(totalSeconds);
            var remainingTime = $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
            
            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            var CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();

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
