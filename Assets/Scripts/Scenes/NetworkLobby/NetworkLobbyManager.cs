using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Managers/Scene/Network Lobby")]
public class NetworkLobbyManager : Singleton<NetworkLobbyManager>
{
    #region Constants

    public const string GAME_VERSION = "0.1";
    public const int MAX_CONNECTED_PLAYERS = 2;
    public const int ROOM_ID_RANDOM_SIZE = 5;

    public const string PROP_IS_READY = "isReady";

    private const string MSG_CONNECTING = "Connecting...";

    private const string MAIN_MENU_ID = "main_menu";
    private const string CHARACTER_SELECT_ID = "character_select";

    #endregion

    #region Fields

    public Canvas MainLobbyCanvas;
    public Canvas GameLobbyCanvas;
    public CanvasGroup LobbyGroup;
    public CanvasGroup PreconnectGroup;
    public CanvasGroup StatusGroup;
    public CanvasGroup RoomListGroup;
    public GameObject RoomListLayoutGroup;
    public GameObject RoomPanelPrefab;
    public Toggle ReadyToggle;
    public UpdateUsernameText P1Username;
    public UpdateUsernameText P2Username;
    public Text P1ReadyStatus;
    public Text P2ReadyStatus;
    public Text CountdownField;

    private PhotonView cPhotonView;
    private string username;
    private bool[] playersReady = new bool[MAX_CONNECTED_PLAYERS];
    private List<GameObject> roomPanelCloneList = new List<GameObject>();
    private InputField usernameField;
    private Button connectBtn;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        cPhotonView = GetComponent<PhotonView>();
        usernameField = PreconnectGroup.GetComponentInChildren<InputField>();
        connectBtn = PreconnectGroup.GetComponentInChildren<Button>();
        PhotonNetwork.automaticallySyncScene = true;
    }

    #endregion

    #region Network Callbacks

    private void OnJoinedLobby()
    {
        ToggleCanvasGroup(StatusGroup, false);
        ToggleCanvasGroup(LobbyGroup, true);
        ToggleCanvasGroup(RoomListGroup, true);
    }

    private void OnReceivedRoomListUpdate()
    {
        RefreshRoomList();
    }

    private void OnJoinedRoom()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add(PROP_IS_READY, false);
        PhotonNetwork.player.SetCustomProperties(properties);

        MainLobbyCanvas.enabled = false;
        GameLobbyCanvas.enabled = true;

        UpdateUsernames();
    }

    private void OnLeftRoom()
    {
        GameLobbyCanvas.enabled = false;
        MainLobbyCanvas.enabled = true;
        ReadyToggle.isOn = false;
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

    private void ConnectToNetwork()
    {
        if (PhotonNetwork.connected || PhotonNetwork.connecting)
            throw new Exception("Already connected to PhotonNetwork!");

        PhotonNetwork.ConnectUsingSettings(GAME_VERSION);
        PhotonNetwork.playerName = username;
        StatusGroup.GetComponentInChildren<Text>().text = MSG_CONNECTING;
        ToggleCanvasGroup(StatusGroup, true);
    }

    private void DisconnectFromNetwork()
    {
        PhotonNetwork.Disconnect();
    }

    private void CreateRoom()
    {
        string roomID = username + " [" + RandomHelper.RandomString(ROOM_ID_RANDOM_SIZE) + "]";

        RoomOptions roomOptions = new RoomOptions() { maxPlayers = MAX_CONNECTED_PLAYERS };
        PhotonNetwork.CreateRoom(roomID, roomOptions, TypedLobby.Default);
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

            if (roomInfo.playerCount != MAX_CONNECTED_PLAYERS)
            {
                GameObject roomPanel = Instantiate(RoomPanelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                roomPanelCloneList.Add(roomPanel);
                roomPanel.transform.SetParent(RoomListLayoutGroup.transform, false);
                roomPanel.GetComponentInChildren<Text>().text = roomInfo.name;
                roomPanel.GetComponentInChildren<Button>().gameObject.GetSafeComponent<JoinRoomButton>().RoomID = roomInfo.name;
            }
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

    private IEnumerator HandleGameCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            if (!playersReady[0] || !playersReady[1])
                break;

            if (i != 1)
                CountdownField.text = "Game starting in " + i.ToString() + " seconds...";
            else
                CountdownField.text = "Game starting in 1 second...";

            yield return new WaitForSeconds(1.0f);
        }

        if (playersReady[0] && playersReady[1])
            PhotonNetwork.LoadLevel(CHARACTER_SELECT_ID);
        else
            CountdownField.text = "";
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
        ToggleCanvasGroup(PreconnectGroup, false);

        ConnectToNetwork();
    }

    public void OnClick_CreateGame()
    {
        CreateRoom();
    }

    public void OnClick_CreatePrivateGame()
    {

    }

    public void OnClick_JoinPrivateGame()
    {

    }

    public void OnClick_BackToMainMenu()
    {
        DisconnectFromNetwork();
    }

    public void OnToggle_Ready()
    {
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
        newProperties.Add(PROP_IS_READY, ReadyToggle.isOn);
        PhotonNetwork.player.SetCustomProperties(newProperties);

        cPhotonView.RPC("RPC_ToggleReady", PhotonTargets.AllViaServer);
    }

    #endregion

    #region Callbacks

    [RPC]
    private void RPC_ToggleReady()
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            playersReady[player.ID - 1] = (bool)player.customProperties[PROP_IS_READY];

            if (player.ID == 1)  // if this is Player 1
            {
                switch (playersReady[player.ID - 1])
                {
                    case true: P1ReadyStatus.text = "Player 1 is ready!"; break;
                    case false: P1ReadyStatus.text = ""; break;
                }
            }
            else if (player.ID == 2)  // if this is Player 2
            {
                switch (playersReady[player.ID - 1])
                {
                    case true: P2ReadyStatus.text = "Player 2 is ready!"; break;
                    case false: P2ReadyStatus.text = ""; break;
                }
            }
        }

        if (playersReady[0] && playersReady[1])
            StartCoroutine(HandleGameCountdown());
    }

    #endregion
}