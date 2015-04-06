using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/Dr. Tracksuit")]
public class Super_DrTracksuit : Super_Base
{
    #region Fields

    public float MoveSpeedMultiplier = 0.5f;
    public float DashSpeedMultiplier = 0.5f;
    public bool IsActive { get; private set; }
    public GameObject Snow;
    public float PosX = 2.6f;

    private Controller_Player targetPlayer;
    private GameObject snow;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnSuper += Controller_Player_OnSuper;
        MatchManager.OnScored += MatchManager_OnScored;
    }

	private void OnDestroy(){
		Controller_Player.OnSuper -= Controller_Player_OnSuper;
		MatchManager.OnScored -= MatchManager_OnScored;
	}

    #endregion

    #region Callbacks

    private void Controller_Player_OnSuper(object sender, SuperEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj != this.gameObject)
                targetPlayer = obj.GetSafeComponent<Controller_Player>();
        }

        targetPlayer.MoveSpeedMultiplier *= MoveSpeedMultiplier;
        targetPlayer.DashSpeedMultiplier *= DashSpeedMultiplier;

        Vector3 position = Vector3.zero;
        position.y = -0.5f;

        switch (player.Team)
        {
            case Team.LEFT: position.x = PosX; break;
            case Team.RIGHT: position.x = -PosX; break;
        }

        snow = Instantiate(Snow, position, Quaternion.identity) as GameObject;

        IsActive = true;
    }

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        if (e.Team != player.Team || !IsActive)
            return;

        targetPlayer.MoveSpeedMultiplier /= MoveSpeedMultiplier;
        targetPlayer.DashSpeedMultiplier /= DashSpeedMultiplier;
        IsActive = false;

        snow.GetComponent<SnowFade>().FadeOut();
    }

    #endregion
}