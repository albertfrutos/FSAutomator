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
        // note update json with properties, at least review
        public AvailableActions GetAvailableActions()
        {
            var availableActions = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && t.IsNested == false && t.Namespace == "FSAutomator.Backend.Actions")
                       .Select(T => T.Name)
                       .ToList();


            FSAutomatorAvailableActions = new List<AvailableAction>();

            foreach (var action in availableActions)
            {

                FSAutomatorAvailableActions.Add(new AvailableAction()
                {
                    Name = action,
                    Parameters = Type.GetType("FSAutomator.Backend.Actions." + action).GetProperties().Select(x => new Parameter() { Name = x.Name }).ToList()
                });

            }


            //var json = File.ReadAllText(@"Configuration\AvailableActions\FSAutomatorAvailableActions.json");
            //var actions = JsonConvert.DeserializeObject<AvailableActions>(json);


            return this;
        }

    }
}
