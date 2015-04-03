using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/Dr. Tracksuit")]
public class Super_DrTracksuit : Super_Base
{
    #region Fields

    public float MoveSpeedMultiplier = 0.5f;
    public float DashSpeedMultiplier = 0.5f;
    public bool IsActive { get; private set; }

    private Controller_Player targetPlayer;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnSuper += Controller_Player_OnSuper;
        MatchManager.OnScored += MatchManager_OnScored;
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnSuper(object sender, SuperEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj != this.gameObject)
                targetPlayer = obj.GetSafeComponent<Controller_Player>();
        }

        targetPlayer.MoveSpeedMultiplier *= MoveSpeedMultiplier;
        targetPlayer.DashSpeedMultiplier *= DashSpeedMultiplier;
        IsActive = true;
    }

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        if (e.Team != player.Team || !IsActive)
            return;

        targetPlayer.MoveSpeedMultiplier /= MoveSpeedMultiplier;
        targetPlayer.DashSpeedMultiplier /= DashSpeedMultiplier;
    }

    #endregion
}