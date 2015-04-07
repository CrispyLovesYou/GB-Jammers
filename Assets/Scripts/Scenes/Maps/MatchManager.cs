using UnityEngine;
using System;
using System.Linq;
using System.Collections;

[AddComponentMenu("Managers/Game/Match Manager")]
public class MatchManager : Singleton<MatchManager>
{
    #region Events

    private static EventHandler<EventArgs> onMatchStart;
    public static event EventHandler<EventArgs> OnMatchStart
    {
        add
        {
            if (onMatchStart == null || !onMatchStart.GetInvocationList().Contains(value))
                onMatchStart += value;
        }
        remove { onMatchStart -= value; }
    }

    private static EventHandler<MatchEndEventArgs> onMatchEnd;
    public static event EventHandler<MatchEndEventArgs> OnMatchEnd
    {
        add
        {
            if (onMatchEnd == null || !onMatchEnd.GetInvocationList().Contains(value))
                onMatchEnd += value;
        }
        remove { onMatchEnd -= value; }
    }

    private static EventHandler<EventArgs> onVolley;
    public static event EventHandler<EventArgs> OnVolley
    {
        add
        {
            if (onVolley == null || !onVolley.GetInvocationList().Contains(value))
                onVolley += value;
        }
        remove { onVolley -= value; }
    }

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

    public int VolleyCountBeforeScore = 0;

    public string Team_Left_Spawn_Tag = "Team_Left_Spawn";
    public string Team_Right_Spawn_Tag = "Team_Right_Spawn";
    public string DiscPrefabID = "Disc";
    public string Disc_Spawn_Left_Tag = "Disc_Spawn_Left";
    public string Disc_Spawn_Right_Tag = "Disc_Spawn_Right";

    public int LobPointValue = 2;
    public int BonusPointValue = 0;

    public Vector3 TeamLeftSpawn { get; private set; }
    public Vector3 TeamRightSpawn { get; private set; }
    public Vector3 DiscLeftSpawn { get; private set; }
    public Vector3 DiscRightSpawn { get; private set; }
    public bool isInitialCatchComplete = false;
    public Team LastTeamToScore = Team.UNASSIGNED;

    public CanvasGroup PauseMenu;
    public bool IsPaused { get; private set; }
    public bool IsPauseAllowed = false;

    public bool HasMatchStarted = false;
    public Canvas WaitingForPlayer;

    public CanvasGroup SetStartCG;
    public CanvasGroup SetEndCG;
    public CanvasGroup MatchEndCG;
    public Animator SetStart;
    public bool SetStartComplete = false;
    public Animator SetEnd;
    public bool SetEndComplete = false;
    public Animator MatchEnd;
    public bool IsTransitioning = false;

    public AudioSource AudioMatchStart;
    public AudioSource AudioSetEnd;
    public AudioSource AudioMatchEnd;
    public AudioSource AP1;
    public AudioSource AP2;
    public AudioSource AP3;
    public AudioSource AP4;
    public AudioSource AP5;
    public AudioSource AP6;
    public AudioSource AP7;
    public AudioSource AP8;
    public AudioSource AP9;
    public AudioSource AP10;

    private PhotonView cPhotonView;
    private Team winner = Team.UNASSIGNED;
    private bool hasVolleyStarted = false;

    private bool remotePlayerReady = false;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Destroy(MainMenuMusic.Instance.gameObject);
		iTween.tweens.Clear();
        cPhotonView = GetComponent<PhotonView>();

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

        DiscLeftSpawn = GameObject.FindGameObjectWithTag(Disc_Spawn_Left_Tag).transform.position;
        DiscRightSpawn = GameObject.FindGameObjectWithTag(Disc_Spawn_Right_Tag).transform.position;

        Controller_Player.OnCatch += Controller_Player_OnCatch;

        if (Globals.GameMode == GameModes.ONLINE_MULTIPLAYER)
            WaitingForPlayer.enabled = true;

        if (!PhotonNetwork.isMasterClient)
            cPhotonView.RPC("RPC_SetRemotePlayerReady", PhotonTargets.AllBufferedViaServer);

        if (Globals.GameMode == GameModes.LOCAL_MULTIPLAYER)
            remotePlayerReady = true;
    }

    private void Start()
    {
        NetworkManager.Instance.Spawn();
        StartCoroutine(MatchDirector());
    }

	private void OnDestroy(){
		Controller_Player.OnCatch -= Controller_Player_OnCatch;
	}
    #endregion

    #region Methods

    public void ScorePoints(Team _team, int _points)
    {
        cPhotonView.RPC("RPC_ScorePoints", PhotonTargets.AllViaServer, (int)_team, _points);
    }

	public void Pause(){
		if (IsPaused)
		{
			if (Globals.GameMode == GameModes.LOCAL_MULTIPLAYER)
				Time.timeScale = 1.0f;
			
			PauseMenu.alpha = 0;
			PauseMenu.interactable = false;
			PauseMenu.blocksRaycasts = false;
			IsPaused = false;
		}
		else
		{
			if (IsPauseAllowed)
			{
				if (Globals.GameMode == GameModes.LOCAL_MULTIPLAYER)
					Time.timeScale = 0;
				
				PauseMenu.alpha = 1.0f;
				PauseMenu.interactable = true;
				PauseMenu.blocksRaycasts = true;
				IsPaused = true;
				PauseMenu.GetComponentInChildren<UnityEngine.UI.Button>().Select();
			}
		}
	}

	public void BackToLobby(){
		Time.timeScale = 1;
		switch(Globals.GameMode){
			case GameModes.ONLINE_MULTIPLAYER:
				PhotonNetwork.LeaveRoom();
				Application.LoadLevel("network_lobby");
				break;
			case GameModes.LOCAL_MULTIPLAYER:
				Application.LoadLevel("main_menu");
				break;
		}


	}

	public void BackToMainMenu(){
		Time.timeScale = 1;
		PhotonNetwork.Disconnect();
		Application.LoadLevel("main_menu");

	}

    private void SpawnDisc()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        Vector3 spawnPosition = Vector3.zero;

        if (UnityEngine.Random.Range(1, 3) == 1)
            spawnPosition = MatchManager.Instance.DiscLeftSpawn;
        else
            spawnPosition = MatchManager.Instance.DiscRightSpawn;

        PhotonNetwork.Instantiate(DiscPrefabID, spawnPosition, Quaternion.identity, 0);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
			Pause ();   
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator MatchDirector()
    {
        yield return StartCoroutine(HandleMatchSetup());
        
        // Main match loop
        while (winner == Team.UNASSIGNED)
        {
            yield return StartCoroutine(HandleSetStart());
            yield return StartCoroutine(HandleSet());
            yield return StartCoroutine(HandleSetEnd());
        }

        // Match over
        yield return StartCoroutine(HandleMatchOver());

        HasMatchStarted = false;
        Globals.HasGameStarted = false;

        if (PhotonNetwork.connectedAndReady)
            PhotonNetwork.Disconnect();

        Application.LoadLevel("main_menu");
    }

    private IEnumerator HandleMatchSetup()
    {
        IsTransitioning = true;

        while (!remotePlayerReady)
            yield return 0;

        WaitingForPlayer.enabled = false;

        HasMatchStarted = true;

        AudioMatchStart.Play();

        SpawnDisc();

        if (onMatchStart != null)
            onMatchStart(this, EventArgs.Empty);

        IsPauseAllowed = true;

        IsTransitioning = false;
    }

    private IEnumerator HandleSetStart()
    {
        IsTransitioning = true;

        CurrentSet++;

        SetStartCG.alpha = 1.0f;
        SetStart.enabled = true;
        SetStart.Play("SetStart", 0, 0);

        while (!SetStartComplete)
            yield return 0;

        SetStartCG.alpha = 0;
        SetStart.enabled = false;
        SetStartComplete = false;

        if (CurrentSet == 1)
            Disc.Instance.StartFadeIn();

        IsTransitioning = false;
    }

    private IEnumerator HandleSet()
    {
        while (L_Points < Rules.PointsToWinSet && R_Points < Rules.PointsToWinSet)
            yield return 0;
    }

    private IEnumerator HandleSetEnd()
    {
        IsTransitioning = true;

        if (L_Points >= Rules.PointsToWinSet)
            L_Sets++;
        else if (R_Points >= Rules.PointsToWinSet)
            R_Sets++;

        SetEndCG.alpha = 1.0f;
        SetEnd.enabled = true;
        SetEnd.Play("ScoreCounter", 0, 0);

        AudioSetEnd.Play();

        while (!SetEndComplete)
            yield return 0;

        SetEndCG.alpha = 0;
        SetEnd.enabled = false;
        SetEndComplete = false;

        L_Points = 0;
        R_Points = 0;

        if (L_Sets == Rules.SetsToWinMatch)
            winner = Team.LEFT;
        else if (R_Sets == Rules.SetsToWinMatch)
            winner = Team.RIGHT;

        IsTransitioning = false;
    }

    private IEnumerator HandleMatchOver()
    {
        IsTransitioning = true;

        MatchEndCG.alpha = 1.0f;
        MatchEnd.enabled = true;
        AudioMatchEnd.Play();

        if (onMatchEnd != null)
            onMatchEnd(this, new MatchEndEventArgs(winner, L_Sets, R_Sets));

        yield return new WaitForSeconds(5.0f);

        IsTransitioning = false;
    }

    private IEnumerator ResetAfterScore()
    {
        VolleyCountBeforeScore = 0;
        isInitialCatchComplete = false;
        hasVolleyStarted = false;

        if (onBeginResetAfterScore != null)
            onBeginResetAfterScore(this, EventArgs.Empty);

        yield return new WaitForSeconds(2.0f);

        if (onCompleteResetAfterScore != null)
            onCompleteResetAfterScore(this, EventArgs.Empty);
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnCatch(object sender, EventArgs e)
    {
        if (!isInitialCatchComplete)
            return;

        if (!hasVolleyStarted)
        {
            hasVolleyStarted = true;
            return;
        }

        // Volley is complete
        VolleyCountBeforeScore++;
        hasVolleyStarted = false;

        if (onVolley != null)
            onVolley(this, EventArgs.Empty);
    }

    #endregion

    #region RPC

    [RPC]
    private void RPC_SetRemotePlayerReady()
    {
        remotePlayerReady = true;
    }

    [RPC]
    private void RPC_ScorePoints(int _team, int _points)
    {
        switch ((Team)_team)
        {
            case Team.LEFT: L_Points += _points + BonusPointValue; break;
            case Team.RIGHT: R_Points += _points + BonusPointValue; break;
        }

        LastTeamToScore = (Team)_team;

        if (L_Points < Rules.PointsToWinSet && R_Points < Rules.PointsToWinSet)
        {
            switch (_points + BonusPointValue)
            {
                case 1: AP1.Play(); break;
                case 2: AP2.Play(); break;
                case 3: AP3.Play(); break;
                case 4: AP4.Play(); break;
                case 5: AP5.Play(); break;
                case 6: AP6.Play(); break;
                case 7: AP7.Play(); break;
                case 8: AP8.Play(); break;
                case 9: AP9.Play(); break;
                case 10: AP10.Play(); break;
            }
        }

        if (onScored != null)
            onScored(this, new ScoredEventArgs((Team)_team, _points));

        StartCoroutine(ResetAfterScore());
    }

    #endregion
}