using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class Controller_Player : MonoBehaviour
{
    #region Constants

    private const string DASH_COROUTINE = "HandleDashTimer";
    private const string DASH_RECOVERY_COROUTINE = "HandleDashRecoveryTimer";
    private const string THROW_RECOVERY_COROUTINE = "HandleThrowRecoveryTimer";
    private const string COURT_AREA_TAG = "Court_Area";

    #endregion

    #region Events

    private static EventHandler<EventArgs> onCatch;
    public static event EventHandler<EventArgs> OnCatch
    {
        add
        {
            if (onCatch == null || !onCatch.GetInvocationList().Contains(value))
                onCatch += value;
        }
        remove { onCatch -= value; }
    }

    private static EventHandler<EventArgs> onThrow;
    public static event EventHandler<EventArgs> OnThrow
    {
        add
        {
            if (onThrow == null || !onThrow.GetInvocationList().Contains(value))
                onThrow += value;
        }
        remove { onThrow -= value; }
    }

    private static EventHandler<EventArgs> onGoodThrow;
    public static event EventHandler<EventArgs> OnGoodThrow
    {
        add
        {
            if (onGoodThrow == null || !onGoodThrow.GetInvocationList().Contains(value))
                onGoodThrow += value;
        }
        remove { onGoodThrow -= value; }
    }

    private static EventHandler<EventArgs> onPerfectThrow;
    public static event EventHandler<EventArgs> OnPerfectThrow
    {
        add
        {
            if (onPerfectThrow == null || !onPerfectThrow.GetInvocationList().Contains(value))
                onPerfectThrow += value;
        }
        remove { onPerfectThrow -= value; }
    }

    private static EventHandler<EventArgs> onLob;
    public static event EventHandler<EventArgs> OnLob
    {
        add
        {
            if (onLob == null || !onLob.GetInvocationList().Contains(value))
                onLob += value;
        }
        remove { onLob -= value; }
    }

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
    public float ThrowKnockback = 3.0f;
    public float Stability = 2.5f;
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

    private Transform cTransform;
    private Rigidbody2D cRigidbody2D;
    private PhotonView cPhotonView;
    private GameObject courtArea;

    private float throwCharge = 0;
    public float ThrowCharge { get { return throwCharge; } }

    private bool isThrowing = false;
    private float maxChargeDuration = 0.5f;
    private Vector2 throwDirection = Vector2.zero;
    private float throwDirectionThreshhold = 0.7f;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cTransform = GetComponent<Transform>();
        cRigidbody2D = GetComponent<Rigidbody2D>();
        cPhotonView = GetComponent<PhotonView>();
        courtArea = GameObject.FindGameObjectWithTag(COURT_AREA_TAG);
    }

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (!cPhotonView.isMine || hasDisc || Disc.Instance == null)
            return;

        if (_collider2D.gameObject == Disc.Instance.gameObject)
            Catch();
    }

    #endregion

    #region Methods

    public void Stop()
    {
        cRigidbody2D.velocity = Vector2.zero;
    }

    public void Move(Vector2 _inputVector)
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

    public void Action(Vector2 _inputVector)
    {
        if (isDashing || isRecovering)
            return;

        if (hasDisc)
        {
            throwDirection = _inputVector;
            StartCoroutine("StartThrow");
        }
        else
            Dash(_inputVector.normalized);
    }

    public void ReleaseAction(Vector2 _inputVector)
    {
        if (!isThrowing)
            return;

        throwDirection = _inputVector;
        StopCoroutine("StartThrow");
        Throw();
    }

    public void Lob(Vector2 _inputVector)
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

        if (onLob != null)
            onLob(this, EventArgs.Empty);
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

        if (onCatch != null)
            onCatch(this, EventArgs.Empty);
    }

    private void Throw()
    {
        if (throwDirection.x < Mathf.Abs(throwDirectionThreshhold))
            switch (Team)
            {
                case Team.LEFT: throwDirection.x = throwDirectionThreshhold; break;
                case Team.RIGHT: throwDirection.x = -throwDirectionThreshhold; break;
            }

        // When throwCharge = 0, throwVector.x = ThrowPower / 2; throwCharge = 100, throwVector.x = ThrowPower
        Vector2 throwVector = throwDirection * ((((ThrowPower / 2) * throwCharge) / 100) + (ThrowPower / 2));

        switch (Team)
        {
            case Team.LEFT: throwVector.x = Mathf.Abs(throwVector.x); break;
            case Team.RIGHT: throwVector.x = -Mathf.Abs(throwVector.x); break;
        }

        if (throwCharge == 0)
            throwVector.y = 0;

        hasDisc = false;
        isThrowing = false;
        throwCharge = 0;
        Disc.Instance.Throw((Vector3)throwVector);
        isThrowRecovering = true;
        StartCoroutine(THROW_RECOVERY_COROUTINE);

        if (onThrow != null)
            onThrow(this, EventArgs.Empty);
    }

    private void Dash(Vector2 _directionVector)
    {
        isDashing = true;
        cRigidbody2D.velocity = _directionVector * DashSpeed;
        StartCoroutine(DASH_COROUTINE);
    }

    #endregion

    #region Coroutines

    private IEnumerator StartThrow()
    {
        isThrowing = true;
        throwCharge = 0;

        int increment = 5;

        for (int i = 1; i <= 100; i += increment)  // Charge raising up to 100
        {
            throwCharge = i;
            yield return new WaitForSeconds(maxChargeDuration / (100 / increment));
        }

        for (int i = 100; i >= 0; i -= increment)  // Charge decaying down to 0
        {
            throwCharge = i;
            yield return new WaitForSeconds(maxChargeDuration / (100 / increment));
        }

        Throw();
    }

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
