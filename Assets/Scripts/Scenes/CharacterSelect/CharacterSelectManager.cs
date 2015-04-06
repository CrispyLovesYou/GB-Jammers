using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[AddComponentMenu("Managers/Scene/Character Select")]
public class CharacterSelectManager : Singleton<CharacterSelectManager>
{
    #region Constants

    private const string MAP_BEACH = "map_beach";
	private const float THROTTLE_TIME = 0.5f;
	public const string PROP_PLAYER_ID = "playerID";
    #endregion

    #region Fields

	public EventSystem LocalEventSystem;
    public Canvas MainCanvas;
    public Text P1CharacterText;
    public Text P2CharacterText;

	public CharacterPanel P1CharacterPanel;
	public CharacterPanel P2CharacterPanel;

	public RectTransform P1Selector;
	public RectTransform P2Selector;
	public RectTransform DualSelector;

	public RectTransform[] CharacterButtons;

    private PhotonView cPhotonView;
    private bool[] playersReady = new bool[Globals.MAX_CONNECTED_PLAYERS];
	private int p1CurrentSelected = 0;
	private int p2CurrentSelected = 1;
	private float p1Throttle = 0;
	private float p2Throttle = 0;

	private int playerID = 1;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();

		// Assign Unity UI's event system's default selection, depending on player number
		switch(Globals.GameMode){
			case GameModes.ONLINE_MULTIPLAYER:
				playerID = (int)PhotonNetwork.player.customProperties[PROP_PLAYER_ID];
				switch(playerID){
				case 1:
					LocalEventSystem.firstSelectedGameObject = CharacterButtons[0].gameObject;
					break;
				case 2:
					LocalEventSystem.firstSelectedGameObject = CharacterButtons[1].gameObject;
					break;
				}
				break;
			case GameModes.LOCAL_MULTIPLAYER:
				LocalEventSystem.firstSelectedGameObject = CharacterButtons[0].gameObject;
				
				break;
		}
		Globals.HasGameStarted = true;
        cPhotonView = GetComponent<PhotonView>();

    }

	private IEnumerator Start(){
		if(EventSystem.current == null) yield return null;
		// Force Unity's EventSystem to select the starting character.
		// Unfortunately this is necessary for any UI that requires multiple simultaneous selection...
		SelectCharacter(1, 0);
		SelectCharacter(2, 1);

	}

	private void Update(){
		CheckInput();
	}
    #endregion

	#region UI Callbacks

	public void OnClick_LockCharacter(int _id){
		// Only activate for mouse input.
		if(Input.GetMouseButtonUp(0) == false){
			return;
		}
		switch(Globals.GameMode){
			case GameModes.ONLINE_MULTIPLAYER:
				switch(playerID){
					case 1:
						p1CurrentSelected = _id;
						cPhotonView.RPC("RPC_SelectCharacter", PhotonTargets.All, 1, p1CurrentSelected);
						cPhotonView.RPC("RPC_LockCharacter", PhotonTargets.All, 0, p1CurrentSelected);
						break;
					default:
						p2CurrentSelected = _id;
						cPhotonView.RPC("RPC_SelectCharacter", PhotonTargets.All, 2, p2CurrentSelected);
						cPhotonView.RPC("RPC_LockCharacter", PhotonTargets.All, 1, p2CurrentSelected);
						break;
				}
				break;
			case GameModes.LOCAL_MULTIPLAYER:
				SelectCharacter(1, _id);
				LockCharacter(0, _id);
				break;
		}
	}

	#endregion

    #region Input

	// Custom input read through Update loop
	// Last resort due to the Unity's input system being incompatible with multiple selection
	// and differentiating between different input sources.
	private void CheckInput(){
		// Handle Cancel button press
		if(Globals.PlayerInputs[1] == InputType.KEYBOARD   && Input.GetButtonDown("Key_Action") || 
		   Globals.PlayerInputs[1] == InputType.CONTROLLER && Input.GetButtonDown("Joy2_Action")){
			OnConfirmPressed(1);
		}else if(Input.GetButtonDown("Joy1_Action")){
			OnConfirmPressed(0);
		}

		if(Input.GetButtonDown("Joy1_Cancel")){
			OnCancelPressed(1);
			return;
		}
		else if(Input.GetButtonDown("Key_Cancel") || Input.GetButtonDown("Joy2_Cancel")){
			OnCancelPressed(2);
			return;
		}
		
		// Handle input throttle
		if(p1Throttle > 0) p1Throttle -= Time.deltaTime;
		if(p2Throttle > 0) p2Throttle -= Time.deltaTime;
		
		// Handle horizontal input and button selection
		switch(Globals.GameMode){
		case GameModes.ONLINE_MULTIPLAYER:
			if(playersReady[playerID - 1] == false &&  Input.GetAxis("Joy1_MenuHorizontal") != 0 ){
				switch(playerID){
				case 1:
					if(p1Throttle > 0) return;
					if(Input.GetAxis("Joy1_MenuHorizontal") > 0) {
						p1CurrentSelected++;
						if(p1CurrentSelected >= 4) p1CurrentSelected = 0;
					}else{
						p1CurrentSelected--;
						if(p1CurrentSelected < 0) p1CurrentSelected = 3;
					}
					cPhotonView.RPC("RPC_SelectCharacter", PhotonTargets.All, PhotonNetwork.player.ID, p1CurrentSelected);
					p1Throttle = THROTTLE_TIME;
					break;
				case 2:
					if(p2Throttle > 0) return;
					if(Input.GetAxis("Joy1_MenuHorizontal") > 0) {
						p2CurrentSelected++;
						if(p2CurrentSelected >= 4) p2CurrentSelected = 0;
					}else{
						p2CurrentSelected--;
						if(p2CurrentSelected < 0) p2CurrentSelected = 3;
					}
					cPhotonView.RPC("RPC_SelectCharacter", PhotonTargets.All, PhotonNetwork.player.ID, p2CurrentSelected);
					p2Throttle = THROTTLE_TIME;
					break;
				}
			} 
			break;
		case GameModes.LOCAL_MULTIPLAYER:
			if(playersReady[0] == false && p1Throttle <= 0 && Input.GetAxis("Joy1_MenuHorizontal") != 0 ){
				if(Input.GetAxis("Joy1_MenuHorizontal") > 0) {
					p1CurrentSelected++;
					if(p1CurrentSelected >= 4) p1CurrentSelected = 0;
				}else{
					p1CurrentSelected--;
					if(p1CurrentSelected < 0) p1CurrentSelected = 3;
				}
				SelectCharacter(1, p1CurrentSelected);
				p1Throttle = THROTTLE_TIME;
			} 
			
			// Player 2 may receive input from keyboard or joystick
			if(playersReady[1] == false && p2Throttle <= 0 && (Input.GetAxis("Key_MenuHorizontal") != 0 || Input.GetAxis("Joy2_MenuHorizontal") != 0)){
				float axis = Globals.PlayerInputs[1] == InputType.CONTROLLER ?  Input.GetAxis("Joy2_MenuHorizontal")  : Input.GetAxis("Key_MenuHorizontal");
				if(axis > 0) {
					p2CurrentSelected++;
					if(p2CurrentSelected >= 4) p2CurrentSelected = 0;
				}else{
					p2CurrentSelected--;
					if(p2CurrentSelected < 0) p2CurrentSelected = 3;
				}
				SelectCharacter(2, p2CurrentSelected);
				p2Throttle = THROTTLE_TIME;
			}
			break;
		}
	}
	public void OnConfirmPressed(int _playerId){
		switch(Globals.GameMode){
		case GameModes.ONLINE_MULTIPLAYER:
			switch(playerID){
			case 1:
				cPhotonView.RPC("RPC_LockCharacter", PhotonTargets.All, 0, p1CurrentSelected);
				break;
			case 2:
				cPhotonView.RPC("RPC_LockCharacter", PhotonTargets.All, 1, p2CurrentSelected);
				break;
			}
			break;
		case GameModes.LOCAL_MULTIPLAYER:
			if(Globals.PlayerInputs[1] == InputType.KEYBOARD   && Input.GetButtonDown("Key_Action") || 
			   Globals.PlayerInputs[1] == InputType.CONTROLLER && Input.GetButtonDown("Joy2_Action")){
				LockCharacter(1, p2CurrentSelected);
			}else{
				LockCharacter(0, p1CurrentSelected);
			}
			break;
		}
	}


	public void OnCancelPressed(int _playerId){
		switch(Globals.GameMode){
		case GameModes.ONLINE_MULTIPLAYER:
			switch(playerID){
				case 1:
					if(playersReady[playerID - 1]) cPhotonView.RPC("RPC_CancelCharacter", PhotonTargets.All, playerID);
					break;
				case 2:
					 ReturnToPrevMenu();
					break;
			}
			break;
		case GameModes.LOCAL_MULTIPLAYER:
			if(playersReady[_playerId - 1])CancelCharacter(_playerId);
			else ReturnToPrevMenu();
			break;
		}
	}
	
	#endregion
	#region Methods

	private	void SelectCharacter(int _playerNum, int _id){
		switch (_playerNum)
		{
		case 1:
			p1CurrentSelected = _id;
			P1Selector.anchoredPosition = new Vector2(CharacterButtons[_id].anchoredPosition.x, P1Selector.anchoredPosition.y) ;
			P1CharacterPanel.OnCharacterSelect(_id);
			break;
			
		case 2:
			p2CurrentSelected = _id;
			P2Selector.anchoredPosition = new Vector2(CharacterButtons[_id].anchoredPosition.x, P2Selector.anchoredPosition.y) ;
			P2CharacterPanel.OnCharacterSelect(_id);
			break;
			
		}

		if(p1CurrentSelected == p2CurrentSelected && DualSelector.GetComponent<Image>().enabled == false){
			P1Selector.GetComponent<Image>().enabled = false;	
			P2Selector.GetComponent<Image>().enabled = false;
			DualSelector.GetComponent<Image>().enabled = true;
			DualSelector.anchoredPosition = P1Selector.anchoredPosition;
		}else if(p1CurrentSelected != p2CurrentSelected && DualSelector.GetComponent<Image>().enabled == true) {
			P1Selector.GetComponent<Image>().enabled = true;	
			P2Selector.GetComponent<Image>().enabled = true;
			DualSelector.GetComponent<Image>().enabled = false;
		}
	}

	private void LockCharacter(int _index, int _id){
//		Debug.Log ("What the hell come on");
		Globals.SelectedCharacters[_index] = (CharacterID)_id;
		playersReady[_index] = true;
		switch(_index){
		case 0:
			P1CharacterPanel.OnCharacterConfirm();
			break;
		case 1:
			P2CharacterPanel.OnCharacterConfirm();
			break;
		}

        if (playersReady[0] && playersReady[1] && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(MAP_BEACH);
        }
	}

	private void CancelCharacter(int _playerNum){
		playersReady[_playerNum -1] = false;
		switch(_playerNum){
			case 1:
				P1CharacterPanel.OnCharacterCancel();
				SelectCharacter(_playerNum, p1CurrentSelected);
				break;
			case 2:
				P2CharacterPanel.OnCharacterCancel();
				SelectCharacter(_playerNum, p2CurrentSelected);
				break;
		}
	}

	private void ReturnToPrevMenu(){
		switch(Globals.GameMode){
			case GameModes.ONLINE_MULTIPLAYER:
				Debug.Log ("Attempting to leave room");
				PhotonNetwork.LeaveRoom();
				Application.LoadLevel("network_lobby");
				
				break;
			default:
				Application.LoadLevel("main_menu");
				break;
		}
	}
	
	#endregion
    #region RPC

    [RPC]
    private void RPC_SelectCharacter(int _playerNum, int _id)
    {
		SelectCharacter(_playerNum, _id);
    }

    [RPC]
    private void RPC_LockCharacter(int _index, int _id)
    {
		LockCharacter(_index, _id);
    }

	[RPC]
	private void RPC_CancelCharacter(int _playerNum)
	{
		CancelCharacter(_playerNum);
	}
	
	#endregion
}