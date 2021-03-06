﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Managers/Scene/Network Lobby")]
public class NetworkLobbyManager : Singleton<NetworkLobbyManager>
{
    #region Constants

    public const string PROP_IS_READY = "isReady";
	public const string PROP_PLAYER_ID = "playerID";

    private const string MSG_CONNECTING = "Connecting...";
    private const string MAIN_MENU_ID = "main_menu";
    private const string CHARACTER_SELECT_ID = "character_select";
    private const string CR_COUNTDOWN = "CR_Countdown";

    #endregion

    #region Fields

    public Canvas MainLobbyCanvas;
    public Canvas GameLobbyCanvas;
	public CanvasGroup HeaderCanvasGroup;
    public CanvasGroup LobbyGroup;
    public CanvasGroup PreconnectGroup;
    public CanvasGroup StatusGroup;
	public CanvasGroup PrivateRoomCreateGroup;
	public CanvasGroup PrivateRoomJoinGroup;
    public CanvasGroup RoomListGroup;
    public GameObject RoomListLayoutGroup;
    public GameObject RoomPanelPrefab;
	public Button P1ReadyButton;
	public Button P2ReadyButton;
	public Text P1ReadyButtonText;
	public Text P2ReadyButtonText;
    public UpdateUsernameText P1Username;
    public UpdateUsernameText P2Username;
    public Text P1Name;
    public Text P2Name;
    public Image P1ReadyStatus;
    public Image P2ReadyStatus;
    public Text CountdownField;
	public GameObject ScrollingBackground; 
	public Text LocalUserName;
    public Text Wins;
    public Text Losses;
    public Text ChatLog;
    public InputField ChatInput;
    public Button ChatButton;
    public Button JoinPublic;
	public InputField PrivateRoomCreateInput;
	public InputField PrivateRoomJoinInput;

    private PhotonView cPhotonView;
    private string username;
    private string otherUsername;
    private bool[] playersReady = new bool[Globals.MAX_CONNECTED_PLAYERS];
    private List<GameObject> roomPanelCloneList = new List<GameObject>();
    private InputField usernameField;
    private Button connectBtn;

	private InputField privateRoomNameCreateField;
	private Button privateCreateButton;
	private InputField privateRoomNameJoinField;
	private Button privateJoinButton;

    private List<string> chatLog = new List<string>();

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        cPhotonView = GetComponent<PhotonView>();
        usernameField = PreconnectGroup.GetComponentInChildren<InputField>();
        connectBtn = PreconnectGroup.GetComponentInChildren<Button>();

		privateRoomNameCreateField = PrivateRoomCreateGroup.GetComponentInChildren<InputField>();
		privateCreateButton = PrivateRoomCreateGroup.GetComponentInChildren<Button>();

		privateRoomNameJoinField = PrivateRoomJoinGroup.GetComponentInChildren<InputField>();
		privateJoinButton = PrivateRoomJoinGroup.GetComponentInChildren<Button>();

        ScrollingBackground.SetActive(true);

        PhotonNetwork.offlineMode = false;

        if (!PhotonNetwork.connectedAndReady)
            ToggleCanvasGroup(PreconnectGroup, true);
        else
        {
            ToggleCanvasGroup(LobbyGroup, true);
			ToggleCanvasGroup(HeaderCanvasGroup, true);
            ToggleCanvasGroup(RoomListGroup, true);
        }

        Wins.text = "WINS: " + PlayerPrefs.GetInt("totalWins", 0).ToString();
        Losses.text = "LOSSES: " + PlayerPrefs.GetInt("totalLosses", 0).ToString();
    }

    #endregion

    #region Network Callbacks

    private void OnReceivedRoomListUpdate()
    {
        RefreshRoomList();
    }

    private void OnJoinedLobby()
    {
        ToggleCanvasGroup(StatusGroup, false);
        ToggleCanvasGroup(LobbyGroup, true);
		ToggleCanvasGroup(HeaderCanvasGroup, true);
        ToggleCanvasGroup(RoomListGroup, true);
    }

    private void OnJoinedRoom()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add(PROP_IS_READY, false);
		if(PhotonNetwork.isMasterClient){
			properties.Add(PROP_PLAYER_ID, 1);
			Globals.PlayerID = 1;

		}else{
			properties.Add(PROP_PLAYER_ID, 2);
			Globals.PlayerID = 2;
            otherUsername = PhotonNetwork.masterClient.name;
		}
		
        PhotonNetwork.player.SetCustomProperties(properties);

        chatLog.Clear();

        MainLobbyCanvas.enabled = false;
        GameLobbyCanvas.enabled = true;
		CountdownField.text = "";
		ChatLog.text = "";
		UpdateReadyButtons();
        UpdateUsernames();
    }

    private void OnPhotonJoinRoomFailed()
    {
        ToggleCanvasGroup(LobbyGroup, true);
    }

    private void OnPhotonPlayerConnected(PhotonPlayer _player)
    {
        otherUsername = _player.name;
        UpdateReadyButtons();
        P2ReadyStatus.enabled = false;
        CountdownField.text = "";
        UpdateUsernames();
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer _player)
    {
        playersReady[1] = false;
        UpdateReadyButtons();
        P2ReadyStatus.enabled = false;
        CountdownField.text = "";
        StopCoroutine(CR_COUNTDOWN);
        otherUsername = "";
        UpdateUsernames();
    }

    private void OnLeftRoom()
    {
        GameLobbyCanvas.enabled = false;
        MainLobbyCanvas.enabled = true;
		P1ReadyStatus.enabled = false;
		P2ReadyStatus.enabled = false;
        otherUsername = "";
    }

    private void OnMasterClientSwitched(PhotonPlayer _player)
    {
        PhotonNetwork.LeaveRoom();
        playersReady[0] = false;
        playersReady[1] = false;
		P1ReadyStatus.enabled = false;
		P2ReadyStatus.enabled = false;
		
    }

    private void OnDisconnectedFromPhoton()
    {
        Application.LoadLevel(MAIN_MENU_ID);
    }

    #endregion

    #region Methods

    public void JoinRoom(string _roomID)
    {
        PhotonNetwork.JoinRoom(_roomID);
    }
	
	private void DisconnectFromNetwork()
    {
        PhotonNetwork.Disconnect();
    }

    private void RefreshRoomList()
    {
        foreach (GameObject obj in roomPanelCloneList)
        {
            Destroy(obj);
        }

        RoomInfo[] roomInfoList = PhotonNetwork.GetRoomList();

        for (int i = 0; i < roomInfoList.Length; i++)
        {
            RoomInfo roomInfo = roomInfoList[i];

            if (roomInfo.playerCount != roomInfo.maxPlayers)
            {
                GameObject roomPanel = Instantiate(RoomPanelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                roomPanelCloneList.Add(roomPanel);
                roomPanel.transform.SetParent(RoomListLayoutGroup.transform, false);
                roomPanel.GetComponentInChildren<Text>().text = roomInfo.name;
                roomPanel.GetComponentInChildren<Button>().gameObject.GetSafeComponent<JoinRoomButton>().RoomID = roomInfo.name;
            }
        }
		if(roomInfoList.Length > 0){
			// Trying to select a default room immediately doesn't work, as it hasn't integrated with the event system yet.
			// Wait a split second.
			StartCoroutine("DelayedButtonSelect");
		}
    }

	IEnumerator DelayedButtonSelect(){
		yield return new WaitForSeconds(0.1f);
		GameObject go = RoomListGroup.transform.Find ("Room List Layout").GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(go);;
	}

	private void UpdateReadyButtons(){
		switch(Globals.PlayerID){
			case 1:
				P1ReadyButton.gameObject.SetActive(true);
				P2ReadyButton.gameObject.SetActive(false);
				
				break;
			case 2:
				P1ReadyButton.gameObject.SetActive(false);
				P2ReadyButton.gameObject.SetActive(true);
				
				break;
		}
		P1ReadyButtonText.text = (playersReady[0] ?  "CANCEL" : "READY");
		P2ReadyButtonText.text = (playersReady[1] ?  "CANCEL" : "READY");
	}

    private void UpdateUsernames()
    {
        P1Name.text = "";
        P2Name.text = "";

        if (PhotonNetwork.isMasterClient)
        {
            P1Name.text = PhotonNetwork.player.name;
            P2Name.text = otherUsername;
        }
        else
        {
            P2Name.text = PhotonNetwork.player.name;
            P1Name.text = otherUsername;
        }
    }

    //private IEnumerator UpdateUsernames()
    //{
    //    P1Username.ClearText();
    //    P2Username.ClearText();
    //    Debug.Log ("Attempting to assign names");
    //    yield return new WaitForSeconds(0.2f);
    //    if(PhotonNetwork.playerList.Length == 1){
    //        P1Username.UpdateText(1);
    //        P2Username.ClearText();
    //    }else{
    //        for (int i = 0; i <  PhotonNetwork.playerList.Length; i++)
    //        {
    //            int id = (int)PhotonNetwork.playerList[i].customProperties[PROP_PLAYER_ID];
    //            if (PhotonNetwork.playerList[i].customProperties[PROP_PLAYER_ID] == null) P2Username.UpdateText(id);
    //            else{
    //                if (i == 0)  // if this is Player 1
    //                {
    //                    P1Username.UpdateText(id);
    //                }
    //                else 
    //                {
    //                    P2Username.UpdateText(id);
    //                }
    //            }
				
    //        }
    //    }

        
       
    //}

    private void ToggleCanvasGroup(CanvasGroup _group, bool _enabled)
    {
        switch (_enabled)
        {
            case true:
                _group.alpha = 1;
                _group.interactable = true;
                _group.blocksRaycasts = true;
                break;

            case false:
                _group.alpha = 0;
                _group.interactable = false;
                _group.blocksRaycasts = false;
                break;
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator CR_Countdown()
    {
        for (int i = 3; i > 0; i--)
        {
            if (i != 1)
                CountdownField.text = "GAME STARTING IN " + i.ToString() + " SECONDS...";
            else
				CountdownField.text = "GAME STARTING IN 1 SECOND...";

            yield return new WaitForSeconds(1.0f);
        }

        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.LoadLevel(CHARACTER_SELECT_ID);
    }

    #endregion

    #region UI Callbacks

    public void OnChange_Username()
    {
        if (usernameField.text != "")
            connectBtn.interactable = true;
        else
            connectBtn.interactable = false;
    }

	public void OnChange_PrivateGameNameCreate()
	{
		if (privateRoomNameCreateField.text != "")
			privateCreateButton.interactable = true;
		else
			privateCreateButton.interactable = false;
	}

	public void OnChange_PrivateGameNameJoin()
	{
		if (privateRoomNameJoinField.text != "")
			privateJoinButton.interactable = true;
		else
			privateJoinButton.interactable = false;
	}

    public void OnClick_Connect()
    {
        username = usernameField.text;
		LocalUserName.text = username;
        ToggleCanvasGroup(PreconnectGroup, false);

        Globals.Username = username;
        NetworkManager.Instance.ConnectToNetwork();

        StatusGroup.GetComponentInChildren<Text>().text = MSG_CONNECTING;
        ToggleCanvasGroup(StatusGroup, true);
    }

    public void OnClick_CreateGame()
    {
        NetworkManager.Instance.CreateRoom();
    }

    public void OnClick_CreatePrivateGame()
    {
		ToggleCanvasGroup(LobbyGroup, false);
		ToggleCanvasGroup(PrivateRoomCreateGroup, true);
		PrivateRoomCreateInput.text = "";
		PrivateRoomCreateInput.Select();
		PrivateRoomCreateInput.ActivateInputField();
    }

    public void OnClick_JoinPrivateGame()
    {
		ToggleCanvasGroup(LobbyGroup, false);
		ToggleCanvasGroup(PrivateRoomJoinGroup, true);
		PrivateRoomJoinInput.text = "";
		PrivateRoomJoinInput.Select();
		PrivateRoomJoinInput.ActivateInputField();
    }

	public void OnClick_PrivateGameNameCreateInput(){
		string roomname = privateRoomNameCreateField.text;
		ToggleCanvasGroup(PrivateRoomCreateGroup, false);
		NetworkManager.Instance.CreateRoom(roomname, false);
	}

	public void OnClick_PrivateGameNameJoinInput(){
		string roomname = privateRoomNameJoinField.text;
		ToggleCanvasGroup(PrivateRoomJoinGroup, false);
		NetworkLobbyManager.Instance.JoinRoom(roomname);
	}

	public void OnClick_BackToLobby(){
		PhotonNetwork.LeaveRoom();
		playersReady[0] = false;
		playersReady[1] = false;
	}

    public void OnClick_BackToLobbyFromPrivateRoomMenu()
    {
        ToggleCanvasGroup(PrivateRoomCreateGroup, false);
        ToggleCanvasGroup(PrivateRoomJoinGroup, false);
        ToggleCanvasGroup(LobbyGroup, true);
    }

    public void OnClick_BackToMainMenu()
    {
        DisconnectFromNetwork();
    }

    public void OnClick_Ready()
    {
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
		if(PhotonNetwork.player.ID == 1) newProperties.Add(PROP_IS_READY,!playersReady[0]);
		else newProperties.Add(PROP_IS_READY,!playersReady[1]);
        PhotonNetwork.player.SetCustomProperties(newProperties);

		cPhotonView.RPC("RPC_ClickReady", PhotonTargets.AllBufferedViaServer);
	}

    public void OnClick_SendChat()
    {
		if(ChatInput.text == "") return;
        string msg = username + ": " + ChatInput.text;
        cPhotonView.RPC("RPC_SendChat", PhotonTargets.All, msg);
        ChatInput.text = "";
        ChatInput.Select();
        ChatInput.ActivateInputField();
    }

    #endregion

    #region RPC

    [RPC]
    private void RPC_ClickReady()
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
			int id = (int)player.customProperties[PROP_PLAYER_ID];
			playersReady[id - 1] = (bool)player.customProperties[PROP_IS_READY];

            if (id == 1)  // if this is Player 1
            {
				P1ReadyStatus.enabled = playersReady[player.ID-1];
				P1ReadyButtonText.text = (playersReady[player.ID-1] ?  "CANCEL" : "READY");
            }
            else if (id == 2)  // if this is Player 2
            {
				P2ReadyStatus.enabled = playersReady[1];
				P2ReadyButtonText.text = (playersReady[1] ? "CANCEL" : "READY");
            }
        }

        if (playersReady[0] && playersReady[1])
            StartCoroutine(CR_COUNTDOWN);
        else
        {
            StopCoroutine(CR_COUNTDOWN);
            CountdownField.text = "";
        }
    }

    [RPC]
    private void RPC_SendChat(string _msg)
    {
        chatLog.Add(_msg);

        string log = "";

        foreach (string str in chatLog)
        {
            log += str + "\n";
        }

        ChatLog.text = log;
    }

    #endregion
}