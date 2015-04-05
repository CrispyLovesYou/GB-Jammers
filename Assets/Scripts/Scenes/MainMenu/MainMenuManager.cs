using UnityEngine;
using System.Collections;

[AddComponentMenu("Managers/Scene/Main Menu")]
public class MainMenuManager : Singleton<MainMenuManager>
{
    #region Constants

    private const string NETWORK_LOBBY_ID = "network_lobby";

    #endregion

    #region Fields

    public int DEBUG_CharacterID_1 = 0;
    public int DEBUG_CharacterID_2 = 0;

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
        // Debug
        Globals.GameMode = GameModes.LOCAL_MULTIPLAYER;
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.CreateRoom(null);

		//Player 2 is set to controller mode by default. 
		//If we have less than 2 controllers, set player 2 to keyboard
		string[] controllerNames = Input.GetJoystickNames();
		if(controllerNames.Length <= 1) Globals.PlayerInputs[1] = InputType.KEYBOARD;

		// Even if the controller names array length is 2 or greater, 
		// disconnected controllers leave blank slots in the array.
		// Check each one, and if there's a null slot, then set player 2 to keyboard.
		else{
			foreach(string name in controllerNames){
				if(string.IsNullOrEmpty(name)){
					Globals.PlayerInputs[1] = InputType.KEYBOARD;
					break;
				}
			}
		}
		// Load character select level directly
		PhotonNetwork.LoadLevel("character_select");
        if (NetworkManager.IsNull)   gameObject.AddComponent<NetworkManager>();

    }

    public void StartOnlineMultiplayer()
    {
        Globals.GameMode = GameModes.ONLINE_MULTIPLAYER;
		if(Input.GetJoystickNames().Length > 0 && string.IsNullOrEmpty(Input.GetJoystickNames()[0]) == false){
			Globals.PlayerInputs[0] = InputType.CONTROLLER;
		}else{
			Globals.PlayerInputs[0] = InputType.KEYBOARD;
		}

        Application.LoadLevel(NETWORK_LOBBY_ID);
    }

    #endregion

    #region UI Callbacks

    public void OnClick_Debug()
    {
        Globals.GameMode = GameModes.DEBUG;
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.CreateRoom(null);
        Globals.SelectedCharacters[0] = (CharacterID)DEBUG_CharacterID_1;
        if (NetworkManager.IsNull)
            gameObject.AddComponent<NetworkManager>();
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