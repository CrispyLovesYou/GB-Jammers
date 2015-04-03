using UnityEngine;
using System.Collections;

[AddComponentMenu("Managers/Game/Network Manager")]
public class NetworkManager : Singleton<NetworkManager>
{
    #region Constants

    private const string PREFAB_ID_TRACKSUIT = "char_dr-tracksuit";
    private const string PREFAB_ID_VBOMB = "char_v-bomb";
    private const string PREFAB_ID_DDR = "char_dirty-dan-ryckert";
    private const string PREFAB_ID_MGS = "char_metal-gear-scanlon";

    #endregion

    #region Fields

    public string Player_Prefab_ID = "Player";
    public string Disc_Prefab_ID = "Disc";
    public string Crosshair_Prefab_ID = "Crosshair";

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            SpawnDisc();
            SpawnMisc();
        }

        SpawnLocalPlayer();
    }

    #endregion

    #region Methods

    private void SpawnLocalPlayer()
    {
        int index = PhotonNetwork.player.ID - 1;
        Team team = Team.UNASSIGNED;
        
        switch (PhotonNetwork.player.ID)
        {
            case 1: team = Team.LEFT; PhotonNetwork.player.SetTeam(PunTeams.Team.blue); break;
            case 2: team = Team.RIGHT; PhotonNetwork.player.SetTeam(PunTeams.Team.red); break;
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
        player.GetComponent<Input_Joy>().enabled = true;

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

    private void SpawnDisc()
    {
        Vector3 spawnPosition = MatchManager.Instance.DiscSpawn;
        PhotonNetwork.Instantiate(Disc_Prefab_ID, spawnPosition, Quaternion.identity, 0);
    }

    private void SpawnChargeBar(GameObject _player, Team _team)
    {
        GameObject chargeBar = PhotonNetwork.Instantiate("Charge Bar", Vector3.zero, Quaternion.identity, 0);
        chargeBar.GetComponent<ChargeBar>().SetPlayer(_player);
    }

    private void SpawnMisc()
    {
        PhotonNetwork.Instantiate(Crosshair_Prefab_ID, Vector3.zero, Quaternion.identity, 0);
    }

    #endregion
}