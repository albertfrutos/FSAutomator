using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.BackEnd.Entities
{
    public class ActionResult
    {
        public string VisibleResult { get; set; }
        public string ComputedResult { get; set; }

        public ActionResult()
        {

        }
        public ActionResult(string visibleResult, string computedResult)
        {
            this.VisibleResult = visibleResult;
            this.ComputedResult = computedResult;
        }
    }
}
