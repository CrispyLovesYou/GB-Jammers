using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/(Passive) Dr. Tracksuit")]
public class Passive_DrTracksuit : Passive_Base
{
    #region Fields

    public float MoveSpeedUp = 1.0f;
    public float DashSpeedUp = 3.0f;
    public float Duration = 10.0f;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        MatchManager.OnScored += MatchManager_OnScored;
    }

    #endregion

    #region Coroutines

    private IEnumerator Effect()
    {
        player.MoveSpeedMod += MoveSpeedUp;
        player.DashSpeedMod += DashSpeedUp;
        player.CallOnSpeedBuff();

        yield return new WaitForSeconds(Duration);

        player.MoveSpeedMod -= MoveSpeedUp;
        player.DashSpeedMod -= DashSpeedUp;
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        if (e.Team != player.Team)
            return;

        StartCoroutine(Effect());
    }

    #endregion
}