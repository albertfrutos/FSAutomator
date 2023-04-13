using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.SimConnectInterface
{
    public class ConnectionStatusChangeEventArgs : EventArgs
    {
        public Enum ConnectionStatus { get; }
        public string Message { get; }
        public ConnectionStatusChangeEventArgs(Enum status, string msg)
        {
            this.ConnectionStatus = status;
            this.Message = msg;
        }
    }
}
