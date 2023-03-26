/* THIS IS NOT PART OF FSAutomator, THIS IS AN AUXILIARY PROJECT TO HELP GENERATE METHODS FOR THE MANAGERS FOR THE DLLAUTOMATION ACTION*/

using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Configuration;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text;

public class MyObject
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class Program
{
    public static void Main()
    {
        var json = File.ReadAllText(@"C:\Users\Albert\source\repos\albertfrutos\FSAutomator\FSAutomator.UI\bin\Debug\net6.0-windows\Automations\atc id.json");

        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);

        List<FSAutomatorAction> list = new List<FSAutomatorAction>();

        foreach (Action action in myDeserializedClass.Actions)
        {
            Type actionType = Type.GetType(String.Format("FSAutomator.Backend.Actions.{0}", action.Name));

            list.Add(new FSAutomatorAction()
            {
                Name = action.Name,
                ActionObject = Convert.ChangeType(action.Parameters, Type.GetType(String.Format("FSAutomator.Backend.Actions.{0}", action.Name)))
            });
        }

        return;

        var b = ApplicationConfig.GetInstance;
        var a = new Program();
        a.EventsUpdater();

    }

    public void MethodCreator(List<string> valueList, string methodTemplate, string separator, string filename)
    {
        var items = valueList.Distinct();


        foreach (string value in valueList)
        {
            string[] words = value.Split(new string[1] { separator }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder(words.Sum(x => x.Length));

            foreach (string word in words)
            {
                //Console.WriteLine(word[0].ToString());
                sb.Append(word[0].ToString().ToUpper() + word.Substring(1).ToLower());
            }
            var methodName = sb.ToString();

            string myMethod = methodTemplate.Replace("<m_name>", methodName).Replace("<item_name>", value);

            File.AppendAllText(filename, myMethod);
        }
    }

    public void VarsCreator()
    {
        List<string> varList = new List<string>() { "AUTOPILOT AIRSPEED ACQUISITION", "AUTOPILOT AIRSPEED HOLD", "AUTOPILOT AIRSPEED HOLD CURRENT", "AUTOPILOT AIRSPEED HOLD VAR", "AUTOPILOT AIRSPEED MAX CALCULATED", "AUTOPILOT AIRSPEED MIN CALCULATED", "AUTOPILOT ALT RADIO MODE", "AUTOPILOT ALTITUDE ARM", "AUTOPILOT ALTITUDE LOCK", "AUTOPILOT ALTITUDE LOCK VAR", "AUTOPILOT ALTITUDE MANUALLY TUNABLE", "AUTOPILOT ALTITUDE SLOT INDEX", "AUTOPILOT APPROACH ACTIVE", "AUTOPILOT APPROACH ARM        ", "AUTOPILOT APPROACH CAPTURED", "AUTOPILOT APPROACH HOLD", "AUTOPILOT APPROACH IS LOCALIZER", "AUTOPILOT ATTITUDE HOLD", "AUTOPILOT AVAILABLE", "AUTOPILOT AVIONICS MANAGED", "AUTOPILOT BACKCOURSE HOLD", "AUTOPILOT BANK HOLD", "AUTOPILOT BANK HOLD REF", "AUTOPILOT CRUISE SPEED HOLD", "AUTOPILOT DEFAULT PITCH MODE", "AUTOPILOT DEFAULT ROLL MODE", "AUTOPILOT DISENGAGED", "AUTOPILOT FLIGHT DIRECTOR ACTIVE", "AUTOPILOT FLIGHT DIRECTOR BANK", "AUTOPILOT FLIGHT DIRECTOR BANK EX1", "AUTOPILOT FLIGHT DIRECTOR PITCH", "AUTOPILOT FLIGHT DIRECTOR PITCH EX1", "AUTOPILOT FLIGHT LEVEL CHANGE", "AUTOPILOT GLIDESLOPE ACTIVE", "AUTOPILOT GLIDESLOPE ARM", "AUTOPILOT GLIDESLOPE HOLD", "AUTOPILOT HEADING LOCK", "AUTOPILOT HEADING LOCK DIR", "AUTOPILOT HEADING MANUALLY TUNABLE", "AUTOPILOT HEADING SLOT INDEX", "AUTOPILOT MACH HOLD", "AUTOPILOT MACH HOLD VAR", "AUTOPILOT MANAGED INDEX", "AUTOPILOT MANAGED SPEED IN MACH", "AUTOPILOT MANAGED THROTTLE ACTIVE", "AUTOPILOT MASTER", "AUTOPILOT MAX BANK", "AUTOPILOT MAX BANK ID", "AUTOPILOT MAX SPEED HOLD", "AUTOPILOT NAV1 LOCK", "AUTOPILOT NAV SELECTED", "AUTOPILOT PITCH HOLD", "AUTOPILOT PITCH HOLD REF", "AUTOPILOT RPM HOLD", "AUTOPILOT RPM HOLD VAR", "AUTOPILOT RPM SLOT INDEX", "AUTOPILOT SPEED SETTING", "AUTOPILOT SPEED SLOT INDEX", "AUTOPILOT TAKEOFF POWER ACTIVE", "AUTOPILOT THROTTLE ARM", "AUTOPILOT THROTTLE MAX THRUST", "AUTOPILOT VERTICAL HOLD", "AUTOPILOT VERTICAL HOLD VAR", "AUTOPILOT VS SLOT INDEX", "AUTOPILOT WING LEVELER", "AUTOPILOT YAW DAMPER", "AUTOPILOT AIRSPEED ACQUISITION", "AUTOPILOT AIRSPEED HOLD", "AUTOPILOT AIRSPEED HOLD CURRENT", "AUTOPILOT AIRSPEED HOLD VAR", "AUTOPILOT AIRSPEED MAX CALCULATED", "AUTOPILOT AIRSPEED MIN CALCULATED", "AUTOPILOT ALT RADIO MODE", "AUTOPILOT ALTITUDE ARM", "AUTOPILOT ALTITUDE LOCK", "AUTOPILOT ALTITUDE LOCK VAR", "AUTOPILOT ALTITUDE MANUALLY TUNABLE", "AUTOPILOT ALTITUDE SLOT INDEX", "AUTOPILOT APPROACH ACTIVE", "AUTOPILOT APPROACH CAPTURED", "AUTOPILOT APPROACH HOLD", "AUTOPILOT APPROACH IS LOCALIZER", "AUTOPILOT ATTITUDE HOLD", "AUTOPILOT AVAILABLE", "AUTOPILOT AVIONICS MANAGED", "AUTOPILOT BACKCOURSE HOLD", "AUTOPILOT BANK HOLD", "AUTOPILOT BANK HOLD REF", "AUTOPILOT CRUISE SPEED HOLD", "AUTOPILOT DEFAULT PITCH MODE", "AUTOPILOT DEFAULT ROLL MODE", "AUTOPILOT DISENGAGED", "AUTOPILOT FLIGHT DIRECTOR ACTIVE", "AUTOPILOT FLIGHT DIRECTOR BANK", "AUTOPILOT FLIGHT DIRECTOR BANK EX1", "AUTOPILOT FLIGHT DIRECTOR PITCH", "AUTOPILOT FLIGHT DIRECTOR PITCH EX1", "AUTOPILOT FLIGHT LEVEL CHANGE", "AUTOPILOT GLIDESLOPE ACTIVE", "AUTOPILOT GLIDESLOPE ARM", "AUTOPILOT GLIDESLOPE HOLD", "AUTOPILOT HEADING LOCK", "AUTOPILOT HEADING LOCK DIR", "AUTOPILOT HEADING MANUALLY TUNABLE", "AUTOPILOT HEADING SLOT INDEX", "AUTOPILOT MACH HOLD", "AUTOPILOT MACH HOLD VAR", "AUTOPILOT MANAGED INDEX", "AUTOPILOT MANAGED SPEED IN MACH", "AUTOPILOT MANAGED THROTTLE ACTIVE", "AUTOPILOT MASTER", "AUTOPILOT MAX BANK", "AUTOPILOT MAX BANK ID", "AUTOPILOT MAX SPEED HOLD", "AUTOPILOT NAV1 LOCK", "AUTOPILOT NAV SELECTED", "AUTOPILOT PITCH HOLD", "AUTOPILOT PITCH HOLD REF", "AUTOPILOT RPM HOLD", "AUTOPILOT RPM HOLD VAR", "AUTOPILOT RPM SLOT INDEX", "AUTOPILOT SPEED SETTING", "AUTOPILOT SPEED SLOT INDEX", "AUTOPILOT TAKEOFF POWER ACTIVE", "AUTOPILOT THROTTLE ARM", "AUTOPILOT THROTTLE MAX THRUST", "AUTOPILOT VERTICAL HOLD", "AUTOPILOT VERTICAL HOLD VAR", "AUTOPILOT VS SLOT INDEX", "AUTOPILOT WING LEVELER", "AUTOPILOT YAW DAMPER" };
        string methodTemplate = "        public string Get<m_name>()\n        {\n            var result = GetVariable(\"Get<var_name>\").ComputedResult;\n            return result;\n        }\n\n";

        MethodCreator(varList, methodTemplate, " ", "output_vars.txt");
    }


    #region Events

    public void EventsUpdater()
    {
        var url = "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Autopilot_Flight_Assist_Events.htm";
        List<string> events = new List<string>();

        HtmlWeb web = new HtmlWeb();


        HtmlDocument doc = web.Load(url);
        var group = Path.GetFileNameWithoutExtension(url);
        events.AddRange(ParseWeb(doc, group));

        string methodTemplate = "        public string SetEvent<m_name>(string value)\n        {\n            var result = SendEvent(\"<item_name>\",value).ComputedResult;\n            return result;\n        }\n\n";

        MethodCreator(events, methodTemplate, "_", "output_events.txt");


    }

    private List<string> ParseWeb(HtmlDocument doc, string variableGroup)
    {
        List<string> events = new List<string>();

        foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//*/table"))
        {

            foreach (HtmlNode row in table.SelectNodes("tbody/tr").Skip(1))
            {
                var cells = row.SelectNodes("td").ToList();
                if (cells.Count >= 3)
                {
                    var mEvent = cells[0].InnerText;

                    foreach (string multievent in mEvent.Split(new string[] { "or", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (multievent.Trim() != string.Empty)
                        {
                            events.Add(cleanString(multievent));

                        }
                    }
                }
            }
        }

        return events;
    }

    private string cleanString(string text)
    {
        return text.Replace("\n", string.Empty).Trim(' ');
    }

    # endregion

}

// 
public class Action
{
    public string Name { get; set; }
    public object Parameters { get; set; }
}

public class Root
{
    public List<Action> Actions { get; set; }
}