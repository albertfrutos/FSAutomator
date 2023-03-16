using FSAutomator.BackEnd.Configuration;
using Newtonsoft.Json;

namespace FSAutomator.Backend.Entities
{
    internal class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }

        internal List<Variable> LoadVariables()
        {
            var variables = File.ReadAllText(Path.Combine(ApplicationConfig.GetInstance.FilesFolder,"Variables.json"));
            return JsonConvert.DeserializeObject<List<Variable>>(variables);
        }
        public Variable GetVariableInformation(string variableName)
        {
            var variablesList = LoadVariables();
            return variablesList.FirstOrDefault(x => x.Name == variableName);
        }
    }
}
