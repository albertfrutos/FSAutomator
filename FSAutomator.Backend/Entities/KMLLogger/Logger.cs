using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FSAutomator.BackEnd.Entities
{
    public class Logger
    {
        internal List<Point> Points = new List<Point>();

        public void AddPoint(string latitude, string longitude, string altitude)
        {
            this.Points.Add(new Point(latitude, longitude, altitude));
        }
    }
}