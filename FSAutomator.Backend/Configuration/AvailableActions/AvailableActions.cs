using FSAutomator.Backend.Utilities;

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

        public string Type { get; set; } = "";
    }

    public class AvailableActions
    {
        public List<AvailableAction> FSAutomatorAvailableActions { get; set; }

        public AvailableActions GetAvailableActions()
        {
            var availableActions = Utils.GetExistingActions();

            FSAutomatorAvailableActions = new List<AvailableAction>();

            foreach (var action in availableActions)
            {
                FSAutomatorAvailableActions.Add(new AvailableAction()
                {
                    Name = action,
                    Parameters = Type.GetType("FSAutomator.Backend.Actions." + action).GetProperties().Select(x => new Parameter() { Name = x.Name, Type = x.PropertyType.Name.ToString() }).ToList()
                });
            }

            return this;
        }

    }
}
