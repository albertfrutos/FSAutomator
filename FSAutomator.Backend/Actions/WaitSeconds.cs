using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Timers;

namespace FSAutomator.Backend.Actions
{
    public class WaitSeconds
    {
        public string WaitTime { get; set; }

        private System.Timers.Timer waitTimer;

        double totalSeconds;
        DateTime startTime;


        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            totalSeconds = Convert.ToDouble(this.WaitTime);

            startTime = DateTime.Now;

            waitTimer = new System.Timers.Timer(1000);

            waitTimer.Elapsed += delegate { OnTick(sender, ReturnValueEvent, UnlockNextStep); };
            waitTimer.Start();


            
        }

        private void OnTick(object sender, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            TimeSpan t = TimeSpan.FromSeconds(totalSeconds);
            var remainingTime = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
            
            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("l_ActionList").GetValue(sender);
            var CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();
            CurrentAction.Result = remainingTime;

            if (totalSeconds == 0)
            {
                waitTimer.Stop();
                ReturnValueEvent.Invoke(this, String.Format("Awaited for {0} seconds", WaitTime));
                UnlockNextStep.Invoke(this, null);
            }
            else
            {
                totalSeconds--;
            }
        }
    }
}
