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
            PhotonNetwork.isMasterClient &&
            !Controller_Player.isPingCompensating)
        {
            StartCoroutine(WaitForLag());
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator WaitForLag()
    {
        float delay = PhotonNetwork.GetPing() / 1000;
        yield return new WaitForSeconds(delay * 2);
        MatchManager.Instance.ScorePoints(TeamToScore, Points);
    }

    #endregion
}