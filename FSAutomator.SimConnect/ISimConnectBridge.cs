using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.SimConnectInterface
{
    public interface ISimConnectBridge
    {
        public SimConnect Connection { get; }

        public event EventHandler<ConnectionStatusChangeEventArgs> ConnectionStatusChangeEvent;
        public void Connect();
        public void Disconnect();
        public bool IsConnected();
        void AddToDataDefinition(Enum defineID, string variableName, string unit, SIMCONNECT_DATATYPE dataType, float v, uint sIMCONNECT_UNUSED);
        void RegisterDataDefineStruct<T>(Enum stringType);
        void RequestDataOnSimObjectType(Enum requestID, Enum defineID, uint v, SIMCONNECT_SIMOBJECT_TYPE uSER);
        void ClearDataDefinition(Enum defineID);
        void MapClientEventToSimEvent(Enum eventToSend, string eventName);
        void AddClientEventToNotificationGroup(Enum groupID, Enum eventToSend, bool v);
        void SetNotificationGroupPriority(Enum groupID, uint sIMCONNECT_GROUP_PRIORITY_HIGHEST);
        void TransmitClientEvent(uint v1, Enum eventToSend, uint v2, Enum gROUP0, SIMCONNECT_EVENT_FLAG gROUPID_IS_PRIORITY);
        void ClearNotificationGroup(Enum groupID);
        void SubscribeToOnRecvSimobjectDataBytypeEventHandler(Action<SimConnect, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE> simconnect_OnRecvSimobjectDataBytype);
    }
}
