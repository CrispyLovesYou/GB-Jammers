using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/V-Bomb")]
public class Super_VBomb : Super_Base
{
    #region Fields

    public GameObject Shield;
    public float xPos = 4.0f;

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
        float scaleX = 0;

        switch (player.Team)
        {
            case Team.LEFT: position.x = -xPos; scaleX = 1; break;
            case Team.RIGHT: position.x = xPos; scaleX = -1;  break;
        }

        GameObject obj = Instantiate(Shield, position, Quaternion.identity) as GameObject;
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * scaleX, obj.transform.localScale.y, 1);
    }

    #endregion
}