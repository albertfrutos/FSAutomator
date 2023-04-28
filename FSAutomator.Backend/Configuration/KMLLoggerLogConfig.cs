namespace FSAutomator.Backend.Configuration
{
    public class KMLLoggerLogConfig
    {
        private string s_TraceColor { get; set; }
        private string s_TraceTitle { get; set; }

        public string TraceTitle
        {
            get { return this.s_TraceTitle; }
            set { this.s_TraceTitle = value; }
        }

        public string TraceColor
        {
            get { return this.s_TraceColor; }
            set { this.s_TraceColor = value; }
        }
    }
}
