using UnityEngine;
using System.Collections;

public class Controller_Player : Controller_Base
{
    #region Constants

    private const string DASH_COROUTINE = "HandleDashTimer";
    private const string DASH_RECOVERY_COROUTINE = "HandleDashRecoveryTimer";
    private const string THROW_RECOVERY_COROUTINE = "HandleThrowRecoveryTimer";
    private const string COURT_AREA_TAG = "Court_Area";

    #endregion

    #region Fields

    private Character character;
    public Character Character { get { return character; } }

    public Team Team = Team.UNASSIGNED;
    public Direction CurrentDirection = Direction.DOWN;

    public float MoveSpeed = 4.0f;
    public float DashSpeed = 15.0f;
    public float DashDuration = 0.15f;
    public float DashRecovery = 0.0f;
    public float ThrowPower = 20.0f;
    public float ThrowRecovery = 0.0f;
    public float ThrowAutoPowerThreshhold = 0.2f;
    public float LobDuration = 0.5f;

    [SerializeField]
    private bool hasDisc = false;
    public bool HasDisc { get { return hasDisc; } }

    [SerializeField]
    private bool isDashing = false;
    public bool IsDashing { get { return isDashing; } }

    private bool isDashRecovering = false;
    private bool isThrowRecovering = false;
    private bool isRecovering { get { return (isDashRecovering || isThrowRecovering); } }
    public bool IsRecovering { get { return isRecovering; } }

    private GameObject courtArea;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        courtArea = GameObject.FindGameObjectWithTag(COURT_AREA_TAG);
    }

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (hasDisc || Disc.Instance == null)
            return;

        if (_collider2D.gameObject == Disc.Instance.gameObject)
            Catch();
    }

    #endregion

    #region Methods

    public override void Move(Vector2 _inputVector)
    {
        if (isDashing || isRecovering || hasDisc)
            return;

        if (Mathf.Abs(_inputVector.x) > Mathf.Abs(_inputVector.y))
        {
            if (_inputVector.x > 0) CurrentDirection = Direction.RIGHT;
            else if (_inputVector.x < 0) CurrentDirection = Direction.LEFT;
        }
        else if (Mathf.Abs(_inputVector.y) > Mathf.Abs(_inputVector.x))
        {
            if (_inputVector.y > 0) CurrentDirection = Direction.UP;
            else if (_inputVector.y < 0) CurrentDirection = Direction.DOWN;
        }

        cRigidbody2D.velocity = _inputVector * MoveSpeed;
    }
    
    private void Catch()
    {
        hasDisc = true;
        Stop();

        isDashing = false;
        StopCoroutine(DASH_COROUTINE);
        isDashRecovering = false;
        StopCoroutine(DASH_RECOVERY_COROUTINE);
        isThrowRecovering = false;
        StopCoroutine(THROW_RECOVERY_COROUTINE);

        float offsetX = 0;
        float buffer = 0.1f;

        switch (Team)
        {
            case Team.LEFT:
                offsetX = Disc.Instance.transform.localScale.x + buffer;
                break;

            case Team.RIGHT:
                offsetX = -Disc.Instance.transform.localScale.x - buffer;
                break;
        }

        Disc.Instance.Catch(cTransform.position, offsetX);
    }

    public override void Action(Vector2 _inputVector)
    {
        if (isDashing || isRecovering)
            return;

        if (hasDisc)
            Throw(_inputVector);
        else
            Dash(_inputVector.normalized);
    }

    public override void Lob(Vector2 _inputVector)
    {
        if (!hasDisc)
            return;

        Vector2 targetPosition = Vector2.zero;
        Transform courtTransform = courtArea.transform;

        // First, we have to define the targetable area for lobs (opponent's side of court, minus disc width / 2)
        float xMin, yMin, xMax, yMax, xDiff, yDiff, xLength, yLength, targetX, targetY;
        float courtCenterX = courtArea.transform.position.x;
        float courtCenterY = courtArea.transform.position.y;
        float halfCourtWidth = courtTransform.localScale.x / 2;
        float halfCourtHeight = courtTransform.localScale.y / 2;
        float halfDiscWidth = Disc.Instance.transform.localScale.x / 2;
        float halfDiscHeight = Disc.Instance.transform.localScale.y / 2;
        float buffer = 0.1f;  // keeps the Disc from accidentally colliding with goal zone

        xMin = yMin = xMax = yMax = xDiff = yDiff = xLength = yLength = targetX = targetY = 0;

        switch (Team)
        {
            case Team.LEFT:
                xMin = courtCenterX + halfDiscWidth + buffer;
                xMax = courtCenterX + halfCourtWidth - halfDiscWidth - buffer;
                break;

            case Team.RIGHT:
                xMin = courtCenterX - halfCourtWidth + halfDiscWidth + buffer;
                xMax = courtCenterX - halfDiscWidth - buffer;
                break;
        }

        yMin = courtCenterY - halfCourtHeight + halfDiscHeight + buffer;
        yMax = courtCenterY + halfCourtHeight - halfDiscHeight - buffer;

        xDiff = xMax - xMin;
        yDiff = yMax - yMin;

        xLength = (xDiff / 2) * _inputVector.x;
        yLength = (yDiff / 2) * _inputVector.y;

        targetX = xMin + (xDiff / 2) + xLength;
        targetY = yMin + (yDiff / 2) + yLength;

        targetPosition = new Vector2(targetX, targetY);

        hasDisc = false;
        Disc.Instance.Lob(targetPosition, LobDuration);
        isThrowRecovering = true;
        StartCoroutine(THROW_RECOVERY_COROUTINE);
    }

    private void Throw(Vector2 _inputVector)
    {
        Vector2 throwVector = _inputVector * ThrowPower;

        switch (Team)
        {
            case Team.LEFT:
                if (Mathf.Abs(_inputVector.x) < ThrowAutoPowerThreshhold)  // If the horizontal input of the aim wasn't high enough, throw a max power shot
                    throwVector.x = ThrowPower;

                if (_inputVector.x < 0)  // If the directionVector is in the opposite direction, do a half-power throw
                    throwVector.x = ThrowPower / 2;
                break;

            case Team.RIGHT:
                if (Mathf.Abs(_inputVector.x) < ThrowAutoPowerThreshhold)
                    throwVector.x = -ThrowPower;

                if (_inputVector.x > 0)
                    throwVector.x = -(ThrowPower / 2);
                break;
        }

        hasDisc = false;
        Disc.Instance.Throw((Vector3)throwVector);
        isThrowRecovering = true;
        StartCoroutine(THROW_RECOVERY_COROUTINE);
    }

    private void Dash(Vector2 _directionVector)
    {
        isDashing = true;
        cRigidbody2D.velocity = _directionVector * DashSpeed;
        StartCoroutine(DASH_COROUTINE);
    }

    #endregion

    #region Coroutines

    private IEnumerator HandleDashTimer()
    {
        yield return new WaitForSeconds(DashDuration);
        Stop();
        isDashing = false;
        isDashRecovering = true;
        StartCoroutine(DASH_RECOVERY_COROUTINE);
    }

    private IEnumerator HandleDashRecoveryTimer()
    {
        yield return new WaitForSeconds(DashRecovery);
        isDashRecovering = false;
    }

    private IEnumerator HandleThrowRecoveryTimer()
    {
        yield return new WaitForSeconds(ThrowRecovery);
        isThrowRecovering = false;
    }

    #endregion

    #region Setters

    public void SetCharacter(Character _character)
    {
        cPhotonView.RPC("RPC_SetCharacter", PhotonTargets.AllBufferedViaServer, _character.Serialize());
    }

    #endregion

    #region Callbacks

    [RPC]
    private void RPC_SetCharacter(string _characterXmlData)
    {
        character = Character.Deserialize(_characterXmlData);
    }

    #endregion
}
