using UnityEngine;
using System.Collections;

public abstract class Super_Base : MonoBehaviour
{
    #region Fields

    protected Controller_Player player;

    #endregion

    #region Unity Callbacks

    protected virtual void Awake()
    {
        player = gameObject.GetSafeComponent<Controller_Player>();
    }

    #endregion
}