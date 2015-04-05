using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConnectionStatusUpdates : MonoBehaviour
{
    public enum ConnectStatus
    {
        NORMAL,
        DISCONNECTED,
        REMOTE_DISCONNECTED
    }

    #region Constants

    private const string MAIN_MENU_SCENE = "main_menu";
    private const string NETWORK_LOBBY_SCENE = "network_lobby";

    #endregion

    #region Fields

    private CanvasGroup cCanvasGroup;
    private Text cText;

    private ConnectStatus currentStatus = ConnectStatus.NORMAL;
    private bool isActive = false;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cCanvasGroup = GetComponent<CanvasGroup>();
        cText = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (!isActive)
            return;

        if (Input.GetButtonDown("Submit"))
        {
            switch (currentStatus)
            {
                case ConnectStatus.DISCONNECTED: Application.LoadLevel(MAIN_MENU_SCENE); break;
                case ConnectStatus.REMOTE_DISCONNECTED: Application.LoadLevel(NETWORK_LOBBY_SCENE); break;
            }
        }
    }

    #endregion

    #region Network Callbacks

    private void OnDisconnectedFromPhoton()
    {
        cCanvasGroup.alpha = 1.0f;
        currentStatus = ConnectStatus.DISCONNECTED;
        cText.text = "You have been disconnected";
        isActive = true;

    }

    private void OnPhotonPlayerDisconnected()
    {
        if (!Globals.HasGameStarted)
            return;

        cCanvasGroup.alpha = 1.0f;
        currentStatus = ConnectStatus.REMOTE_DISCONNECTED;
        cText.text = "Player has disconnected";
        isActive = true;
    }

    #endregion
}