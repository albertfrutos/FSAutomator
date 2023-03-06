using Newtonsoft.Json;

namespace FSAutomator.BackEnd.Configuration
{
    public sealed class ApplicationConfig
    {
        private string s_AutomationsFolder { get; set; }
        private string s_ExportFolder { get; set; }
        private string s_TempFolder { get; set; }

        private static ApplicationConfig instance = null;

        public static ApplicationConfig GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = Initialize();
                }
                return instance;
            }
        }

        private static ApplicationConfig Initialize()
        {
            var json = File.ReadAllText(Path.Combine("Configuration", "ApplicationConfiguration", "ApplicationConfiguration.json"));
            var applicationConfig = JsonConvert.DeserializeObject<ApplicationConfig>(json);
            return applicationConfig;
        }

        public string AutomationsFolder
        {
            get { return this.s_AutomationsFolder; }
            set { this.s_AutomationsFolder = value; }
        }

        public string ExportFolder
        {
            get { return this.s_ExportFolder; }
            set { this.s_ExportFolder = value; }
        }

        public string TempFolder
        {
            get { return this.s_TempFolder; }
            set { this.s_TempFolder = value; }
        }
    }
}
