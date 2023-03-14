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

        /*
        public Kml kml = new Kml();

        public void AddPoint(string latitude, string longitude, string altitude)
        {
            this.kml.Document.Placemark.Add(new Placemark(latitude, longitude, altitude));
        }
        */

    }

    /*
    [XmlRoot(ElementName = "kml", Namespace = "http://www.opengis.net/kml/2.2")]
    public class Kml
    {
        [XmlElement(ElementName = "Document", Namespace = "http://www.opengis.net/kml/2.2")]
        public Document Document = new Document();
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        public Kml()
        {

        }
    }

    [XmlRoot(ElementName = "Document", Namespace = "http://www.opengis.net/kml/2.2")]
    public class Document
    {
        [XmlElement(ElementName = "Placemark", Namespace = "http://www.opengis.net/kml/2.2")]
        public List<Placemark> Placemark = new List<Placemark>();

        public Document()
        {

        }
    }



    [XmlRoot(ElementName = "Placemark", Namespace = "http://www.opengis.net/kml/2.2")]
    public class Placemark
    {
        [XmlElement(ElementName = "Point", Namespace = "http://www.opengis.net/kml/2.2")]
        public Point Point { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        public Placemark(string latitude, string longitude, string altitude)
        {
            Id = Guid.NewGuid().ToString();
            Point = new Point(latitude, longitude, altitude);
        }

        public Placemark()
        {

        }
    }

    [XmlRoot(ElementName = "Point", Namespace = "http://www.opengis.net/kml/2.2")]
    public class Point
    {
        [XmlElement(ElementName = "altitudeMode", Namespace = "http://www.opengis.net/kml/2.2")]
        public string AltitudeMode { get; set; } = "relativeToGround";
        [XmlElement(ElementName = "coordinates", Namespace = "http://www.opengis.net/kml/2.2")]
        public string Coordinates { get; set; }

        public Point(string latitude, string longitude, string altitude)
        {
            this.Coordinates = $"{longitude},{latitude},{altitude}";
        }

        public Point()
        {

        }
    }
    */

}