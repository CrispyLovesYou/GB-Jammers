﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;

[AddComponentMenu("Game/Match Manager")]
public class MatchManager : Singleton<MatchManager>
{
    #region Events

    private static EventHandler<ScoredEventArgs> onScored;
    public static event EventHandler<ScoredEventArgs> OnScored
    {
        add
        {
            if (onScored == null || !onScored.GetInvocationList().Contains(value))
                onScored += value;
        }
        remove { onScored -= value; }
    }

    private static EventHandler<EventArgs> onBeginResetAfterScore;
    public static event EventHandler<EventArgs> OnBeginResetAfterScore
    {
        add
        {
            if (onBeginResetAfterScore == null || !onBeginResetAfterScore.GetInvocationList().Contains(value))
                onBeginResetAfterScore += value;
        }
        remove { onBeginResetAfterScore -= value; }
    }

    private static EventHandler<EventArgs> onCompleteResetAfterScore;
    public static event EventHandler<EventArgs> OnCompleteResetAfterScore
    {
        add
        {
            if (onCompleteResetAfterScore == null || !onCompleteResetAfterScore.GetInvocationList().Contains(value))
                onCompleteResetAfterScore += value;
        }
        remove { onCompleteResetAfterScore -= value; }
    }

    #endregion

    #region Fields

    public static MatchRules Rules;

    public int CurrentSet = 0;
    public int L_Points = 0;
    public int L_Sets = 0;
    public int R_Points = 0;
    public int R_Sets = 0;

    public string Team_Left_Spawn_Tag = "Team_Left_Spawn";
    public string Team_Right_Spawn_Tag = "Team_Right_Spawn";
    public string Disc_Spawn_Tag = "Disc_Spawn";

    public Vector3 TeamLeftSpawn { get; private set; }
    public Vector3 TeamRightSpawn { get; private set; }
    public Vector3 DiscSpawn { get; private set; }

    private Team winner = Team.UNASSIGNED;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();

        if (Rules.SetsToWinMatch == 0)
        {
            Rules = new MatchRules()
            {
                SetsToWinMatch = Globals.DEFAULT_SETS_TO_WIN_MATCH,
                PointsToWinSet = Globals.DEFAULT_POINTS_TO_WIN_SET
            };
        }

        TeamLeftSpawn = GameObject.FindGameObjectWithTag(Team_Left_Spawn_Tag).transform.position;
        TeamRightSpawn = GameObject.FindGameObjectWithTag(Team_Right_Spawn_Tag).transform.position;
        DiscSpawn = GameObject.FindGameObjectWithTag(Disc_Spawn_Tag).transform.position;
    }

    private void Start()
    {
        StartCoroutine(MatchDirector());
    }

    #endregion

    #region Methods

    public void ScorePoints(Team _team, int _points)
    {
        switch (_team)
        {
            case Team.LEFT: L_Points += _points; break;
            case Team.RIGHT: R_Points += _points; break;
        }

        if (onScored != null)
            onScored(this, new ScoredEventArgs(_team));

        StartCoroutine(ResetAfterScore());
    }

    #endregion

    #region Coroutines

    private IEnumerator MatchDirector()
    {
        yield return StartCoroutine(HandleMatchSetup());
        
        // Main match loop
        while (winner == Team.UNASSIGNED)
        {
            if (L_Points >= Rules.PointsToWinSet ||
                R_Points >= Rules.PointsToWinSet)
            {
                yield return StartCoroutine(HandleSetWon());
            }

            yield return 0;
        }

        // Match over
        yield return StartCoroutine(HandleMatchOver());

        // temporary until a Match Over menu is implemented
        if (PhotonNetwork.connectedAndReady)
            PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("main_menu");
    }

    private IEnumerator HandleMatchSetup()
    {
        yield return 0;
    }

    private IEnumerator HandleSetWon()
    {
        if (L_Points >= Rules.PointsToWinSet)
            L_Sets++;
        else if (R_Points >= Rules.PointsToWinSet)
            R_Sets++;

        yield return 0;

        L_Points = 0;
        R_Points = 0;

        if (L_Sets == Rules.SetsToWinMatch)
            winner = Team.LEFT;
        else if (R_Sets == Rules.SetsToWinMatch)
            winner = Team.RIGHT;
    }

    private IEnumerator HandleMatchOver()
    {
        yield return 0;
    }

    private IEnumerator ResetAfterScore()
    {
        onBeginResetAfterScore(this, EventArgs.Empty);
        yield return new WaitForSeconds(2.0f);
        Disc.IsScoring = false;
        onCompleteResetAfterScore(this, EventArgs.Empty);
    }

    #endregion
}