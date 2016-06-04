using UnityEngine;
using System.Collections;
using System;

public class mWaitingRoomConnection : mNetworkConnectionHandler
{
    bool hostReady = false;
    bool peerReady = false;

    public override void handleMessage(byte[] data, bool reliable)
    {
        char status = (char)data[0];

        if (status == 'H' || status == 'P') //HOST OR PEER READY
        {
            int value = System.BitConverter.ToInt32(data, 1);
            readyChange(status, value);
        }
        else if (status == 'S') //START GAME
        {
            mLobbyNavigator.instance.waitingRoom.startGame();
        }
    }
    

    public override void onDisconnect(string reason)
    {
        mLobbyNavigator.instance.waitingRoom.hide();
        mLobbyNavigator.instance.showingWaitingRoom = false;
    }
    

    private void readyChange(char status, int value)
    {
        bool ready = false;
        if(value == 1)
        {
            ready = true;
        }
        if((status == 'H' && mConnectionManager.instance.isHost()) || (status == 'P' && !mConnectionManager.instance.isHost()))
        {
            peerReady = ready;
            mLobbyNavigator.instance.waitingRoom.playerReadyChange(ready);
        }
        else
        {
            hostReady = ready;
            mLobbyNavigator.instance.waitingRoom.peerReadyChange(ready);
        }
        checkForStart();
    }

    private void checkForStart()
    {
        if(mConnectionManager.instance.isHost() && hostReady && peerReady)
        {
            SendStatusMessage('S', 0);
        }
    }
}
