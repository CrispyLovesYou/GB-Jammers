using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu("Input/Joystick 1")]
public class Input_Joy : Input_Base
{
    #region Constants

    private const string INPUT_HORIZONTAL = "Joy1_Horizontal";
    private const string INPUT_VERTICAL = "Joy1_Vertical";
    private const string INPUT_ACTION = "Joy1_Action";
    private const string INPUT_CANCEL = "Joy1_Cancel";
    private const string INPUT_EX = "Joy1_EX";
    private const string INPUT_SUPER = "Joy1_Super";

    #endregion

    #region Fields

    public float DeadZone = 0.2f;

    private bool isEnabled = true;
    private bool appHasFocus = true;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        MatchManager.OnBeginResetAfterScore += Disable;
        MatchManager.OnCompleteResetAfterScore += Enable;
    }

    private void Update()
    {
        if (!isEnabled || !appHasFocus)
            return;

        CheckMovement();
        CheckAction();
        CheckLob();
        CheckEX();
    }

    private void OnApplicationFocus(bool _hasFocus)
    {
        appHasFocus = _hasFocus;
    }

    #endregion

    #region Methods

    private void CheckMovement()
    {
        float horizontal = Input.GetAxis(INPUT_HORIZONTAL);
        float vertical = Input.GetAxis(INPUT_VERTICAL);
        inputVector = new Vector2(horizontal, vertical);

        if (inputVector.magnitude < DeadZone)
            inputVector = Vector2.zero;

        if (inputVector != Vector2.zero)
            controller.Move(inputVector);
        else
            controller.Stop();
    }

    private void CheckAction()
    {
        if (Input.GetButtonDown(INPUT_ACTION))
            controller.Action(inputVector);

        if (Input.GetButtonUp(INPUT_ACTION))
            controller.ReleaseAction(inputVector);
    }

    private void CheckLob()
    {
        if (Input.GetButtonDown(INPUT_CANCEL))
            controller.Lob(inputVector);
    }

    private void CheckEX()
    {
        if (Input.GetButtonDown(INPUT_EX))
            controller.EX(inputVector);
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