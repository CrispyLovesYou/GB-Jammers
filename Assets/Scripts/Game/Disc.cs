using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu("Game/Disc")]
public class Disc : Singleton<Disc>
{
    #region Constants

    private const string WALL_TOP_TAG = "Wall_Top";
    private const string WALL_BOTTOM_TAG = "Wall_Bottom";
    private const string WALL_LEFT_TAG = "Wall_Left";
    private const string WALL_RIGHT_TAG = "Wall_Right";
    private const string PLAYER_LAYER = "Player";
    private const string GOAL_LAYER = "Goal";

    private const string CR_LOB_SCORE = "CR_LobScore";
    private const float LOB_CATCH_PERIOD = 0.1f;
    private const float LOB_SCALE_AMOUNT = 5.0f;

    #endregion

    #region Fields

    public bool HasKnockback = false;
    public float KnockbackPower = 0;

    private Transform cTransform;
    private Rigidbody2D cRigidbody2D;
    private PhotonView cPhotonView;
    private Collider2D cCollider2D;

    private Vector3 velocity = Vector3.zero;
    public Vector3 Velocity { get { return velocity; } }

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        cTransform = GetComponent<Transform>();
        cRigidbody2D = GetComponent<Rigidbody2D>();
        cPhotonView = GetComponent<PhotonView>();
        cCollider2D = GetComponent<Collider2D>();

        MatchManager.OnBeginResetAfterScore += MatchManager_OnBeginResetAfterScore;
        MatchManager.OnCompleteResetAfterScore += MatchManager_OnCompleteResetAfterScore;
        Controller_Player.OnPerfectThrow += Controller_Player_OnPerfectThrow;
    }

    private void Update()
    {
        cTransform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D _collider)
    {
        switch(_collider.tag)
        {
            case WALL_TOP_TAG:
                Bounce(Direction.DOWN);
                break;
                
            case WALL_BOTTOM_TAG:
                Bounce(Direction.UP);
                break;

            case WALL_LEFT_TAG:
            case WALL_RIGHT_TAG:
                cCollider2D.enabled = false;
                velocity = Vector3.zero;
                break;
        }
    }

    #endregion

    #region Methods

    public void Catch(Vector3 _snapPosition)
    {
        StopCoroutine(CR_LOB_SCORE);

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(PLAYER_LAYER), true);
        cRigidbody2D.velocity = velocity = Vector3.zero;
        cRigidbody2D.fixedAngle = true;

        cPhotonView.RPC("RPC_SetPosition", PhotonTargets.All, _snapPosition);
    }

    public void Throw(Vector3 _snapPosition, Vector2 _throwVector)
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(PLAYER_LAYER), false);
        cTransform.position = _snapPosition;
        velocity = _throwVector;
        cRigidbody2D.fixedAngle = false;
    }

    public void Lob(Team _team, Vector2 _targetPosition, float _duration)
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(PLAYER_LAYER), false);
        StartCoroutine(CR_Lob((Team)_team, (Vector2)_targetPosition, _duration));
    }

    public void SetPosition(Vector3 _position)
    {
        cPhotonView.RPC("RPC_SetPosition", PhotonTargets.AllViaServer, _position);
    }

    private void Bounce(Direction _direction)
    {
        switch (_direction)
        {
            case Direction.LEFT:
            case Direction.RIGHT:
                velocity = new Vector3(velocity.x * -1, velocity.y, 0);
                break;

            case Direction.UP:
            case Direction.DOWN:
                velocity = new Vector3(velocity.x, velocity.y * -1, 0);
                break;
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator CR_Lob(Team _team, Vector2 _targetPosition, float _duration)
    {
        Crosshair.Instance.transform.position = _targetPosition;
        Crosshair.Instance.GetComponent<SpriteRenderer>().enabled = true;
        cCollider2D.enabled = false;

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", (Vector3)_targetPosition,
            "time", _duration,
            "easetype", iTween.EaseType.linear));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", cTransform.localScale * LOB_SCALE_AMOUNT,
            "time", _duration / 2,
            "easetype", iTween.EaseType.easeOutQuad,
            "oncomplete",
                (Action<object>)(duration =>
                {
                    iTween.ScaleTo(gameObject, iTween.Hash(
                        "scale", cTransform.localScale / LOB_SCALE_AMOUNT,
                        "time", (float)duration / 2,
                        "easetype", iTween.EaseType.easeInQuad));
                })
            ));

        yield return new WaitForSeconds(_duration);

        Crosshair.Instance.GetComponent<SpriteRenderer>().enabled = false;
        cCollider2D.enabled = true;

        StartCoroutine(CR_LOB_SCORE, _team);
    }

    private IEnumerator CR_LobScore(Team _team)
    {
        float delay = (float)PhotonNetwork.GetPing() / 1000;
        yield return new WaitForSeconds(LOB_CATCH_PERIOD + (delay * 3));
        cCollider2D.enabled = false;
        MatchManager.Instance.ScorePoints(_team, MatchManager.Instance.LobPointValue);
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnBeginResetAfterScore(object sender, EventArgs e)
    {
        HasKnockback = false;
        KnockbackPower = 0;
    }

    private void MatchManager_OnCompleteResetAfterScore(object sender, EventArgs e)
    {
        cTransform.position = MatchManager.Instance.DiscSpawn;
        velocity = Vector3.zero;
        cCollider2D.enabled = true;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(PLAYER_LAYER), false);
    }

    private void Controller_Player_OnPerfectThrow(object sender, EventArgs e)
    {
        HasKnockback = true;
    }

    #endregion

    #region RPC

    [RPC]
    private void RPC_SetPosition(Vector3 _position)
    {
        cTransform.position = _position;
    }

    [RPC]
    private void RPC_SetVelocity(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    #endregion
}