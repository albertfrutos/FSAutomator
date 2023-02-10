namespace FSAutomator.Backend.Entities
{
    public class AutomationFile
    {
        public string FileName { get; set; }
        public string PackageName { get; set; } = null;
        public string VisibleName { get; set; }
        public string FilePath { get; set; }

        public AutomationFile(string fileName, string packageName = "", string visibleName = "", string filePath = "")
        {
            FileName = fileName;
            PackageName = packageName;
            VisibleName = visibleName != "" ? visibleName : FileName;
            FilePath = filePath;
        }
    }
}
