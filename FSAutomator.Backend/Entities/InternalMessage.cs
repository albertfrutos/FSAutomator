namespace FSAutomator.Backend.Entities
{
    public class InternalMessage
    {
        public string Message = "";
        public MsgType Type;

        public InternalMessage()
        {

        }
        public InternalMessage(string message, bool isError, bool isCriticalError = false)
        {
            this.Message = message;

            MsgType errorType;

            switch (isError, isCriticalError)
            {
                case (true, true):
                    errorType = MsgType.Critical;
                    break;
                case (true, false):
                    errorType = MsgType.Error;
                    break;
                default:
                    errorType = MsgType.Info;
                    break;
            }

            this.Type = errorType;
        }

        public InternalMessage(string message)
        {
            this.Message = message;
            this.Type = MsgType.Info;
        }

        public enum MsgType
        {
            Info,
            Error,
            Critical
        }
    }
}
