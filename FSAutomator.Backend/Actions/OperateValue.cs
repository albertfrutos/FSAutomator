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

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var valueToOperateOn = Utils.GetValueToOperateOnFromTag(sender, connection, this.ItemToOperateOver);

            var isNumeric = Utils.IsNumericDouble(valueToOperateOn);

            if (isNumeric)
            {
                var numToOperate = double.Parse(valueToOperateOn);

                var newVariableValue = Operation switch
                {
                    "+" => numToOperate + Number,
                    "-" => numToOperate - Number,
                    "*" => numToOperate * Number,
                    "/" => numToOperate / Number,
                    "NOT" => numToOperate == 0 ? 1 : 0,     //only for booleans
                    _ => numToOperate,
                };

                return new ActionResult(newVariableValue.ToString(), newVariableValue.ToString(), false);
            }
            else
            {
                return new ActionResult("Previous value is not a number", null, true);
            }

        }

    }
}
