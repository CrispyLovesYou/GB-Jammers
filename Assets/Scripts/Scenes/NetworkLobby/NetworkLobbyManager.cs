using UnityEngine;
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
    public CanvasGroup RoomListGroup;
    public GameObject RoomListLayoutGroup;
    public GameObject RoomPanelPrefab;
    public Toggle ReadyToggle;
	public Button P1ReadyButton;
	public Button P2ReadyButton;
	public Text P1ReadyButtonText;
	public Text P2ReadyButtonText;
    public UpdateUsernameText P1Username;
    public UpdateUsernameText P2Username;
    public Image P1ReadyStatus;
    public Image P2ReadyStatus;
    public Text CountdownField;
	public GameObject ScrollingBackground; 
	public Text LocalUserName;
    public Text ChatLog;
    public InputField ChatInput;
    public Button ChatButton;
    public Button JoinPublic;

    private PhotonView cPhotonView;
    private string username;
    private bool[] playersReady = new bool[Globals.MAX_CONNECTED_PLAYERS];
    private List<GameObject> roomPanelCloneList = new List<GameObject>();
    private InputField usernameField;
    private Button connectBtn;

    private List<string> chatLog = new List<string>();

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        cPhotonView = GetComponent<PhotonView>();
        usernameField = PreconnectGroup.GetComponentInChildren<InputField>();
        connectBtn = PreconnectGroup.GetComponentInChildren<Button>();
        ScrollingBackground.SetActive(true);

        PhotonNetwork.offlineMode = false;

        if (!PhotonNetwork.connectedAndReady)
            ToggleCanvasGroup(PreconnectGroup, true);
        else
        {
            ToggleCanvasGroup(LobbyGroup, true);
            ToggleCanvasGroup(RoomListGroup, true);
        }
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

//        JoinPublic.Select();
    }

    private void OnJoinedRoom()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add(PROP_IS_READY, false);
        PhotonNetwork.player.SetCustomProperties(properties);

        chatLog.Clear();

        MainLobbyCanvas.enabled = false;
        GameLobbyCanvas.enabled = true;
		UpdateReadyButtons();
        UpdateUsernames();
		EventSystem.current.SetSelectedGameObject(ChatInput.gameObject);
    }

    private void OnLeftRoom()
    {
        GameLobbyCanvas.enabled = false;
        MainLobbyCanvas.enabled = true;
		P1ReadyStatus.enabled = false;
		P2ReadyStatus.enabled = false;
    }

    private void OnMasterClientSwitched()
    {
        PhotonNetwork.LeaveRoom();
        playersReady[0] = false;
        playersReady[1] = false;
    }

    private void OnPhotonPlayerConnected()
    {
        UpdateUsernames();
    }

    private void OnPhotonPlayerDisconnected()
    {
        playersReady[1] = false;
        UpdateUsernames();
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
		EventSystem.current.SetSelectedGameObject(go);

//		ExecuteEvents.Execute<ISelectHandler>(go, new BaseEventData(EventSystem.current), ExecuteEvents.selectHandler);
	}

	private void UpdateReadyButtons(){
		switch(PhotonNetwork.player.ID){
			case 1:
				P1ReadyButton.gameObject.SetActive(true);
				P2ReadyButton.gameObject.SetActive(false);
				break;
			case 2:
				P1ReadyButton.gameObject.SetActive(false);
				P2ReadyButton.gameObject.SetActive(true);

				break;
		}
	}

    private void UpdateUsernames()
    {
        P1Username.UpdateText(1);
        P2Username.UpdateText(2);
    }

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

    public void OnClick_Connect()
    {
        username = usernameField.text;
		LocalUserName.text = username;
        ToggleCanvasGroup(PreconnectGroup, false);
		PreconnectGroup.gameObject.SetActive(false);

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

    }

    public void OnClick_JoinPrivateGame()
    {

    }

	public void OnClick_BackToLobby(){
		PhotonNetwork.LeaveRoom();
	}

    public void OnClick_BackToMainMenu()
    {
        DisconnectFromNetwork();
    }

    public void OnClick_Ready()
    {
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
        newProperties.Add(PROP_IS_READY, !playersReady[PhotonNetwork.player.ID - 1]);
        PhotonNetwork.player.SetCustomProperties(newProperties);

		cPhotonView.RPC("RPC_ClickReady", PhotonTargets.AllBufferedViaServer);
	}

    public void OnClick_SendChat()
    {
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
            playersReady[player.ID - 1] = (bool)player.customProperties[PROP_IS_READY];

            if (player.ID == 1)  // if this is Player 1
            {
				P1ReadyStatus.enabled = playersReady[player.ID-1];
				P1ReadyButtonText.text = (playersReady[player.ID-1] ? "READY" : "CANCEL");
 
            }
            else if (player.ID == 2)  // if this is Player 2
            {
				P2ReadyStatus.enabled = playersReady[player.ID-1];
				P2ReadyButtonText.text = (playersReady[player.ID-1] ? "READY" : "CANCEL");
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