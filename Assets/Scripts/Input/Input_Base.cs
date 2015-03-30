using UnityEngine;
using System.Collections;

public abstract class Input_Base : MonoBehaviour
{
    #region Fields

    protected Controller_Player controller;
    protected Vector2 inputVector = Vector2.zero;

    #endregion

    #region Unity Callbacks

    protected virtual void Awake()
    {
        controller = gameObject.GetSafeComponent<Controller_Player>();
    }

    #endregion
}