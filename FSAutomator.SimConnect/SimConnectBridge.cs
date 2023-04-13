using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static FSAutomator.SimConnectInterface.Entities;

namespace FSAutomator.SimConnectInterface
{
    public sealed class SimConnectBridge : ISimConnectBridge
    {
        public SimConnect Connection { get; private set; } = null;

        public event EventHandler<ConnectionStatusChangeEventArgs> ConnectionStatusChangeEvent;

        private EventWaitHandle simConnectEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        
        private Thread simConnectReceiveThread = null;

        private static SimConnectBridge simConnectBridgeInstance = null;

        public static SimConnectBridge Instance
        {
            get
            {
                if (simConnectBridgeInstance == null)
                {
                    simConnectBridgeInstance = new SimConnectBridge();
                }
                return simConnectBridgeInstance;
            }
        }

        public void Connect(bool reconnectIfConnected)
        {
            if ((this.Connection != null && reconnectIfConnected) || this.Connection == null)
            {
                Connect();
            }
        }

        public void AddClientEventToNotificationGroup(Enum groupID, Enum eventID, bool maskable)
        {
            Connection.AddClientEventToNotificationGroup(groupID, eventID, maskable);

        }

        public void SetNotificationGroupPriority(Enum groupID, uint priority)
        {
            Connection.SetNotificationGroupPriority(groupID, priority);
        }

        public void MapClientEventToSimEvent(Enum eventToSend, string eventName)
        {
            Connection.MapClientEventToSimEvent((Enum) eventToSend, eventName);
        }

        public void TransmitClientEvent(uint objectID, Enum eventID, uint dwData, Enum groupID, SIMCONNECT_EVENT_FLAG flags)
        {
            Connection.TransmitClientEvent(objectID, eventID, dwData, groupID, flags);

        }

        public void ClearNotificationGroup(Enum group)
        {
            Connection.ClearNotificationGroup(group);
        }

        public void Connect()
        {
            try
            {
                this.Connection = new SimConnect("SimConnectBridge", IntPtr.Zero, 0, this.simConnectEventHandle, 0);

                this.Connection.OnRecvOpen += new SimConnect.RecvOpenEventHandler(this.Simconnect_OnRecvOpen);
                this.Connection.OnRecvQuit += new SimConnect.RecvQuitEventHandler(this.Simconnect_OnRecvQuit);
                this.Connection.OnRecvException += new SimConnect.RecvExceptionEventHandler(this.Simconnect_OnRecvException);

                this.StartMessageReceiveThreadHandler();
            }
            catch (COMException ex)
            {
                this.AnnounceConnectionStatusChange(ConnectionStatus.Failed, ex.Message);
            }
        }

        public void RegisterDataDefineStruct<T>(Enum type)
        {
            this.Connection.RegisterDataDefineStruct<T>(type);
        }

        public void AddToDataDefinition(Enum defineID, string variableName, string unit, SIMCONNECT_DATATYPE dataType, float v, uint sIMCONNECT_UNUSED)
        {
            this.Connection.AddToDataDefinition(defineID, variableName, unit, dataType, 0.0f, sIMCONNECT_UNUSED);
        }

        public void Disconnect()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
                this.Connection = null;
                //status.IsConnectedToSim = false;
            }
        }

        public bool IsConnected()
        {
            return this.Connection != null;
        }



        public void RequestDataOnSimObjectType(Enum requestID, Enum defineID, uint v, SIMCONNECT_SIMOBJECT_TYPE simObjectType)
        {
            Connection.RequestDataOnSimObjectType(requestID, defineID, v, simObjectType);
        }

        public void ClearDataDefinition(Enum defineID)
        {
            Connection.ClearDataDefinition(defineID);
        }

        public void SubscribeToRecvSimobjectDataBytypeEventHandler(Action<SimConnect, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE> method)
        {
            this.Connection.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(method);

        }

        private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            this.AnnounceConnectionStatusChange(ConnectionStatus.Open, null);

            /*
            this.automator.connection = this.m_SimConnect;
            status.IsConnectedToSim = true;
            status.GeneralErrorHasOcurred = false;
            */
        }

        private void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            var exceptionMessage = data.dwException.ToString();

            this.AnnounceConnectionStatusChange(ConnectionStatus.Exception, exceptionMessage);

            /*
            Console.WriteLine("An exception occurred Simconnect_OnRecvException: {0}", data.dwException.ToString());
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            Disconnect();

            // This causes a trigger of a general (critical error). This means that the automation will be stopped.
            status.ReportStatus(new InternalMessage("An exception ocurred with the connection to the sim and the automation will be stopped: " + eException.ToString(), true, true));
            */
        }

        private void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            this.AnnounceConnectionStatusChange(ConnectionStatus.Quit, null);
            Disconnect();

            /*
            status.IsConnectedToSim = false;
            Disconnect();
            Console.WriteLine("Simulator has exited. Closing connection and exiting Simulator module");
            */
        }

        private void AnnounceConnectionStatusChange(Enum connectionStatus, string message)
        {
            this.ConnectionStatusChangeEvent(this, new ConnectionStatusChangeEventArgs(connectionStatus, message));
        }


        private void StartMessageReceiveThreadHandler()
        {
            this.simConnectReceiveThread = new Thread(new ThreadStart(SimConnect_MessageReceiveThreadHandler));
            this.simConnectReceiveThread.IsBackground = true;
            this.simConnectReceiveThread.Start();

        }
        private void SimConnect_MessageReceiveThreadHandler()
        {
            while (true)
            {
                this.simConnectEventHandle.WaitOne();

                try
                {
                    this.Connection?.ReceiveMessage();
                }
                catch
                {
                    // ignored
                }
            }
        }


    }
}
