using UnityEngine;
using System.Collections;

public class Input_Base : MonoBehaviour
{
    #region Fields

    protected Controller_Base controller;
    protected Vector2 inputVector = Vector2.zero;

    #endregion

    #region Unity Callbacks

    protected virtual void Awake()
    {
        controller = gameObject.GetSafeComponent<Controller_Player>();
    }

    #endregion
}