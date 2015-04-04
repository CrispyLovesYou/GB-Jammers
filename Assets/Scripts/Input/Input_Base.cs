using UnityEngine;
using System;
using System.Collections;

public abstract class Input_Base : MonoBehaviour
{
    #region Fields

    protected Controller_Player controller;
    protected Vector2 inputVector = Vector2.zero;
    protected PhotonView cPhotonView;

    protected bool isEnabled = true;
    protected bool appHasFocus = true;

    #endregion

    #region Unity Callbacks

    protected virtual void Awake()
    {
        controller = GetComponent<Controller_Player>();
        cPhotonView = GetComponent<PhotonView>();

        MatchManager.OnBeginResetAfterScore += Disable;
        MatchManager.OnCompleteResetAfterScore += Enable;
    }

    #endregion

    #region Callbacks

    private void Enable(object sender, EventArgs e)
    {
        isEnabled = true;
    }

    private void Disable(object sender, EventArgs e)
    {
        isEnabled = false;
    }

    #endregion
}