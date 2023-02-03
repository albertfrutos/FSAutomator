using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.BackEnd.Configuration
{
    public class AvailableFixedBoolItem
    {
        public string Name { get; set; }
        public bool Value { get; set; }
    }
    public class AvailableFixedBoolItems
    {
        List<AvailableFixedBoolItem> FSAutomatorFixedBoolItems = new List<AvailableFixedBoolItem>();

        public List<AvailableFixedBoolItem> GetAvailableItems()
        {
            var json = File.ReadAllText(@"Configuration\AvailableFixedBoolItems\FSAutomatorFixedBoolItems.json");
            var items = JsonConvert.DeserializeObject<string[]>(json);
            foreach(string item in items)
            {
                FSAutomatorFixedBoolItems.Add(new AvailableFixedBoolItem
                {
                    Name = item,
                    Value = false
                });
            }
            return FSAutomatorFixedBoolItems;
        }
    }


}
