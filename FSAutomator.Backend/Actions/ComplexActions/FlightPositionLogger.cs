using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using FSAutomator.BackEnd.Entities;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FSAutomator.Backend.Actions
{
    public class FlightPositionLogger
    {
        public string LoggingTimeSeconds { get; set; } = null;
        public string LoggingPeriodSeconds { get; set; } = "1";
        public string LogInNoLockingBackgroundMode { get; set; } = "false";
 
        private System.Timers.Timer loggingTimeTimer;

        private bool continueLogging = true;

        private EventHandler<bool> finishLoggingEvent;

        private string KmlContent = "";

        public FlightPositionLogger()
        {

        }

        public FlightPositionLogger(string loggingTimeSeconds, string loggingPeriodSeconds, string logInNoLockingBackgroundMode = "false")
        {
            this.LoggingTimeSeconds = loggingTimeSeconds;
            this.LoggingPeriodSeconds = loggingPeriodSeconds;
            this.LogInNoLockingBackgroundMode = logInNoLockingBackgroundMode;
            //finishLoggingEvent += StopBackgroundLogging;
        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            // check isBackgroundModeEnabled es bool

            bool isBackgroundModeEnabled = this.LogInNoLockingBackgroundMode == "false" ? false : true;

            if (isBackgroundModeEnabled)
            {
                finishLoggingEvent += StopBackgroundLogging;
                var newThread = new Thread(() => StartLoggingFlight(sender, connection));
                newThread.Start();
                return new ActionResult("Logger started", "Logger started", false, finishLoggingEvent);
            }
            else
            {
                StartLoggingFlight(sender, connection);
            }
            
            return new ActionResult(KmlContent, KmlContent, false);
        }

        private void StopBackgroundLogging(object sender, bool isManualStop)
        {
            StopLogging(this, isManualStop);
            finishLoggingEvent -= StopBackgroundLogging;
        }

        private void StartLoggingFlight(object sender, SimConnect connection)
        {
            Logger logger = new Logger();


            // check loggingtimeseconds es int
            // check LoggingPeriodSeconds es int


            int loggingTime = Convert.ToInt32(this.LoggingTimeSeconds);
            int loggingPeriod = 1000 * Convert.ToInt32(this.LoggingPeriodSeconds);

            bool isLoggingTimeEnabled = !String.IsNullOrEmpty(this.LoggingTimeSeconds) && loggingTime > 0;

            if (isLoggingTimeEnabled)
            {
                loggingTimeTimer = new System.Timers.Timer(loggingTime * 1000);
                loggingTimeTimer.Elapsed += delegate { StopLoggingTimeIsOver(); };
                loggingTimeTimer.Start();
            }

            string latitude, longitude, altitude;

            while (continueLogging)
            {
                latitude = new GetVariable("PLANE LATITUDE").ExecuteAction(sender, connection).ComputedResult;
                longitude = new GetVariable("PLANE LONGITUDE").ExecuteAction(sender, connection).ComputedResult;
                altitude = new GetVariable("PLANE ALTITUDE").ExecuteAction(sender, connection).ComputedResult;

                var altitudeMetric = (Convert.ToDouble(altitude) * 0.3048).ToString();

                logger.AddPoint(latitude, longitude, altitudeMetric);

                Thread.Sleep(loggingPeriod);
            }

            // si no hi ha punts loguejats, error i return            
            var xmlPoints = ConvertLogToXML(logger.Points, typeof(List<Point>));
            WriteLogToDisk(xmlPoints, "prova.xml");

            var kmlPoints = ConvertLogToKMLTrace(logger.Points);
            WriteLogToDisk(kmlPoints, "prova.kml");

        }

        private static string ConvertLogToKMLTrace(List<Point> points)
        {
            var filesFolder = ApplicationConfig.GetInstance.FilesFolder;

            var template = File.ReadAllText(Path.Combine(filesFolder, "KMLLogger_Template.kml"));

            template = template.Replace("{trace_color}", Utils.RecalculateColorForKML(ApplicationConfig.GetInstance.KMLLoggerLog.TraceColor));
            template = template.Replace("{logging_project_title}", ApplicationConfig.GetInstance.KMLLoggerLog.TraceTitle);
            template = template.Replace("{lookAt_longitude}", points[0].Longitude);
            template = template.Replace("{lookAt_latitude}", points[0].Latitude);
            template = template.Replace("{lookAt_altitude}", points[0].Altitude);
            template = template.Replace("{lookAt_heading}", "0");
            template = template.Replace("{lookAt_range}", "5000");
            template = template.Replace("{trace_coordinates_set}", ConvertPointsToKMLCompatibleCoordinates(points));

            var indentedKmlFileContent = XDocument.Parse(template).ToString();

            return indentedKmlFileContent;

        }

        private static string ConvertPointsToKMLCompatibleCoordinates(List<Point> points)
        {
            var coordinatesString = "";

            foreach(Point point in points)
            {
                coordinatesString = $"{coordinatesString}{point.Longitude},{point.Latitude},{point.Altitude} ";
            }

            return coordinatesString.ToString();
        }


        private static string ConvertLogToXML(object ObjectToSerialize, Type typeOfPoints)
        {
            XmlSerializer serializedKML = new XmlSerializer(typeOfPoints);

            var loggedXML = "";

            using (var stringWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter))
                {
                    serializedKML.Serialize(writer, ObjectToSerialize);
                    loggedXML = stringWriter.ToString();
                }
            }

            var indentedXmlFileContent = XDocument.Parse(loggedXML).ToString();

            return indentedXmlFileContent;
        }

        private void StopLoggingTimeIsOver()
        {
            StopLogging(this, false);
            loggingTimeTimer.Stop();
        }

        public void StopLogging(object sender, bool isManualStop = false)
        {
            continueLogging = false;
        }

        private void WriteLogToDisk(string content, string filename)
        {
            var logsFolder = ApplicationConfig.GetInstance.LoggerFolder;

            if (!Directory.Exists(logsFolder))
            {
                Directory.CreateDirectory(logsFolder);
            }

            // usar path.combine

            File.WriteAllText(logsFolder+"\\" + filename, content);
        }


    }
}
