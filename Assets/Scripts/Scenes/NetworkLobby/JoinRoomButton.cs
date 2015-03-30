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
        NetworkLobbyManager.Instance.JoinRoom(RoomID);
    }

    #endregion
}