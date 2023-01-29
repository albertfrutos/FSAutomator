namespace FSAutomator.Backend.Entities
{
    public class AutomationFile
    {
        public string FileName { get; set; }
        public string PackageName { get; set; } = null;
        public string VisibleName { get; set; }

        public AutomationFile(string name, string packageName = "", string visibleName = "") { FileName = name; PackageName = packageName; VisibleName = visibleName != "" ? visibleName : FileName; }
    }
}
