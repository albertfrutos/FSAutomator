using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class OperateLastValue : IAction
    {

        public string Operation { get; set; }
        public double Number { get; set; }

        public string ItemToOperateOver { get; set; }

        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            var valueToOperateOn = Utils.GetValueToOperateOnFromTag(sender, this.ItemToOperateOver);

            var isNumeric = Utils.IsNumericDouble(valueToOperateOn);

            if (isNumeric)
            {
                var numToOperate = double.Parse(valueToOperateOn);

                double newVariableValue = numToOperate;

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

                ReturnValueEvent.Invoke(this, newVariableValue.ToString());
            }
            else
            {
                // NOTE : fer un control d'errors, ara es posa això com a previous variable.
                ReturnValueEvent.Invoke(this, "Previous variable value is not a number");
            }

            UnlockNextStep.Invoke(this, null);
        }

    }
}
