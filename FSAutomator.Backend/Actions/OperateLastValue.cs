using FSAutomator.BackEnd.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class OperateLastValue : IAction
    {

        public string Operation { get; set; }
        public double Number { get; set; }
        public string ItemToOperateOver { get; set; }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            // note la classe es diu operatelastvalue, i aquí deixem triar.

            var valueToOperateOn = Utils.GetValueToOperateOnFromTag(sender, connection, this.ItemToOperateOver);

            var isNumeric = Utils.IsNumericDouble(valueToOperateOn);

            if (isNumeric)
            {
                var numToOperate = double.Parse(valueToOperateOn);

                double newVariableValue;

                switch (Operation)
                {
                    case "+":
                        newVariableValue = numToOperate + Number;
                        break;
                    case "-":
                        newVariableValue = numToOperate - Number;
                        break;
                    case "*":
                        newVariableValue = numToOperate * Number;
                        break;
                    case "/":
                        newVariableValue = numToOperate / Number;
                        break;
                    case "NOT":  //only for booleans
                        newVariableValue = numToOperate == 0 ? 1 : 0;
                        break;
                    default:
                        newVariableValue = numToOperate;
                        break;
                }
                
                return new ActionResult(newVariableValue.ToString(), newVariableValue.ToString());

            }
            else
            {
                return new ActionResult("Previous value is not a number", null);
                // NOTE : fer un control d'errors, ara es posa això com a previous variable.
            }

        }

    }
}
