﻿using FSAutomator.Backend.Entities;
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
        public virtual ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            bool error = false;
            this.VariableValue = null;

            var returnResult = "";

            variable = new Variable().GetVariableInformation(this.VariableName);

            if (variable is not null && variable.Type is not null)
            {
                CommonEntities entities = new CommonEntities();

                var dataType = entities.VariableTypes[variable.Type];
                var defineID = entities.DefineIDs[variable.Type];
                var unit = variable.Unit;
                
                semaphore.WaitOne();

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

                retainUntilValueReadyEvent.Set();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while receiving variable: {1}", ex.Message);
            }
        }
    }
}
