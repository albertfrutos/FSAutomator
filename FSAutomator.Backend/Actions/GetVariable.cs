using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using Newtonsoft.Json;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.Backend.Actions
{
    public class GetVariable
    {
        public string VariableName { get; set; }

        public string VariableValue = null;

        [JsonIgnore]
        public AutoResetEvent evento = new AutoResetEvent(false);

        private Variable variable;

        private SimConnect connection;

        public GetVariable()
        {

        }

        public GetVariable(string name)
        {
            VariableName = name;
            VariableValue = null;
        }
        public string ExecuteAction(object sender, SimConnect connection)
        {
            this.connection = connection;

            variable = new Variable().GetVariableInformation(this.VariableName);

            if (!(variable is null) && !(variable.Type is null))
            {
                CommonEntities entities = new CommonEntities();

                var dataType = entities.VariableTypes[variable.Type];
                var defineID = entities.DefineIDs[variable.Type];
                var unit = variable.Unit;

                if (variable.Type == "string")
                {
                    connection.AddToDataDefinition(defineID, this.VariableName, "", dataType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    connection.RegisterDataDefineStruct<StringType>(DEFINITIONS.StringType);
                }
                else if (variable.Type == "num")
                {
                    connection.AddToDataDefinition(defineID, this.VariableName, unit, dataType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    connection.RegisterDataDefineStruct<NumType>(DEFINITIONS.NumType);
                }
                else if (variable.Type == "bool")
                {
                    connection.AddToDataDefinition(defineID, this.VariableName, unit, dataType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    connection.RegisterDataDefineStruct<BoolType>(DEFINITIONS.BoolType);
                }

                connection.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(Simconnect_OnRecvSimobjectDataBytype);

                connection.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, defineID, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
                connection.ClearDataDefinition(defineID);

                evento.WaitOne();
            }

            return this.VariableValue;

        }

        private void Simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (this.VariableValue != null)
            {
                return;
            }

            try
            {
                if (variable.Type == "string")
                {
                    StringType result = (StringType)data.dwData[0];
                    this.VariableValue = result.stringVar;
                }
                else if (variable.Type == "num")
                {
                    NumType result = (NumType)data.dwData[0];
                    this.VariableValue = result.numVar.ToString();
                }
                else if (variable.Type == "bool")
                {
                    BoolType result = (BoolType)data.dwData[0];
                    this.VariableValue = result.boolVar.ToString();
                }

                evento.Set();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while receiving variable: {1}", ex.Message);
            }
        }
    }
}
