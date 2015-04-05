using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu("Input/Joystick 2")]
public class Input_Joy2 : Input_Base
{
    #region Constants

    private const string INPUT_HORIZONTAL = "Joy2_Horizontal";
    private const string INPUT_VERTICAL = "Joy2_Vertical";
    private const string INPUT_ACTION = "Joy2_Action";
    private const string INPUT_CANCEL = "Joy2_Cancel";
    private const string INPUT_EX = "Joy2_EX";
    private const string INPUT_SUPER = "Joy2_Super";

    #endregion

    #region Fields

    public float DeadZone = 0.2f;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!isEnabled ||
            !appHasFocus ||
            !cPhotonView.isMine ||
            Time.timeScale == 0)
            return;

        CheckMovement();
        CheckAction();
        CheckLob();
        CheckEX();
        CheckSuper();
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

    private void CheckSuper()
    {
        if (Input.GetButtonDown(INPUT_SUPER))
            controller.Super(inputVector);
    }

    #endregion
}