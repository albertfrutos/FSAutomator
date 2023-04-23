using Newtonsoft.Json;

namespace FSAutomator.BackEnd.Configuration
{
    public sealed class ApplicationConfig
    {
        private string s_AutomationsFolder { get; set; }
        private string s_ExportFolder { get; set; }
        private string s_TempFolder { get; set; }
        private string s_LoggerFolder { get; set; }
        private string s_FilesFolder { get; set; }
        private string s_SchemaFile { get; set; }
        private FSPackagesPathsConfig o_FSPackagesPaths { get; set; }
        private KMLLoggerLogConfig o_KMLLoggerLog { get; set; }



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
            var json = File.ReadAllText(Path.Combine("Configuration", "ApplicationConfiguration.json"));
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

        public string LoggerFolder
        {
            get { return this.s_LoggerFolder; }
            set { this.s_LoggerFolder = value; }
        }

        public string FilesFolder
        {
            get { return this.s_FilesFolder; }
            set { this.s_FilesFolder = value; }
        }

        public string SchemaFile
        {
            get { return this.s_SchemaFile; }
            set { this.s_SchemaFile = value; }
        }

        public FSPackagesPathsConfig FSPackagesPaths
        {
            get { return this.o_FSPackagesPaths; }
            set { this.o_FSPackagesPaths = value; }
        }

        public KMLLoggerLogConfig KMLLoggerLog
        {
            get { return this.o_KMLLoggerLog; }
            set { this.o_KMLLoggerLog = value; }
        }
    }
}
