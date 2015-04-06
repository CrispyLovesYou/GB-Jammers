using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchEnd : MonoBehaviour
{
    #region Fields

    public Animator P1Portrait;
    public Animator P2Portrait;
    public Image P1Win;
    public Image P1Lose;
    public Image P2Win;
    public Image P2Lose;
    public Animator P1Set;
    public Animator P2Set;

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        P1Portrait.SetInteger("value", (int)Globals.SelectedCharacters[0]);
        P2Portrait.SetInteger("value", (int)Globals.SelectedCharacters[1]);
        P1Set.SetInteger("value", MatchManager.Instance.L_Sets);
        P2Set.SetInteger("value", MatchManager.Instance.R_Sets);

        Color color = Color.white;
        color.a = 0;
        
        if (MatchManager.Instance.L_Sets > MatchManager.Instance.R_Sets)
        {
            P1Win.color = Color.white;
            P2Lose.color = Color.white;
            P1Lose.color = color;
            P2Win.color = color;
        }
        else
        {
            P1Lose.color = Color.white;
            P2Win.color = Color.white;
            P1Win.color = color;
            P2Lose.color = color;
        }
    }

    #endregion
}