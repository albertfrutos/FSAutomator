using Newtonsoft.Json;

namespace FSAutomator.BackEnd.Configuration
{
    public class AvailableAction
    {
        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; } = "";
    }

    public class AvailableActions
    {
        public List<AvailableAction> FSAutomatorAvailableActions { get; set; }

        public AvailableActions GetAvailableActions()
        {
            var json = File.ReadAllText(@"Configuration\AvailableActions\FSAutomatorAvailableActions.json");
            var actions = JsonConvert.DeserializeObject<AvailableActions>(json);
            return actions;
        }

        public List<string> GetAvailableActionsName()
        {
            var availableActions = GetAvailableActions().FSAutomatorAvailableActions.Select(x => x.Name).ToList();
            return availableActions;
        }

    }
}
