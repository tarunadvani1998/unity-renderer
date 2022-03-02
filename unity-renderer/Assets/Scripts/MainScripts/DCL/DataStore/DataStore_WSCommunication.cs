namespace DCL
{
    public class DataStore_WSCommunication
    {
        [System.NonSerialized]
        public string url = "ws://127.0.0.1:5000/";

        public readonly BaseVariable<bool> communicationEstablished = new BaseVariable<bool>();
        public readonly BaseVariable<bool> communicationReady = new BaseVariable<bool>();
    }
}