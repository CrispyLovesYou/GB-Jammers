using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Managers/Scene/Character Select")]
public class CharacterSelectManager : Singleton<CharacterSelectManager>
{
    #region Constants

    private const string MAP_BEACH = "map_beach";

    #endregion

    #region Fields

    public Canvas MainCanvas;
    public Text P1CharacterText;
    public Text P2CharacterText;

    private PhotonView cPhotonView;
    private bool[] playersReady = new bool[Globals.MAX_CONNECTED_PLAYERS];
    private int currentSelected = 0;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        cPhotonView = GetComponent<PhotonView>();
    }

    #endregion

    #region UI Callbacks

    public void OnClick_SelectCharacter(int _id)
    {
        currentSelected = _id;
        cPhotonView.RPC("RPC_SelectCharacter", PhotonTargets.AllViaServer, PhotonNetwork.player.ID, currentSelected);
    }

    public void OnClick_LockCharacter()
    {
        foreach (Button button in MainCanvas.GetComponentsInChildren<Button>())
        {
            button.interactable = false;
        }

        cPhotonView.RPC("RPC_LockCharacter", PhotonTargets.AllViaServer, PhotonNetwork.player.ID - 1, currentSelected);
    }

    #endregion

    #region Callbacks

    [RPC]
    private void RPC_SelectCharacter(int _playerNum, int _id)
    {
        switch (_playerNum)
        {
            case 1:
                switch (_id)
                {
                    case (int)CharacterID.DR_TRACKSUIT: P1CharacterText.text = "Dr. Tracksuit"; break;
                    case (int)CharacterID.V_BOMB: P1CharacterText.text = "V-Bomb"; break;
                    case (int)CharacterID.DIRTY_DAN_RYCKERT: P1CharacterText.text = "Dirty Dan Ryckert"; break;
                    case (int)CharacterID.METAL_GEAR_SCANLON: P1CharacterText.text = "Metal Gear Scanlon"; break;
                }
                break;

            case 2:
                switch (_id)
                {
                    case (int)CharacterID.DR_TRACKSUIT: P2CharacterText.text = "Dr. Tracksuit"; break;
                    case (int)CharacterID.V_BOMB: P2CharacterText.text = "V-Bomb"; break;
                    case (int)CharacterID.DIRTY_DAN_RYCKERT: P2CharacterText.text = "Dirty Dan Ryckert"; break;
                    case (int)CharacterID.METAL_GEAR_SCANLON: P2CharacterText.text = "Metal Gear Scanlon"; break;
                }
                break;
        }
    }

    [RPC]
    private void RPC_LockCharacter(int _index, int _id)
    {
        Globals.SelectedCharacters[_index] = (CharacterID)_id;
        playersReady[_index] = true;

        if (playersReady[0] && playersReady[1] && PhotonNetwork.isMasterClient)
            PhotonNetwork.LoadLevel(MAP_BEACH);
    }

    #endregion
}