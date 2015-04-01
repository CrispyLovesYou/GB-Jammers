﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Goal Area")]
public class GoalArea : MonoBehaviour
{
    #region Fields

    public Team TeamToScore = Team.LEFT;
    public int Points = 3;

    #endregion

    #region Unity Callbacks

    private void OnTriggerEnter2D(Collider2D _collider)
    {
        if (_collider.tag == "Disc" &&
            !Disc.Instance.IsScoring &&
            PhotonNetwork.isMasterClient &&
            !Controller_Player.isPingCompensating)
        {
            Disc.Instance.IsScoring = true;
            MatchManager.Instance.ScorePoints(TeamToScore, Points);
        }
    }

    #endregion
}