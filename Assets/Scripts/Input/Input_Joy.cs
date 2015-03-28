using UnityEngine;
using System.Collections;

public class Input_Joy : Input_Base
{
    #region Constants

    private const string INPUT_HORIZONTAL = "Joy1_Horizontal";
    private const string INPUT_VERTICAL = "Joy1_Vertical";
    private const string INPUT_ACTION = "Joy1_Action";
    private const string INPUT_CANCEL = "Joy1_Cancel";

    #endregion

    #region Fields

    public float DeadZone = 0.2f;

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        CheckMovement();
        CheckAction();
        CheckLob();
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
    }

    private void CheckLob()
    {
        if (Input.GetButtonDown(INPUT_CANCEL))
            controller.Lob(inputVector);
    }

    #endregion
}