using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/UI/Scoreboard/Player Score")]
public class PlayerScore_Update : MonoBehaviour
{
    public enum Digit
    {
        Ones,
        Tens
    }

    #region Fields

    public Digit DigitPlace = Digit.Ones;
    public Team Team = Team.LEFT;

    private Animator cAnimator;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        int score = 0;

        switch (Team)
        {
            case global::Team.LEFT: score = MatchManager.Instance.L_Points; break;
            case global::Team.RIGHT: score = MatchManager.Instance.R_Points; break;
        }

        int digit = 0;

        switch (DigitPlace)
        {
            case Digit.Ones:
                digit = score % 10;
                break;

            case Digit.Tens:
                digit = Mathf.FloorToInt(score / 10);
                break;
        }

        cAnimator.SetInteger("value", digit);
    }

    #endregion
}