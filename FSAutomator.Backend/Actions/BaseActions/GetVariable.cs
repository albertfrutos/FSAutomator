using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;
using Microsoft.FlightSimulator.SimConnect;
using Newtonsoft.Json;
using System.Diagnostics;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.Backend.Actions
{
    public class GetVariable : IGetVariable
    {
        public string VariableName { get; set; }

        public string VariableValue = null;

        [JsonIgnore]
        public AutoResetEvent retainUntilValueReadyEvent = new AutoResetEvent(false);

        private Variable variable;

        static Semaphore semaphore = new Semaphore(1, 1);
        
        public GetVariable()
        {

        }

        internal GetVariable(string variableName)
        {
            VariableName = variableName;
            VariableValue = null;
        }
        public virtual ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
        {
            bool error = false;
            this.VariableValue = null;
            var returnResult = "";

            CommonEntities entities = new CommonEntities();

            variable = new Variable().GetVariableInformation(this.VariableName);

            if (variable is not null && variable.Type is not null)
            {
                var dataType = entities.VariableTypes[variable.Type];
                var defineID = entities.DefineIDs[variable.Type];
                var unit = variable.Unit;
                
                semaphore.WaitOne();

                connection.AddToDataDefinition(defineID, this.VariableName, unit, dataType, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                switch (variable.Type)
                {
                    case "string":
                        connection.RegisterDataDefineStruct<StringType>(DEFINITIONS.StringType);
                        break;
                    case "num":
                        connection.RegisterDataDefineStruct<NumType>(DEFINITIONS.NumType);
                        break;
                    case "bool":
                        connection.RegisterDataDefineStruct<BoolType>(DEFINITIONS.BoolType);
                        break;
                }

                connection.SubscribeToRecvSimobjectDataBytypeEventHandler(Simconnect_OnRecvSimobjectDataBytype);

                Trace.WriteLine(this.VariableName);

                connection.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, defineID, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
                connection.ClearDataDefinition(defineID);

                retainUntilValueReadyEvent.WaitOne();
                semaphore.Release();

                returnResult = $"Variable value is {this.VariableValue }";
            }
            else
            {
                error = true;
                returnResult = "Variable does not exist.";
            }

            return new ActionResult(returnResult, this.VariableValue, error);
        }

        private void Simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (this.VariableValue != null)
            {
                return;
            }

            try
            {
                dynamic result = null;

                switch (variable.Type)
                {
                    case "string":
                        result = (StringType)data.dwData[0];
                        break;
                    case "num":
                        result = (NumType)data.dwData[0];
                        break;
                    case "bool":
                        result = (BoolType)data.dwData[0];
                        break;
                }

                this.VariableValue = result.value.ToString();

                retainUntilValueReadyEvent.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while receiving variable: {1}", ex.Message);
            }
        }
    }
}
