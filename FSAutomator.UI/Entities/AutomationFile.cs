namespace FSAutomator.UI.Entities
{
    public class aAutomationFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public aAutomationFile(string name, string path) { FileName = name; FilePath = path; }
    }
}
