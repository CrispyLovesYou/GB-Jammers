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

    public void UpdateText(int _playerNum)
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.ID == _playerNum)
                cText.text = player.name;
        }

        if (_playerNum == 2 && PhotonNetwork.playerList.Length == 1)
            cText.text = "";
    }

    #endregion
}