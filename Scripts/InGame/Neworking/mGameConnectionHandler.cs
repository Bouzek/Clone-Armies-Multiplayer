using UnityEngine;
using System.Collections;
using System;

public abstract class mGameConnectionHandler : mNetworkConnectionHandler
{
    public override void handleMessage(byte[] data, bool reliable)
    {
        throw new NotImplementedException();
    }

    public override void onDisconnect(string reason)
    {
        throw new NotImplementedException();
    }
}
