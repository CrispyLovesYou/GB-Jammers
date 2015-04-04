using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/EX/Dr. Tracksuit")]
public class EX_DrTracksuit : EX_Base
{
    #region Fields

    public bool HasKnockback = false;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();

        Controller_Player.OnEX += Controller_Player_OnEX;
        Controller_Player.OnCatch += Controller_Player_OnCatch;
        MatchManager.OnScored += MatchManager_OnScored;
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnEX(object sender, EXEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        Vector2 inputVector = e.InputVector.normalized;

        if (inputVector.y == 0)
            inputVector.y = 1.0f;

        Disc.Instance.IsMagnet = true;
        player.SpecialThrow(inputVector, HasKnockback);
    }

    private void Controller_Player_OnCatch(object sender, System.EventArgs e)
    {
        Disc.Instance.IsMagnet = false;
    }

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        Disc.Instance.IsMagnet = false;
    }

    #endregion
}