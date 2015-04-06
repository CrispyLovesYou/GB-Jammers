using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Passive/Metal Gear Scanlon")]
public class Passive_MetalGearScanlon : Passive_Base
{
    #region Fields

    public int Amount = 5;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnCatch += Controller_Player_OnCatch;
    }

	private void OnDestroy(){
		Controller_Player.OnCatch -= Controller_Player_OnCatch;
	}

    #endregion

    #region Callbacks

    private void Controller_Player_OnCatch(object sender, System.EventArgs e)
    {
        if (!MatchManager.Instance.isInitialCatchComplete ||
            (Controller_Player)sender != player)
            return;

        player.Meter += Amount;
    }

    #endregion
}