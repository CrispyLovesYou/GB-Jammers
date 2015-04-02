using UnityEngine;
using System.Collections;

[AddComponentMenu("Managers/Scene/Main Menu")]
public class MainMenuManager : Singleton<MainMenuManager>
{
    #region Constants

    private const string NETWORK_LOBBY_ID = "network_lobby";

    #endregion

    #region Fields

    public int DEBUG_CharacterID = 0;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Globals.GameMode = GameModes.MAIN_MENU;

        iTween.Defaults.easeType = iTween.EaseType.linear;

        if (Globals.CharacterDict.Count == 0)  // in case the splash screen didn't load Character Data (i.e. debugging in Editor)
            SplashScreenManager.LoadCharacterData();
    }

    #endregion

    #region Methods

    public void StartLocalMultiplayer()
    {
        Globals.GameMode = GameModes.LOCAL_MULTIPLAYER;
    }

    public void StartOnlineMultiplayer()
    {
        Globals.GameMode = GameModes.ONLINE_MULTIPLAYER;
        Application.LoadLevel(NETWORK_LOBBY_ID);
    }

    #endregion

    #region UI Callbacks

    public void OnClick_Debug()
    {
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.CreateRoom(null);
        Globals.SelectedCharacters[0] = (CharacterID)DEBUG_CharacterID;
        PhotonNetwork.LoadLevel("map_beach");
    }

    public void OnClick_VSMode()
    {
        StartLocalMultiplayer();
    }

    public void OnClick_Online()
    {
        StartOnlineMultiplayer();
    }

    #endregion
}