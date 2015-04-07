using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Goal Area")]
public class GoalArea : MonoBehaviour
{
    #region Fields

    public Team TeamToScore = Team.LEFT;
    public int Points = 3;

    private static bool hasScored = false;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        MatchManager.OnCompleteResetAfterScore += MatchManager_OnCompleteResetAfterScore;
		hasScored = false;
    }

	private void OnDestroy(){
		MatchManager.OnCompleteResetAfterScore -= MatchManager_OnCompleteResetAfterScore;
	}

    private void OnTriggerEnter2D(Collider2D _collider)
    {
        if (_collider.tag == "Disc" &&
            !hasScored)
        {
            if (Globals.GameMode == GameModes.ONLINE_MULTIPLAYER)
            {
                if (TeamToScore == Team.RIGHT && PhotonNetwork.isMasterClient)
                {
                    hasScored = true;
                    MatchManager.Instance.ScorePoints(TeamToScore, Points);
                }
                else if (TeamToScore == Team.LEFT && !PhotonNetwork.isMasterClient)
                {
                    hasScored = true;
                    MatchManager.Instance.ScorePoints(TeamToScore, Points);
                }
            }
            else if (Globals.GameMode == GameModes.LOCAL_MULTIPLAYER)
            {
                hasScored = true;
                MatchManager.Instance.ScorePoints(TeamToScore, Points);
            }
        }
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnCompleteResetAfterScore(object sender, System.EventArgs e)
    {
        hasScored = false;
    }

    #endregion
}