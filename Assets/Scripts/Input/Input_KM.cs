﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Input/Keyboard")]
public class Input_KM : Input_Base
{
    #region Constants

    private const string INPUT_LEFT = "Key_Left";
    private const string INPUT_RIGHT = "Key_Right";
    private const string INPUT_UP = "Key_Up";
    private const string INPUT_DOWN = "Key_Down";
    private const string INPUT_ACTION = "Key_Action";
    private const string INPUT_CANCEL = "Key_Cancel";
    private const string INPUT_EX = "Key_EX";
    private const string INPUT_SUPER = "Key_Super";

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        if (!isEnabled || !appHasFocus || !cPhotonView.isMine)
            return;

        CheckMovement();
        CheckAction();
        CheckLob();
        CheckEX();
        CheckSuper();
    }

    #endregion

    #region Methods

    private void CheckMovement()
    {
        inputVector = Vector2.zero;

        if (Input.GetButton(INPUT_LEFT))
            inputVector.x = -1.0f;
        else if (Input.GetButton(INPUT_RIGHT))
            inputVector.x = 1.0f;

        if (Input.GetButton(INPUT_UP))
            inputVector.y = 1.0f;
        else if (Input.GetButton(INPUT_DOWN))
            inputVector.y = -1.0f;

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