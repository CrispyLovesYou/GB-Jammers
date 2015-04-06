using UnityEngine;
using System.Collections;

public class SetEnd : MonoBehaviour
{
    #region Fields

    public Animator P1X0;
    public Animator P10X;
    public Animator P2X0;
    public Animator P20X;
    public Animator P1Set;
    public Animator P2Set;

    #endregion

    #region Unity Callbacks

    private void Update()
    {
        P1X0.SetInteger("value", Mathf.FloorToInt(MatchManager.Instance.L_Points / 10.0f));
        P10X.SetInteger("value", MatchManager.Instance.L_Points % 10);
        P2X0.SetInteger("value", Mathf.FloorToInt(MatchManager.Instance.R_Points / 10.0f));
        P20X.SetInteger("value", MatchManager.Instance.R_Points % 10);
        P1Set.SetInteger("value", MatchManager.Instance.L_Sets);
        P2Set.SetInteger("value", MatchManager.Instance.R_Sets);
    }

    #endregion

    #region Methods

    public void SetEndComplete()
    {
        MatchManager.Instance.SetEndComplete = true;
    }

    #endregion
}