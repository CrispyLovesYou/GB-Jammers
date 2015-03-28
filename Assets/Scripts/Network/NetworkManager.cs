﻿using UnityEngine;
using System.Collections;

public class NetworkManager : Singleton<NetworkManager>
{
    #region Constants

    private const string GAME_VERSION = "0.1";
    private const int MAX_CONNECTED_PLAYERS = 2;

    #endregion

    #region Fields

    public string Player_Left_ID = "Player_Left";
    public string Player_Right_ID = "Player_Right";
    public string Disc_ID = "Disc";
    public string Player_Left_Spawn_Tag = "Team_Left_Spawn";
    public string Player_Right_Spawn_Tag = "Team_Right_Spawn";
    public string Disc_Spawn_Tag = "Disc_Spawn";
    public string Crosshair_ID = "Crosshair";

    #endregion

    #region Unity Callbacks

    private void OnGUI()
    {
        float buttonWidth = 200;
        float buttonHeight = 60;
        float buttonSpacing = 20;

        if (!PhotonNetwork.connected)
        {
            if (!PhotonNetwork.connecting)
            {
                if (GUI.Button(new Rect(0, 0, buttonWidth, buttonHeight), "Connect to Server"))
                    PhotonNetwork.ConnectUsingSettings(GAME_VERSION);

                if (GUI.Button(new Rect(buttonWidth + buttonSpacing, 0, buttonWidth, buttonHeight), "Offline Mode"))
                {
                    PhotonNetwork.offlineMode = true;
                    PhotonNetwork.CreateRoom(null);
                }
            }
            else
                GUI.Label(new Rect(0, 0, buttonWidth, buttonHeight), "Connecting...");
        }
        else
        {
            if (PhotonNetwork.insideLobby)
            {
                if (GUI.Button(new Rect(0, 0, buttonWidth, buttonHeight), "Create Room"))
                    CreateRoom();

                RoomInfo[] roomInfoList = PhotonNetwork.GetRoomList();

                for (int i = 0; i < roomInfoList.Length; i++)
                {
                    RoomInfo roomInfo = roomInfoList[i];

                    if (GUI.Button(new Rect((buttonWidth + buttonSpacing) * (i + 1), 0, buttonWidth, buttonHeight), "Join: " + roomInfo.name))
                        JoinRoom(roomInfo);
                }
            }
        }
    }
    
    #endregion

    #region Network Callbacks

    private void OnCreatedRoom()
    {
        SpawnDisc();
        SpawnMisc();
    }

    private void OnJoinedRoom()
    {
        string prefabID = Player_Left_ID;
        Team joinTeam = Team.UNASSIGNED;

        if (PhotonNetwork.room.playerCount == 1)
        {
            prefabID = Player_Left_ID;
            joinTeam = Team.LEFT;
        }
        else if (PhotonNetwork.room.playerCount == 2)
        {
            prefabID = Player_Right_ID;
            joinTeam = Team.RIGHT;
        }

        SpawnPlayer(prefabID, joinTeam);
    }

    #endregion

    #region Methods

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { maxPlayers = MAX_CONNECTED_PLAYERS };
        PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
    }

    private void JoinRoom(RoomInfo _roomInfo)
    {
        PhotonNetwork.JoinRoom(_roomInfo.name);
    }

    private void SpawnPlayer(string _prefabID, Team _team)
    {
        Vector3 spawnPosition = Vector3.zero;

        switch (_team)
        {
            case Team.LEFT:
                spawnPosition = GameObject.FindGameObjectWithTag(Player_Left_Spawn_Tag).transform.position;
                break;

            case Team.RIGHT:
                spawnPosition = GameObject.FindGameObjectWithTag(Player_Right_Spawn_Tag).transform.position;
                break;
        }

        GameObject player = PhotonNetwork.Instantiate(_prefabID, spawnPosition, Quaternion.identity, 0) as GameObject;
        player.GetComponent<Controller_Player>().Team = _team;
        player.GetComponent<Input_Joy>().enabled = true;
    }

    private void SpawnDisc()
    {
        Vector3 spawnPosition = GameObject.FindGameObjectWithTag(Disc_Spawn_Tag).transform.position;
        PhotonNetwork.Instantiate(Disc_ID, spawnPosition, Quaternion.identity, 0);
    }

    private void SpawnMisc()
    {
        PhotonNetwork.Instantiate(Crosshair_ID, Vector3.zero, Quaternion.identity, 0);
    }

    #endregion
}