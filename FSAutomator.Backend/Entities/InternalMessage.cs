namespace FSAutomator.Backend.Entities
{
    public class InternalMessage
    {
        public string Message = "";
        public string Type = "";
        public bool IsError = false;
        public bool IsCriticalError = false;

        public InternalMessage()
        {

        }
        public InternalMessage(string message, string type, bool isError = false, bool isCriticalError = false)
        {
            this.Message = message;
            this.Type = type;
            this.IsError = isError;
            this.IsCriticalError = isCriticalError;
        }
    }
}
