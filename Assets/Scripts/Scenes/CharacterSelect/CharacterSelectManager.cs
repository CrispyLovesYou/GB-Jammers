using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[AddComponentMenu("Managers/Scene/Character Select")]
public class CharacterSelectManager : Singleton<CharacterSelectManager>
{
    #region Constants

    private const string MAP_BEACH = "map_beach";

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
    private int currentSelected = 0;
	private int p1CurrentSelected = 0;
	private int p2CurrentSelected = 0;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();

		// Assign Unity UI's event system's default selection, depending on player number
		switch(PhotonNetwork.player.ID){
		case 1:
			LocalEventSystem.firstSelectedGameObject = CharacterButtons[0].gameObject;
			break;
		case 2:
			LocalEventSystem.firstSelectedGameObject = CharacterButtons[1].gameObject;
			break;
		}
        cPhotonView = GetComponent<PhotonView>();
    }

    #endregion

    #region UI Callbacks

    public void OnClick_SelectCharacter(int _id)
    {
        currentSelected = _id;
		Debug.Log ("Current selected is " + currentSelected);
		cPhotonView.RPC("RPC_SelectCharacter", PhotonTargets.All, PhotonNetwork.player.ID, currentSelected);
    }

    public void OnClick_LockCharacter()
    {
        cPhotonView.RPC("RPC_LockCharacter", PhotonTargets.All, PhotonNetwork.player.ID - 1, currentSelected);
    }

	public void OnClick_ReturnToMenu(){
		Debug.Log ("Loading previous menu");
	}

    #endregion

    #region RPC

    [RPC]
    private void RPC_SelectCharacter(int _playerNum, int _id)
    {
		playersReady[_playerNum - 1] = false;
        switch (_playerNum)
        {
            case 1:
				p1CurrentSelected = _id;
				P1Selector.anchoredPosition = new Vector2(CharacterButtons[_id].anchoredPosition.x, P1Selector.anchoredPosition.y) ;
				P1CharacterPanel.OnCharacterSelect(_id);
                break;

            case 2:
				p2CurrentSelected = _id;
				P2Selector.anchoredPosition = new Vector2(CharacterButtons[_id].anchoredPosition.x, P1Selector.anchoredPosition.y) ;
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

    [RPC]
    private void RPC_LockCharacter(int _index, int _id)
    {

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
            PhotonNetwork.LoadLevel(MAP_BEACH);
    }

    #endregion
}