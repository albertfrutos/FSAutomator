namespace FSAutomator.Backend.Entities
{
    public class AutomationFile
    {
        public string FileName { get; set; }
        public string PackageName { get; set; } = null;
        public bool IsPackage { get; set; }
        public string VisibleName { get; set; }
        public string FilePath { get; set; }
        public string BasePath { get; set; }

        public AutomationFile(string fileName, string packageName = "", string visibleName = "", string filePath = "", string basePath = "", bool isPackage = false)
        {
            FileName = fileName;
            PackageName = packageName;
            this.IsPackage = isPackage;
            VisibleName = visibleName != "" ? visibleName : FileName;
            FilePath = filePath;
            BasePath = basePath;
        }
    }
}
