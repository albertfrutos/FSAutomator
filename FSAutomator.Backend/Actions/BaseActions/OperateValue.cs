using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class OperateValue : IAction
    {

        public string Operation { get; set; }
        public double Number { get; set; }
        public string ItemToOperateOver { get; set; } 

        public OperateValue()
        {

        }
        
        public OperateValue(string operation, double number, string itemToOperateOver)
        {
            this.Operation = operation;
            this.Number = number;
            this.ItemToOperateOver = itemToOperateOver;
        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var valueToOperateOn = Utils.GetValueToOperateOnFromTag(sender, connection, this.ItemToOperateOver);

            var isNumeric = Utils.IsNumericDouble(valueToOperateOn);
            if (isNumeric)
            {
                var numToOperate = double.Parse(valueToOperateOn);
                var newVariableValue = numToOperate;
                var isUnsupportedOperation = false;

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
                    case "NOT":
                        newVariableValue = numToOperate == 0 ? 1 : 0;     //only for booleans
                        break;
                    default:
                        newVariableValue = numToOperate;
                        isUnsupportedOperation = true;
                        break;
                };

                return new ActionResult(newVariableValue.ToString(), newVariableValue.ToString(), isUnsupportedOperation);
            }
            else
            {
                return new ActionResult("Previous value is not a number", null, true);
            }

        }

    }
}
