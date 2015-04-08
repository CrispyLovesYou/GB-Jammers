using UnityEngine;
using System.Collections;

public class MatchStart : MonoBehaviour
{
    #region Fields

    public Animator P1Portrait;
    public Animator P2Portrait;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        P1Portrait.SetInteger("value", (int)Globals.SelectedCharacters[0]);
        P2Portrait.SetInteger("value", (int)Globals.SelectedCharacters[1]);
    }

    #endregion

    #region Methods

    public void MatchStartTransitionComplete()
    {
        MatchManager.Instance.MatchStartTransitionComplete = true;
    }

    public void MatchStartComplete()
    {
        MatchManager.Instance.MatchStartComplete = true;
    }

    #endregion
}