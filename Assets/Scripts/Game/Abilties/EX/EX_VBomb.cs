using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/EX/V-Bomb")]
public class EX_VBomb : EX_Base
{
    #region Fields

    public bool HasKnockback = true;
    public int ThrowCharge = 100;
    public float ThrowPowerMultiplier = 1.1f;
    public float KnockbackMultiplier = 1.2f;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnEX += Controller_Player_OnEX;
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnEX(object sender, EXEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        player.ThrowPowerMultiplier *= ThrowPowerMultiplier;
        player.KnockbackMultiplier *= KnockbackMultiplier;

        player.SpecialThrow(ThrowCharge, e.InputVector, HasKnockback);

        player.ThrowPowerMultiplier /= ThrowPowerMultiplier;
        player.KnockbackMultiplier /= KnockbackMultiplier;
    }

    #endregion
}