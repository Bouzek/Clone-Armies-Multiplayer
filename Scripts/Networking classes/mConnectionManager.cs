using UnityEngine;
using System.Collections;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using GooglePlayGames;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class mConnectionManager : MonoBehaviour, RealTimeMultiplayerListener
{
    public static mConnectionManager instance;
    public mNetworkConnectionHandler connectionHandler;
    private mTimeoutManager timeoutManager;
    

    private bool host = false;

    private string disconnectReason = "Disconnected";

    uint currentLvlId;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void finish()
    {
        mLobbyNavigator.currentInvitation = null;
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        Destroy(this.gameObject);
    }

    public void CreateRoom(uint lvlId)
    {
        mLobbyNavigator.instance.showingWaitingRoom = false;
        PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(1, 1,
                (uint)lvlId, this);
        currentLvlId = lvlId;
    }

    public void AcceptInvitation(Invitation invitation)
    {
        mLobbyNavigator.instance.showingWaitingRoom = false;
        PlayGamesPlatform.Instance.RealTime.AcceptInvitation(invitation.InvitationId, this);
        currentLvlId = (uint)invitation.Variant;
    }

    public void DeclineInvitation(Invitation invitation)
    {
        PlayGamesPlatform.Instance.RealTime.DeclineInvitation(invitation.InvitationId);
    }

    public void OnLeftRoom()
    {
        connectionHandler.onDisconnect(disconnectReason);
    }

    public void leaveRoom()
    {
        PlayGamesPlatform.Instance.RealTime.LeaveRoom();
    }

    public bool isInRoom()
    {
        return PlayGamesPlatform.Instance.RealTime.IsRoomConnected();
    }

    public void OnParticipantLeft(Participant participant)
    {
        leaveRoom();
    }

    public void OnPeersConnected(string[] participantIds)
    {
        host = decideHost();
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        leaveRoom();
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        if (!timeoutManager.processMessage(isReliable, data)) //CHECK IF MESSAGE ISNT JUST REFRESH MESSAGE
        {
            connectionHandler.handleMessage(data, isReliable);
        }
    }
    
    public void SendMessageToAll(bool isreliable, byte[] data)
    {
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(isreliable, data);
        OnRealTimeMessageReceived(isreliable, PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId, data);
    }
    public void SendMessageToOtherPlayer(bool isreliable, byte[] data)
    {
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(isreliable, data);
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            connectionHandler = new mWaitingRoomConnection();
            timeoutManager = new mTimeoutManager(10, 2);
            host = decideHost();
            mLobbyNavigator.instance.waitingRoom.connectedToRoom();
        }
        else
        {
            mLobbyNavigator.instance.showingWaitingRoom = false;
            mLobbyNavigator.instance.waitingRoom.hide();
        }
    }

    public void OnRoomSetupProgress(float percent)
    {
        if (!mLobbyNavigator.instance.showingWaitingRoom)
        {
            mLobbyNavigator.instance.showingWaitingRoom = true;
            mLobbyNavigator.instance.waitingRoom.show((int)currentLvlId);
        }
    }

    internal static void OnInvitationReceived(Invitation invitation, bool shouldAutoAccept)
    {
        if (mConnectionManager.instance != null)
        {
            if (shouldAutoAccept)
            {
                mConnectionManager.instance.AcceptInvitation(invitation);
            }
            else
            {
                mLobbyNavigator.instance.showInvitation(invitation);
            }
        }
        else
        {
            if (shouldAutoAccept)
            {
                mLobbyNavigator.currentInvitation = invitation;
                SceneManager.LoadScene(5);
            }
        }
    }

    public bool isHost()
    {
        return host;
    }

    private bool decideHost()
    {
        string myId = PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
        Participant player = getOtherPlayer();
        if (player == null) return true;
        return String.Compare(player.ParticipantId, myId, false) > 0;
    }

    public string getOtherPlayerName()
    {
        Participant player = getOtherPlayer();
        if (player == null)
        {
            return "PLAYER";
        }
        return player.DisplayName;
    }

    private Participant getOtherPlayer()
    {
        string myId = PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
        foreach (Participant participant in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
        {
            if (String.Compare(participant.ParticipantId, myId, false) != 0)
            {
                return participant;
            }
        }
        return null;
    }
}
