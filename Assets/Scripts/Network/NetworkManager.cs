﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Managers/Game/Network Manager")]
public class NetworkManager : Singleton<NetworkManager>
{
    #region Constants

    private const string PREFAB_ID_TRACKSUIT = "char_dr-tracksuit";
    private const string PREFAB_ID_VBOMB = "char_v-bomb";
    private const string PREFAB_ID_DDR = "char_dirty-dan-ryckert";
    private const string PREFAB_ID_MGS = "char_metal-gear-scanlon";

    public const int ROOM_ID_RANDOM_SIZE = 5;

    #endregion

    #region Fields

    public string Player_Prefab_ID = "Player";

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        if (NetworkManager.Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        base.Awake();

        PhotonNetwork.automaticallySyncScene = true;
        DontDestroyOnLoad(this);
    }

    #endregion

    #region Methods

    public void ConnectToNetwork()
    {
        if (PhotonNetwork.connected || PhotonNetwork.connecting)
            return;

        PhotonNetwork.ConnectUsingSettings(Globals.GAME_VERSION);
        PhotonNetwork.playerName = Globals.Username;
    }

	public bool CreateRoom(string _roomID = "", bool _isVisible = true)
    {
		if(string.IsNullOrEmpty(_roomID)) _roomID = Globals.Username + " [" + RandomHelper.RandomString(ROOM_ID_RANDOM_SIZE) + "]";

		RoomOptions roomOptions = new RoomOptions() { maxPlayers = Globals.MAX_CONNECTED_PLAYERS, isVisible = _isVisible };

		return PhotonNetwork.CreateRoom(_roomID, roomOptions, TypedLobby.Default);
    }

    public void Spawn()
    {
        switch (Globals.GameMode)
        {
            case GameModes.LOCAL_MULTIPLAYER:
                SpawnLocalPlayer(1); SpawnLocalPlayer(2);
                break;

            case GameModes.ONLINE_MULTIPLAYER:
            case GameModes.DEBUG:
                SpawnOnlinePlayer();
                break;
        }
    }

    private void SpawnLocalPlayer(int _playerNum)
    {
        int index = _playerNum - 1;
        Team team = Team.UNASSIGNED;

        switch (_playerNum)
        {
            case 1: team = Team.LEFT; break;
            case 2: team = Team.RIGHT; break;
        }

        string prefabID = "";

        switch (Globals.SelectedCharacters[index])
        {
            case CharacterID.DR_TRACKSUIT: prefabID = PREFAB_ID_TRACKSUIT; break;
            case CharacterID.V_BOMB: prefabID = PREFAB_ID_VBOMB; break;
            case CharacterID.DIRTY_DAN_RYCKERT: prefabID = PREFAB_ID_DDR; break;
            case CharacterID.METAL_GEAR_SCANLON: prefabID = PREFAB_ID_MGS; break;
        }

        Vector3 spawnPosition = Vector3.zero;

        switch (team)
        {
            case Team.LEFT:
                spawnPosition = MatchManager.Instance.TeamLeftSpawn;
                break;

            case Team.RIGHT:
                spawnPosition = MatchManager.Instance.TeamRightSpawn;
                break;
        }

        GameObject player = PhotonNetwork.Instantiate(prefabID, spawnPosition, Quaternion.identity, 0) as GameObject;
        Controller_Player cPlayer = player.GetComponent<Controller_Player>();
        cPlayer.SetData(team, Globals.SelectedCharacters[index]);
        cPlayer.Team = team;

		// Player 1 is always controller 1...
		// We need to handle what happens to Player 2.
		if(_playerNum == 2){
			player.GetComponent<Input_Joy>().enabled = false;
			switch(Globals.PlayerInputs[1]){
				// If player 2 is in keyboard mode, use default settings.
				case InputType.KEYBOARD:
					player.AddComponent<Input_KM>();	
					break;
				
				// If player 2's in controller mode, then attach controller 2
				case InputType.CONTROLLER:
					player.AddComponent<Input_Joy2>();
					break;
			}
		}

        // ========== DEBUG CODE
        if (Globals.GameMode == GameModes.DEBUG)
        {
            cPlayer.Meter = 100;
            cPlayer.MeterForEX = 0;
            cPlayer.MeterForSuper = 0;
        }

        // =====================

        SpawnChargeBar(player, team);
    }

    private void SpawnOnlinePlayer()
    {
		int index = Globals.PlayerID - 1;
        Team team = Team.UNASSIGNED;
        
		switch (index)
        {
            case 0: team = Team.LEFT; PhotonNetwork.player.SetTeam(PunTeams.Team.blue); break;
            case 1: team = Team.RIGHT; PhotonNetwork.player.SetTeam(PunTeams.Team.red); break;
        }

        string prefabID = "";

        switch (Globals.SelectedCharacters[index])
        {
            case CharacterID.DR_TRACKSUIT: prefabID = PREFAB_ID_TRACKSUIT; break;
            case CharacterID.V_BOMB: prefabID = PREFAB_ID_VBOMB; break;
            case CharacterID.DIRTY_DAN_RYCKERT: prefabID = PREFAB_ID_DDR; break;
            case CharacterID.METAL_GEAR_SCANLON: prefabID = PREFAB_ID_MGS; break;
        }

        Vector3 spawnPosition = Vector3.zero;

        switch (team)
        {
            case Team.LEFT:
                spawnPosition = MatchManager.Instance.TeamLeftSpawn;
                break;

            case Team.RIGHT:
                spawnPosition = MatchManager.Instance.TeamRightSpawn;
                break;
        }

        GameObject player = PhotonNetwork.Instantiate(prefabID, spawnPosition, Quaternion.identity, 0) as GameObject;
        Controller_Player cPlayer = player.GetComponent<Controller_Player>();
        cPlayer.SetData(team, Globals.SelectedCharacters[index]);
        cPlayer.Team = team;

		switch(Globals.PlayerInputs[0]){
		case InputType.KEYBOARD:
			player.GetComponent<Input_Joy>().enabled = false;
			player.AddComponent<Input_KM>();
			break;
		}

        // ========== DEBUG CODE
        if (Globals.GameMode == GameModes.DEBUG)
        {
            cPlayer.Meter = 100;
            cPlayer.MeterForEX = 0;
            cPlayer.MeterForSuper = 0;
        }
        // =====================

        SpawnChargeBar(player, team);
    }

    private void SpawnChargeBar(GameObject _player, Team _team)
    {
        GameObject chargeBar = PhotonNetwork.Instantiate("Charge Bar", Vector3.zero, Quaternion.identity, 0);
        chargeBar.GetComponentInChildren<ChargeBar>().SetPlayer(_player);
    }

    #endregion
}