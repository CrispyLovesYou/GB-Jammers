using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateUsernameText : MonoBehaviour
{
    #region Fields

    private Text cText;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cText = GetComponent<Text>();
    }

    #endregion

    #region Methods
	public void ClearText(){
		cText.text = "";
	}
    public void UpdateText(int _playerNum)
    {
		if(PhotonNetwork.playerList.Length > 1 || _playerNum == 1)	cText.text = PhotonNetwork.playerList[_playerNum - 1].name;
		else if (_playerNum == 2 ) cText.text = "";
    }

    #endregion
}