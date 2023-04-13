using FSAutomator.Backend.Actions.Base;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using FSAutomator.BackEnd.Entities;
using FSAutomator.SimConnectInterface;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FSAutomator.Backend.Actions
{
    public class FlightPositionLogger: ActionBase, IAction
    {
        public int LoggingTimeSeconds { get; set; } = 60;
        public int LoggingPeriodSeconds { get; set; } = 1;
        public bool LogInNoLockingBackgroundMode { get; set; } = false;
 
        private System.Timers.Timer loggingTimeTimer;

        internal bool continueLogging = true;

        private EventHandler<bool> finishLoggingEvent;


        public FlightPositionLogger()
        {

        }

        public FlightPositionLogger(int loggingTimeSeconds, int loggingPeriodSeconds, IGetVariable getVariable, bool logInNoLockingBackgroundMode = false) : base(getVariable)
        {
            this.LoggingTimeSeconds = loggingTimeSeconds;
            this.LoggingPeriodSeconds = loggingPeriodSeconds;
            this.LogInNoLockingBackgroundMode = logInNoLockingBackgroundMode;
            this.getVariable = getVariable;
        }

        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
        {
            if (this.LogInNoLockingBackgroundMode)
            {
                finishLoggingEvent += StopBackgroundLogging;
                var newThread = new Thread(() => StartLoggingFlight(sender, connection));
                newThread.Start();
                return new ActionResult("Logger started", "Logger started", false, finishLoggingEvent);
            }
            else
            {
                var result = StartLoggingFlight(sender, connection);
                return result;
            }
        }

        internal void StopBackgroundLogging(object sender, bool isManualStop)
        {
            StopLogging(this, isManualStop);
            finishLoggingEvent -= StopBackgroundLogging;
        }

        private ActionResult StartLoggingFlight(object sender, ISimConnectBridge connection)
        {
            Logger logger = new Logger();

            int loggingTime = Convert.ToInt32(this.LoggingTimeSeconds);
            int loggingPeriod = 1000 * Convert.ToInt32(this.LoggingPeriodSeconds);

            bool isLoggingTimeEnabled = loggingTime > 0;

            if (isLoggingTimeEnabled && !LogInNoLockingBackgroundMode)
            {
                loggingTimeTimer = new System.Timers.Timer(loggingTime * 1000);
                loggingTimeTimer.Elapsed += delegate { StopLoggingTimeIsOver(); };
                loggingTimeTimer.Start();
            }

            string latitude, longitude, altitude;

            while (continueLogging)
            {
                this.getVariable.VariableName = "PLANE LATITUDE";
                latitude = this.getVariable.ExecuteAction(sender, connection).ComputedResult;

                this.getVariable.VariableName = "PLANE LONGITUDE";
                longitude = this.getVariable.ExecuteAction(sender, connection).ComputedResult;

                this.getVariable.VariableName = "PLANE ALTITUDE";
                altitude = this.getVariable.ExecuteAction(sender, connection).ComputedResult;

                var altitudeMetric = (Convert.ToDouble(altitude) * 0.3048).ToString();

                logger.AddPoint(latitude, longitude, altitudeMetric);

                Thread.Sleep(loggingPeriod);
            }

            if(!logger.Points.Any())
            {
                return new ActionResult("No points were logged.", "No points were logged.", true);
            }

            var fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var xmlPoints = ConvertLogToXML(logger.Points, typeof(List<Point>));
            WriteLogToDisk(xmlPoints, $"{fileName}.xml");

            var kmlPoints = ConvertLogToKMLTrace(logger.Points);
            WriteLogToDisk(kmlPoints, $"{fileName}.kml");

            return new ActionResult("Logging finished.", fileName, false);

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

            File.WriteAllText(Path.Combine(logsFolder, filename), content);
        }
    }
}
