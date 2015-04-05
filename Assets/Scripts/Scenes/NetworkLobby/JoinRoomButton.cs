using UnityEngine;
using System.Collections;

public class JoinRoomButton : MonoBehaviour
{
    #region Fields

    public string RoomID;

    #endregion

    #region UI Callbacks

    public void OnClick_Join()
    {
		Debug.Log ("Attempting to join D:");
        NetworkLobbyManager.Instance.JoinRoom(RoomID);
    }

    #endregion
}