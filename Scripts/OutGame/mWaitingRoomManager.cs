using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class mWaitingRoomManager : MonoBehaviour
{

    public Text readyButtonText;
    public Text playerStatusText;
    public Text playerReadyText;

    public Text peerStatusText;
    public Text peerReadyText;
    public Text peerNameText;

    public GameObject botPanel;
    public GameObject botPanelStarting;

    private bool isReady = false;
    private bool isPeerReady = false;

    private bool roomSetup = false;

    private int lvlId;


    public void show(int lvl)
    {
        roomSetup = false;
        GetComponent<Animator>().SetBool("in", true);
        lvlId = lvl;

        playerStatusText.text = "WAITING FOR OTHER PLAYER";
        peerStatusText.text = "NOT CONNECTED";
        peerNameText.text = "PLAYER";
        peerReadyText.text = "INVITED";
    }

    public void hide()
    {
        GetComponent<Animator>().SetBool("in", false);
    }


    public void leave()
    {
        roomSetup = false;
        mConnectionManager.instance.leaveRoom();
    }

    public void connectedToRoom()
    {
        roomSetup = true;
        playerStatusText.text = "CONNECTED";
        readyUpdate(false);
        setPeerName();
    }

    private void setPeerName()
    {
        peerNameText.text = mConnectionManager.instance.getOtherPlayerName();
        peerStatusText.text = "CONNECTED";
    }

    public void ready()
    {
        if (roomSetup)
        {
            readyUpdate(!isReady);
        }
        else
        {
            playerReadyChange(!isReady);
        }
    }

    public void readyUpdate(bool ready)
    {
        char whoIsReady;
        int value = 0; //NOT READY
        if (mConnectionManager.instance.isHost())
        {
            whoIsReady = 'H'; //HOST
        }
        else
        {
            whoIsReady = 'P'; //PEER
        }
        if (ready)
        {
            value = 1;
        }

        mConnectionManager.instance.connectionHandler.SendStatusMessage(whoIsReady, value);
    }

    public void startGame()
    {
        botPanel.SetActive(false);
        botPanelStarting.SetActive(true);
        Invoker.InvokeDelayed(launchGame, 5);
    }

    void launchGame()
    {
        mLobbyNavigator.instance.StartMission(lvlId);
    }

    public void playerReadyChange(bool ready)
    {
        isReady = ready;
        if (isReady)
        {
            playerReadyText.text = "Ready";
            readyButtonText.text = "Not ready";
        }
        else
        {
            playerReadyText.text = "Not ready";
            readyButtonText.text = "Set ready";
        }
    }

    public void peerReadyChange(bool ready)
    {
        isPeerReady = ready;
        if (isPeerReady)
        {
            peerReadyText.text = "Ready";
        }
        else
        {
            peerReadyText.text = "Not ready";
        }
    }
}
