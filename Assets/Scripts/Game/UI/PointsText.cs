using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Game/UI/Points Text")]
public class PointsText : MonoBehaviour
{
    #region Fields

    public Team Team = Team.LEFT;

    private Text cText;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cText = GetComponent<Text>();
    }

    private void Update()
    {
        switch (Team)
        {
            case global::Team.LEFT: cText.text = MatchManager.Instance.L_Points.ToString(); break;
            case global::Team.RIGHT: cText.text = MatchManager.Instance.R_Points.ToString(); break;
        }
    }

    #endregion
}