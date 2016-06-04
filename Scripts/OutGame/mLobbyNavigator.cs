using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System;

public class mLobbyNavigator : MonoBehaviour
{
    public static mLobbyNavigator instance;
    public static Invitation currentInvitation = null;

    public Animator LobbyAnimator;
    public GameObject invitationPanel;
    public mWaitingRoomManager waitingRoom;
    public Text invitationText;

    private bool inCoop = false;

    public bool showingWaitingRoom = false;

    public Hider hider;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(currentInvitation != null)
        {
            mConnectionManager.instance.AcceptInvitation(currentInvitation);
            currentInvitation = null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    public void GoToCoop()
    {
        if (!inCoop)
        {
            LobbyAnimator.SetTrigger("toCoop");
            inCoop = true;
        }
    }

    public void GoBack()
    {
        if (inCoop)
        {
            LobbyAnimator.SetTrigger("toLobby");
            inCoop = false;
        }
        else
        {
            mConnectionManager.instance.finish();
            hider.FadeIn(0);
        }
    }

    public void OpenCoopMission(int lvlId)
    {
        mConnectionManager.instance.CreateRoom((uint)lvlId);
    }
    
    public void StartMission(int lvlId)
    {
        hider.FadeIn(lvlId);
    }

    public void showInvitation(Invitation invitation)
    {
        currentInvitation = invitation;
        invitationPanel.SetActive(true);
        invitationText.text = "You have been invited to play cooperation mode (mission "+invitation.Variant+") by " + invitation.Inviter.DisplayName;
    }
   
    public void acceptInv()
    {
        mConnectionManager.instance.AcceptInvitation(currentInvitation);
    }
    public void declineInv()
    {
        mConnectionManager.instance.DeclineInvitation(currentInvitation);
    }
}
