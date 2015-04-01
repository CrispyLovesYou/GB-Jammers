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
    private const string PLAYER_LAYER = "Default";

    private const string CR_LOB_SCORE = "CR_LobScore";
    private const float LOB_CATCH_PERIOD = 0.1f;

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
        cPhotonView.RPC("RPC_Catch", PhotonTargets.All, _snapPosition);
    }

    public void Throw(Vector3 _snapPosition, Vector2 _throwVector)
    {
        cPhotonView.RPC("RPC_Throw", PhotonTargets.AllViaServer, _snapPosition, (Vector3)_throwVector);
    }

    public void Lob(Team _team, Vector2 _targetPosition, float _duration)
    {
        cPhotonView.RPC("RPC_Lob", PhotonTargets.AllViaServer, (int)_team, (Vector3)_targetPosition, _duration);
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
        Vector3 startPosition = cTransform.position;

        Crosshair.Instance.transform.position = _targetPosition;
        Crosshair.Instance.GetComponent<SpriteRenderer>().enabled = true;
        cCollider2D.enabled = false;

        float nFrames = _duration * 60;

        for (float i = 0; i < nFrames; i++)
        {
            float xDistance = (_targetPosition.x - startPosition.x) * (i / nFrames);
            float yDistance = (_targetPosition.y - startPosition.y) * (i / nFrames);
            float x = startPosition.x + xDistance;
            float y = startPosition.y + yDistance;

            cTransform.position = new Vector3(x, y, 0);

            float scale = 0.05f;
            float scaleX = 0;
            float scaleY = 0;

            if (i < nFrames / 2)
            {
                scaleX = cTransform.localScale.x + scale;
                scaleY = cTransform.localScale.y + scale;
            }
            else
            {
                scaleX = cTransform.localScale.x - scale;
                scaleY = cTransform.localScale.y - scale;
            }

            cTransform.localScale = new Vector3(scaleX, scaleY, 1);

            yield return 0;
        }

        Crosshair.Instance.GetComponent<SpriteRenderer>().enabled = false;
        cCollider2D.enabled = true;

        if (PhotonNetwork.isMasterClient)
            StartCoroutine(CR_LOB_SCORE, _team);
    }

    private IEnumerator CR_LobScore(Team _team)
    {
        float delay = (float)PhotonNetwork.GetPing() / 1000;

        yield return new WaitForSeconds(LOB_CATCH_PERIOD + (delay * 2));
        cCollider2D.enabled = false;
        MatchManager.Instance.ScorePoints(_team, MatchManager.Instance.LobPointValue);
    }

    /*
    private IEnumerator BezierMovement(Vector2 _startPosition, Vector2 _targetPosition, float _duration)
    {
        float initX = _startPosition.x;
        float initY = _startPosition.y;
        float targetX = _targetPosition.x;
        float targetY = _targetPosition.y;
        float bezierX = 0;
        float bezierY = Vector2.Distance(_startPosition, _targetPosition);

        for (float i = 0; i <= 1; i += 0.01f)
        {
            float x = (float)((1 - i) * (1 - i) * initX + 2 * (1 - i) * i * bezierX + i * i * targetX);
            float y = (float)((1 - i) * (1 - i) * initY + 2 * (1 - i) * i * bezierY + i * i * targetY);

            cTransform.position = new Vector3(x, y, 0);

            yield return new WaitForSeconds(_duration / (1 / 0.01f));
        }
    
     } */

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

    [RPC]
    private void RPC_Catch(Vector3 _snapPosition)
    {
        StopCoroutine(CR_LOB_SCORE);

        cRigidbody2D.velocity = velocity = Vector3.zero;
        cRigidbody2D.fixedAngle = true;

        cTransform.position = _snapPosition;
    }

    [RPC]
    private void RPC_Throw(Vector3 _snapPosition, Vector3 _throwVector)
    {
        cTransform.position = _snapPosition;
        velocity = _throwVector;
        cRigidbody2D.fixedAngle = false;
    }

    [RPC]
    private void RPC_Lob(int _team, Vector3 _targetPosition, float _duration)
    {
        StartCoroutine(CR_Lob((Team)_team, (Vector2)_targetPosition, _duration));
    }

    #endregion
}