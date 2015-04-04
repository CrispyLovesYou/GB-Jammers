using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/V-Bomb")]
public class Super_VBomb : Super_Base
{
    #region Fields
    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnSuper += Controller_Player_OnSuper;
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnSuper(object sender, SuperEventArgs e)
    {
        
    }

    #endregion
}