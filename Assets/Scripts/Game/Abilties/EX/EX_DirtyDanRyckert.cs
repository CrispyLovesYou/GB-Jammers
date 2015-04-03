using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/EX/Dirty Dan Ryckert")]
public class EX_DirtyDanRyckert : EX_Base
{
    #region Fields

    public int BonusPoints = 1;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnEX += Controller_Player_OnEX;
        MatchManager.OnScored += MatchManager_OnScored;
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnEX(object sender, EXEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        MatchManager.Instance.BonusPointValue += BonusPoints;
    }

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        MatchManager.Instance.BonusPointValue = 0;
    }

    #endregion
}