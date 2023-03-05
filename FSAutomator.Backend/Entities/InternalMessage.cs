


namespace FSAutomator.BackEnd.Entities
{
    public class InternalMessage
    {
        public string Message = "";
        public string Type = "";
        public bool IsError = false;

        public InternalMessage()
        {

        }
        public InternalMessage(string message, string type, bool isError = false)
        {
            this.Message = message;
            this.Type = type;
            this.IsError = isError;
        }
    }
}
