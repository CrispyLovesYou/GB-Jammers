using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/EX/Metal Gear Scanlon")]
public class EX_MetalGearScanlon : EX_Base
{
    #region Fields

    public GameObject Book;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnEX += Controller_Player_OnEX;
    }

	private void OnDestroy(){
		Controller_Player.OnEX -= Controller_Player_OnEX;
	}

    #endregion

    #region Callbacks

    private void Controller_Player_OnEX(object sender, EXEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        GameObject gObj = Instantiate(Book, player.transform.position, Quaternion.identity) as GameObject;
        gObj.GetSafeComponent<Book>().TargetPosition = player.GetLobTargetPosition(e.InputVector);
        player.State = PlayerState.AIM;
    }

    #endregion
}