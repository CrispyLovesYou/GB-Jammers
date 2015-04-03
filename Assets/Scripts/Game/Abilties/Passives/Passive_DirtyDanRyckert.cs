using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Passive/Dirty Dan Ryckert")]
public class Passive_DirtyDanRyckert : Passive_Base
{
    #region Fields

    public float MoveSpeedUp = 0.1f;
    public float DashSpeedUp = 0.2f;
    public float ThrowPowerUp = 0.3f;
    public float KnockbackUp = 0.1f;
    public float StabilityUp = 0.05f;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        MatchManager.OnScored += MatchManager_OnScored;
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        if (e.Team != player.Team)
        {
            player.MoveSpeedMod += MoveSpeedUp * e.Points;
            player.DashSpeedMod += DashSpeedUp * e.Points;
            player.ThrowPowerMod += ThrowPowerUp * e.Points;
            player.KnockbackMod += KnockbackUp * e.Points;
            player.StabilityMod += StabilityUp * e.Points;
            player.CallOnSpeedBuff();
            player.CallOnPowerBuff();
        }
    }

    #endregion
}