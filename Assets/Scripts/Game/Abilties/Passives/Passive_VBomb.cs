using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Passive/V-Bomb")]
public class Passive_VBomb : Passive_Base
{
    #region Fields

    public int VolleysToActivate = 2;
    public int MaxStacks = 5;
    public float ThrowPowerMod = 1.0f;
    public float KnockbackMod = 0.5f;
    public float StabilityMod = 0.5f;
    public float MoveSpeedMod = -0.25f;
    public float DashSpeedMod = -0.75f;

    private int nVolleys = 0;
    private int currentStacks = 0;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        MatchManager.OnVolley += MatchManager_OnVolley;
        MatchManager.OnScored += MatchManager_OnScored;
    }

	private void OnDestroy(){
		MatchManager.OnVolley -= MatchManager_OnVolley;
		MatchManager.OnScored -= MatchManager_OnScored;
	}

    #endregion

    #region Callbacks

    private void MatchManager_OnVolley(object sender, System.EventArgs e)
    {
        nVolleys++;

        if (nVolleys < VolleysToActivate)
            return;

        if (currentStacks > MaxStacks)
            return;

        player.ThrowPowerMod += ThrowPowerMod;
        player.KnockbackMod += KnockbackMod;
        player.StabilityMod += StabilityMod;
        player.CallOnPowerBuff();

        player.MoveSpeedMod += MoveSpeedMod;
        player.DashSpeedMod += DashSpeedMod;
        player.CallOnSpeedDebuff();

        nVolleys = 0;
        currentStacks++;
    }

    void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        player.ThrowPowerMod -= ThrowPowerMod * currentStacks;
        player.KnockbackMod -= KnockbackMod * currentStacks;
        player.StabilityMod -= StabilityMod * currentStacks;
        player.MoveSpeedMod -= MoveSpeedMod * currentStacks;
        player.DashSpeedMod -= DashSpeedMod * currentStacks;

        currentStacks = 0;
    }

    #endregion
}