using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;
using System.Xml;
using System.Xml.Serialization;

namespace FSAutomator.Backend.Actions
{
    public class FlightPositionLogger
    {
        public string LoggingTimeSeconds { get; set; } = null;
        public string LoggingPeriodSeconds { get; set; } = "1";

        private System.Timers.Timer loggingTimeTimer;

        private bool continueLogging = true;

        private EventHandler<bool> StopLoggingEvent;

        private string KmlContent = "";

        public FlightPositionLogger(string loggingTimeSeconds, string loggingPeriodSeconds)
        {
            this.LoggingTimeSeconds = loggingTimeSeconds;
            this.LoggingPeriodSeconds = loggingPeriodSeconds;
        }

        public async Task<ActionResult> ExecuteAction(object sender, SimConnect connection)
        {
            StartLoggingFlight(sender, connection);

            return new ActionResult(KmlContent, KmlContent, false);
        }

        private void StartLoggingFlight(object sender, SimConnect connection)
        {
            Logger logger = new Logger();

            this.StopLoggingEvent += StopLogging;

            //check loggingtimeseconds es int
            //check LoggingPeriodSeconds es int


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

            loggingTimeTimer.Stop();

            var xml = "";

            XmlSerializer serializedKML = new XmlSerializer(typeof(Kml));

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    serializedKML.Serialize(writer, logger.kml);
                    xml = sww.ToString();
                }
            }

            KmlContent = xml;
        }

        public void StopLoggingManually()
        {
            StopLoggingEvent.Invoke(this, true);
        }
        private void StopLoggingTimeIsOver()
        {
            StopLogging(this, false);
        }

        private void StopLogging(object sender, bool isManualStop)
        {
            continueLogging = false;
        }


    }
}
