using System.Collections.Generic;

public abstract class mNetworkConnectionHandler {

    List<byte> statusUpdateMessage = new List<byte>(5); //char + int


    public abstract void handleMessage(byte[] data, bool reliable);

    public abstract void onDisconnect(string reason);



    public void SendStatusMessage(char status, int value)
    {
        statusUpdateMessage.Clear();
        statusUpdateMessage.Add((byte)status);
        statusUpdateMessage.AddRange(System.BitConverter.GetBytes(value));
        mConnectionManager.instance.SendMessageToAll(true, statusUpdateMessage.ToArray());
    }

}
