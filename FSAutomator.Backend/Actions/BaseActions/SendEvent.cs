using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.Backend.Actions
{
    public class SendEvent : IAction
    {

        public string EventName { get; set; }
        public string EventValue { get; set; }

        internal SendEvent(string name, string value)
        {
            this.EventName = name;
            this.EventValue = value;
        }

        public SendEvent()
        {

        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {

            this.EventValue = Utils.GetValueToOperateOnFromTag(sender, connection, this.EventValue);

            if (CheckIfEventExists(EventName))
            {
                EVENTS eventToSend = (EVENTS)Enum.Parse(typeof(EVENTS), EventName);

                connection.MapClientEventToSimEvent((Enum)eventToSend, EventName);

                connection.AddClientEventToNotificationGroup(NOTIFICATION_GROUPS.GROUP0, (Enum)eventToSend, true);
                connection.SetNotificationGroupPriority(NOTIFICATION_GROUPS.GROUP0, SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST);

                connection.TransmitClientEvent(0U, (Enum)eventToSend, (uint)Convert.ToDouble(EventValue), (Enum)NOTIFICATION_GROUPS.GROUP0, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
                connection.ClearNotificationGroup(NOTIFICATION_GROUPS.GROUP0);

                return new ActionResult($"{EventName} - {EventValue} has been sent", this.EventValue, false);
            }
            else
            {
                return new ActionResult("Event does not exist", null, true);
            }
        }

        internal static bool CheckIfEventExists(string eventName)
        {
            return Enum.IsDefined(typeof(EVENTS), eventName);
        }

    }
}
