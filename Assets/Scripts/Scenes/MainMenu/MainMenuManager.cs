using UnityEngine;
using System.Collections;

[AddComponentMenu("Managers/Scene/Main Menu")]
public class MainMenuManager : Singleton<MainMenuManager>
{
    #region Constants

    private const string NETWORK_LOBBY_ID = "network_lobby";

    #endregion

    #region Fields
    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        LoadCharacterData();
    }

    #endregion

    #region Methods

    public void StartLocalMultiplayer()
    {

    }

    public void StartOnlineMultiplayer()
    {

    }

    private void LoadCharacterData()
    {
        TextAsset ta;

        ta = Resources.Load("data_dr-tracksuit") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.DR_TRACKSUIT, Character.Deserialize(ta.text));

        ta = Resources.Load("data_v-bomb") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.V_BOMB, Character.Deserialize(ta.text));

        ta = Resources.Load("data_dirty-dan-ryckert") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.DIRTY_DAN_RYCKERT, Character.Deserialize(ta.text));

        ta = Resources.Load("data_metal-gear-scanlon") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.METAL_GEAR_SCANLON, Character.Deserialize(ta.text));
    }

    #endregion

    #region UI Callbacks

    public void OnClick_VSMode()
    {

    }

    public void OnClick_Online()
    {
        Application.LoadLevel(NETWORK_LOBBY_ID);
    }

    #endregion
}