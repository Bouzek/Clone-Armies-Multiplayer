using UnityEngine;
using System.Collections;

public class mTimeoutManager{
    float lastRefreshReceived;
    float timeout;
    float interval;


    public mTimeoutManager (float timeout, float checkInterval)
    {
        this.interval = checkInterval;
        this.timeout = timeout;
        Invoker.InvokeDelayed(refresh, checkInterval);
        refreshed();
    }

    private void refresh()
    {
        byte[] refreshData = {(byte)'R'};
        mConnectionManager.instance.SendMessageToOtherPlayer(true, refreshData); // REFRESH MESSAGE

        if (Time.unscaledTime - lastRefreshReceived > timeout)
        {
            if (mConnectionManager.instance.isInRoom())
            {
                mConnectionManager.instance.leaveRoom();
            }
            else
            {
                mConnectionManager.instance.OnLeftRoom();
            }
            return;
        }
        Invoker.InvokeDelayed(refresh, interval);
    }

    private void refreshed()
    {
        lastRefreshReceived = Time.unscaledTime;
    }

    //RETURN TRUE IF THIS WAS REFRESH MESSAGE
    public bool processMessage(bool isReliable, byte[] data)
    {
        if(isReliable && data.Length == 1 && ((char)data[0]) == 'R')
        {
            refreshed();
            return true;
        }
        return false;
    }
}
