using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/V-Bomb")]
public class Super_VBomb : Super_Base
{
    #region Fields

    public GameObject Shield;
    public float xPos = 4.3f;

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
        if ((Controller_Player)sender != player)
            return;

        Vector3 position = Vector3.zero;

        switch (player.Team)
        {
            case Team.LEFT: position.x = -xPos; break;
            case Team.RIGHT: position.x = xPos; break;
        }

        Instantiate(Shield, position, Quaternion.identity);
    }

    #endregion
}