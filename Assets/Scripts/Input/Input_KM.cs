using UnityEngine;
using System.Collections;

public class Input_KM : Input_Base
{
    #region Constants

    private const string INPUT_LEFT = "Key_Left";
    private const string INPUT_RIGHT = "Key_Right";
    private const string INPUT_UP = "Key_Up";
    private const string INPUT_DOWN = "Key_Down";
    private const string INPUT_ACTION = "Key_Action";

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        CheckMovement();
        CheckAction();
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

        controller.Move(inputVector);
    }

    private void CheckAction()
    {
        if (Input.GetButtonDown(INPUT_ACTION))
            controller.Action(inputVector);
    }

    #endregion
}