using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller_Player))]
public abstract class Passive_Base : MonoBehaviour
{
    #region Fields

    protected Controller_Player player;

    #endregion

    #region Unity Callbacks

    protected virtual void Awake()
    {
        player = GetComponent<Controller_Player>();
    }

    #endregion
}