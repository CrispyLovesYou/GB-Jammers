using UnityEngine;
using System;
using System.Collections;
using System.Linq;

[AddComponentMenu("Controllers/Controller (Player)")]
public class Controller_Player : MonoBehaviour
{
    #region Constants

    public const float KNOCKBACK_NORMALIZER = 0.1f;
    public const float MIN_KNOCKBACK_DURATION = 0.2f;
    public const float MIN_MOVE_SPEED = 1.0f;
    public const float MIN_DASH_SPEED = 3.0f;
    public const float THROW_AFTER_IDLE_DURATION = 3.0f;

    private const string CR_DASH = "CR_Dash";
    private const string CR_CHARGE = "CR_Charge";
    private const string CR_THROW_RECOVERY = "CR_ThrowRecovery";
    private const string CR_KNOCKBACK = "CR_Knockback";
    private const string CR_THROW_AFTER_IDLE = "CR_ThrowAfterIdle";

    private const string COURT_AREA_TAG = "Court_Area";
    private const string BOOK_TAG = "Book";

    #endregion

    #region Events

    private static EventHandler<EventArgs> onSpeedBuff;
    public static event EventHandler<EventArgs> OnSpeedBuff
    {
        add
        {
            if (onSpeedBuff == null || !onSpeedBuff.GetInvocationList().Contains(value))
                onSpeedBuff += value;
        }
        remove { onSpeedBuff -= value; }
    }
    public void CallOnSpeedBuff()
    {
        if (onSpeedBuff != null)
            onSpeedBuff(this, EventArgs.Empty);
    }

    private static EventHandler<EventArgs> onSpeedDebuff;
    public static event EventHandler<EventArgs> OnSpeedDebuff
    {
        add
        {
            if (onSpeedDebuff == null || !onSpeedDebuff.GetInvocationList().Contains(value))
                onSpeedDebuff += value;
        }
        remove { onSpeedDebuff -= value; }
    }
    public void CallOnSpeedDebuff()
    {
        if (onSpeedDebuff != null)
            onSpeedDebuff(this, EventArgs.Empty);
    }

    private static EventHandler<EventArgs> onPowerBuff;
    public static event EventHandler<EventArgs> OnPowerBuff
    {
        add
        {
            if (onPowerBuff == null || !onPowerBuff.GetInvocationList().Contains(value))
                onPowerBuff += value;
        }
        remove { onPowerBuff -= value; }
    }
    public void CallOnPowerBuff()
    {
        if (onPowerBuff != null)
            onPowerBuff(this, EventArgs.Empty);
    }

    private static EventHandler<EventArgs> onPowerDebuff;
    public static event EventHandler<EventArgs> OnPowerDebuff
    {
        add
        {
            if (onPowerDebuff == null || !onPowerDebuff.GetInvocationList().Contains(value))
                onPowerDebuff += value;
        }
        remove { onPowerDebuff -= value; }
    }
    public void CallOnPowerDebuff()
    {
        if (onPowerDebuff != null)
            onPowerDebuff(this, EventArgs.Empty);
    }

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

    private static EventHandler<EventArgs> onGreatThrow;
    public static event EventHandler<EventArgs> OnGreatThrow
    {
        add
        {
            if (onGreatThrow == null || !onGreatThrow.GetInvocationList().Contains(value))
                onGreatThrow += value;
        }
        remove { onGreatThrow -= value; }
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

    private static EventHandler<EXEventArgs> onEX;
    public static event EventHandler<EXEventArgs> OnEX
    {
        add
        {
            if (onEX == null || !onEX.GetInvocationList().Contains(value))
                onEX += value;
        }
        remove { onEX -= value; }
    }

    private static EventHandler<SuperEventArgs> onSuper;
    public static event EventHandler<SuperEventArgs> OnSuper
    {
        add
        {
            if (onSuper == null || !onSuper.GetInvocationList().Contains(value))
                onSuper += value;
        }
        remove { onSuper -= value; }
    }

    private static EventHandler<MeterChangeEventArgs> onMeterChange;
    public static event EventHandler<MeterChangeEventArgs> OnMeterChange
    {
        add
        {
            if (onMeterChange == null || !onMeterChange.GetInvocationList().Contains(value))
                onMeterChange += value;
        }
        remove { onMeterChange -= value; }
    }

    private static EventHandler<EventArgs> onBookStun;
    public static event EventHandler<EventArgs> OnBookStun
    {
        add
        {
            if (onBookStun == null || !onBookStun.GetInvocationList().Contains(value))
                onBookStun += value;
        }
        remove { onBookStun -= value; }
    }

    #endregion

    #region Fields

    public string ID = "";
    public string Name = "";
    public string RealName = "";
    public float MoveSpeed = 4.0f;
    public float DashSpeed = 15.0f;
    public float DashDuration = 0.15f;
    public float ThrowPower = 20.0f;
    public float ThrowRecovery = 0.3f;
    public float Knockback = 3.0f;
    public float Stability = 2.5f;
    public float LobDuration = 0.5f;

    private int meter = 0;
    public int Meter
    {
        get { return meter; }
        set
        {
            int clampValue = Mathf.Clamp(value, 0, 100);

            if (onMeterChange != null)
                onMeterChange(this, new MeterChangeEventArgs(Team, clampValue - meter, clampValue));

            meter = clampValue;
        }
    }
    public int MeterGainOnGreat = 10;
    public int MeterGainOnPerfect = 33;

    public float MoveSpeedMod = 0;
    public float MoveSpeedMultiplier = 1.0f;
    public float DashSpeedMod = 0;
    public float DashSpeedMultiplier = 1.0f;
    public float ThrowPowerMod = 0;
    public float ThrowPowerMultiplier = 1.0f;
    public float KnockbackMod = 0;
    public float KnockbackMultiplier = 1.0f;
    public float StabilityMod = 0;
    public float StabilityMultiplier = 1.0f;
    public float LobDurationMod = 0;
    public float LobDurationMultiplier = 1.0f;

    public Team Team = Team.UNASSIGNED;
    public PlayerState State = PlayerState.WALK;
    public Direction CurrentDirection = Direction.DOWN;
    public float GreatThrowThreshhold = 80;
    public float PerfectThrowThreshhold = 90;
    public int MeterForEX = 33;
    public int MeterForSuper = 99;

    public AudioClip SFXError;

    public static bool isPingCompensating = false;

    private Transform cTransform;
    private Rigidbody2D cRigidbody2D;
    private Collider2D cCollider2D;
    private PhotonView cPhotonView;
    private Animator cAnimator;

    private GameObject courtArea;

    private float throwCharge = 0;
    public float ThrowCharge { get { return throwCharge; } }

    private float maxChargeDuration = 1.0f;
    private Vector2 throwDirection = Vector2.zero;
    private float throwDirectionThreshhold = 0.7f;
    private Vector2 throwVector = Vector2.zero;
    private Vector2 knockbackVector = Vector2.zero;
    private Vector2 lobTarget = Vector2.zero;

    private bool walking = false;  // for bandwidth purposes

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cTransform = GetComponent<Transform>();
        cRigidbody2D = GetComponent<Rigidbody2D>();
        cCollider2D = GetComponent<Collider2D>();
        cPhotonView = GetComponent<PhotonView>();
        cAnimator = GetComponent<Animator>();

        courtArea = GameObject.FindGameObjectWithTag(COURT_AREA_TAG);

        MatchManager.OnBeginResetAfterScore += MatchManager_OnBeginResetAfterScore;
        MatchManager.OnCompleteResetAfterScore += MatchManager_OnCompleteResetAfterScore;
    }

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (Disc.Instance == null)
            return;

        if (State == PlayerState.AIM ||
            State == PlayerState.RESET)
            return;

        if (_collider2D.tag == Disc.Instance.tag)
        {
            if (cPhotonView.isMine)
                cPhotonView.RPC("RPC_Catch", PhotonTargets.All);
            else
            {
                isPingCompensating = true;
                StartCoroutine(CR_PingCompensation());
            }
        }
        else if (_collider2D.tag == BOOK_TAG)
        {
            if (cPhotonView.isMine)
            {
                Book book = _collider2D.gameObject.GetSafeComponent<Book>();
                cPhotonView.RPC("RPC_BookStun", PhotonTargets.AllViaServer, book.ID);
            }
        }
    }

    #endregion

    #region Methods

    public void SetData(Team _team, CharacterID _id)
    {
        cPhotonView.RPC("RPC_SetData", PhotonTargets.AllBufferedViaServer, (int)_team, (int)_id);
    }

    public void Stop()
    {
        cRigidbody2D.velocity = Vector2.zero;
        walking = false;

        if (State == PlayerState.WALK)
            cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.IDLE);
    }

    public void Move(Vector2 _inputVector)
    {
        if (State != PlayerState.IDLE && State != PlayerState.WALK)
            return;

        if (!walking)
            cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.WALK);

        walking = true;

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

        float moveSpeed = (MoveSpeed + MoveSpeedMod) * MoveSpeedMultiplier;

        if (moveSpeed < MIN_MOVE_SPEED)
            moveSpeed = MIN_MOVE_SPEED;

        cRigidbody2D.velocity = _inputVector * moveSpeed;
    }

    public void Action(Vector2 _inputVector)
    {
        switch (State)
        {
            case PlayerState.WALK:
                StartCoroutine(CR_DASH, _inputVector.normalized);
                break;

            case PlayerState.AIM:
                throwDirection = _inputVector;
                StartCoroutine(CR_CHARGE);
                break;
        }
    }

    public void ReleaseAction(Vector2 _inputVector)
    {
        if (State != PlayerState.CHARGE)
            return;

        throwDirection = _inputVector;
        StopCoroutine(CR_CHARGE);
        iTween.StopByName("ChargeUp");
        iTween.StopByName("ChargeDown");
        cPhotonView.RPC("RPC_Throw", PhotonTargets.AllViaServer, (Vector3)throwDirection, throwCharge);
    }

    public void SpecialThrow(Vector2 _inputVector, bool _hasKnockback)
    {
        throwDirection = _inputVector;
        cPhotonView.RPC("RPC_SpecialThrow", PhotonTargets.AllViaServer, (Vector3)throwDirection, _hasKnockback);
    }

    public void ThrowAfterAnimation()
    {
        Disc.Instance.Throw(cTransform.position + (Vector3.right * GetDiscOffset()), (Vector3)throwVector);

        StartCoroutine(CR_THROW_RECOVERY);
    }

    public void LobAfterAnimation()
    {
        float lobDuration = (LobDuration + LobDurationMod) * LobDurationMultiplier;
        Disc.Instance.Lob(Team, lobTarget, lobDuration);
        StartCoroutine(CR_THROW_RECOVERY);
    }

    public void Lob(Vector2 _inputVector)
    {
        if (State != PlayerState.AIM)
            return;

        cPhotonView.RPC("RPC_Lob", PhotonTargets.AllViaServer, (Vector3)_inputVector);
    }

    public void EX(Vector2 _inputVector)
    {
        if (State != PlayerState.AIM)
            return;

        if (Meter < MeterForEX)
        {
            AudioSource.PlayClipAtPoint(SFXError, Vector3.zero);
            return;
        }

        cPhotonView.RPC("RPC_EX", PhotonTargets.AllViaServer, (Vector3)_inputVector);
    }

    public void Super(Vector2 _inputVector)
    {
        if (State != PlayerState.AIM)
            return;

        if (Meter < MeterForSuper)
        {
            AudioSource.PlayClipAtPoint(SFXError, Vector3.zero);
            return;
        }

        cPhotonView.RPC("RPC_Super", PhotonTargets.AllViaServer, (Vector3)_inputVector);
    }

    public Vector2 GetLobTargetPosition(Vector2 _inputVector)
    {
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

        return targetPosition;
    }

    private float GetDiscOffset()
    {
        float discOffset = 0;
        float buffer = 0.1f;

        switch (Team)
        {
            case Team.LEFT:
                discOffset = Disc.Instance.transform.localScale.x + buffer;
                break;

            case Team.RIGHT:
                discOffset = -Disc.Instance.transform.localScale.x - buffer;
                break;
        }

        return discOffset;
    }

    private void GreatThrow()
    {
        if (onGreatThrow != null)
            onGreatThrow(this, EventArgs.Empty);

        Meter += MeterGainOnGreat;
    }

    private void PerfectThrow()
    {
        if (onPerfectThrow != null)
            onPerfectThrow(this, EventArgs.Empty);

        Meter += MeterGainOnPerfect;    
        Disc.Instance.HasKnockback = true;
        float knockback = (Knockback + KnockbackMod) * KnockbackMultiplier;
        Disc.Instance.KnockbackPower = knockback;
    }

    #endregion

    #region Coroutines

    private IEnumerator CR_Charge()
    {
        cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.CHARGE);
        throwCharge = 0;

        iTween.ValueTo(gameObject, iTween.Hash(
            "name", "ChargeUp",
            "from", 0,
            "to", 100,
            "time", maxChargeDuration / 2,
            "onupdate",
                (Action<object>)(_value =>
                {
                    throwCharge = (float)_value;
                }),
            "oncomplete",
                (Action<object>)(param => {
                    iTween.ValueTo(gameObject, iTween.Hash(
                        "name", "ChargeDown",
                        "from", 100,
                        "to", 0,
                        "time", maxChargeDuration / 2,
                        "onupdate",
                            (Action<object>)(_value =>
                            {
                                throwCharge = (float)_value;
                            })
                    ));
                })
            ));

        yield return new WaitForSeconds(maxChargeDuration);

        cPhotonView.RPC("RPC_Throw", PhotonTargets.AllViaServer, (Vector3)throwDirection, throwCharge);
    }

    private IEnumerator CR_Dash(Vector2 _directionVector)
    {
        cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.DASH);

        float dashSpeed = (DashSpeed + DashSpeedMod) * DashSpeedMultiplier;

        if (dashSpeed < MIN_DASH_SPEED)
            dashSpeed = MIN_DASH_SPEED;

        cRigidbody2D.velocity = _directionVector * dashSpeed;
        yield return new WaitForSeconds(DashDuration);
        Stop();
        cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.IDLE);
    }

    private IEnumerator CR_ThrowAfterIdle()
    {
        yield return new WaitForSeconds(THROW_AFTER_IDLE_DURATION);

        if (State == PlayerState.AIM)
            cPhotonView.RPC("RPC_Throw", PhotonTargets.AllViaServer, Vector3.right, (float)0);
    }

    private IEnumerator CR_ThrowRecovery()
    {
        cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.RECOVERY);
        yield return new WaitForSeconds(ThrowRecovery);
        State = PlayerState.IDLE;
        walking = false;
    }

    private IEnumerator CR_Knockback()
    {
        State = PlayerState.KNOCKBACK;
        cRigidbody2D.velocity = knockbackVector * KNOCKBACK_NORMALIZER;

        float stability = (Stability + StabilityMod) * StabilityMultiplier;
        float duration = (Disc.Instance.KnockbackPower - stability) / 2;

        if (duration < MIN_KNOCKBACK_DURATION)
            duration = MIN_KNOCKBACK_DURATION;

        yield return new WaitForSeconds(duration);
        Stop();
        cPhotonView.RPC("RPC_RemoveKnockback", PhotonTargets.All);
        Disc.Instance.SetPosition(cTransform.position);
        State = PlayerState.AIM;
    }

    private IEnumerator CR_BookStun(Book _book)
    {
        State = PlayerState.STUN;
        Stop();
        cCollider2D.enabled = false;
        _book.DestroySelf();

        yield return new WaitForSeconds(Book.StunDuration);

        cCollider2D.enabled = true;
        State = PlayerState.IDLE;
    }

    private IEnumerator CR_PingCompensation()
    {
        float delay = (float)PhotonNetwork.GetPing() / 1000;
        yield return new WaitForSeconds(delay * 3);
        isPingCompensating = false;
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnBeginResetAfterScore(object sender, EventArgs e)
    {
        if (!cPhotonView.isMine)
            return;

        cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.RESET);

        Stop();
        Vector3 targetPos = Vector3.zero;
        float duration = 2.0f;

        switch (Team)
        {
            case global::Team.LEFT: targetPos = MatchManager.Instance.TeamLeftSpawn; break;
            case global::Team.RIGHT: targetPos = MatchManager.Instance.TeamRightSpawn; break;
        }

        iTween.MoveTo(this.gameObject, targetPos, duration);
    }

    private void MatchManager_OnCompleteResetAfterScore(object sender, EventArgs e)
    {
        cPhotonView.RPC("RPC_SetState", PhotonTargets.All, (int)PlayerState.IDLE);
    }

    #endregion

    #region RPC

    [RPC]
    private void RPC_SetData(int _team, int _id)
    {
        Team = (Team)_team;

        if (Team == Team.RIGHT)  // Flip the player on the right
            cTransform.localScale = new Vector3(-cTransform.localScale.x, cTransform.localScale.y, 1);

        if (Globals.CharacterDict.Count == 0)
            return;

        Character ch = Globals.CharacterDict[(CharacterID)_id];

        ID = ch.ID;
        Name = ch.Name;
        RealName = ch.RealName;
        MoveSpeed = ch.MoveSpeed;
        DashSpeed = ch.DashSpeed;
        DashDuration = ch.DashDuration;
        ThrowPower = ch.ThrowPower;
        Knockback = ch.ThrowKnockback;
        Stability = ch.Stability;
        LobDuration = ch.LobDuration;
    }

    [RPC]
    private void RPC_SetState(int _state)
    {
        State = (PlayerState)_state;
        cAnimator.SetInteger("state", _state);
    }

    [RPC]
    private void RPC_Throw(Vector3 _throwDirection, float _throwCharge)
    {
        StopCoroutine(CR_THROW_AFTER_IDLE);

        throwDirection = _throwDirection;
        throwCharge = _throwCharge;

        if (throwDirection.x < Mathf.Abs(throwDirectionThreshhold))
            switch (Team)
            {
                case Team.LEFT: throwDirection.x = throwDirectionThreshhold; break;
                case Team.RIGHT: throwDirection.x = -throwDirectionThreshhold; break;
            }

        // When throwCharge = 0, throwVector.x = throwPower / 2; throwCharge = 100, throwVector.x = throwPower
        float throwPower = (ThrowPower + ThrowPowerMod) * ThrowPowerMultiplier;
        throwVector = throwDirection * ((((throwPower / 2) * throwCharge) / 100) + (throwPower / 2));

        switch (Team)
        {
            case Team.LEFT: throwVector.x = Mathf.Abs(throwVector.x); break;
            case Team.RIGHT: throwVector.x = -Mathf.Abs(throwVector.x); break;
        }

        if (throwCharge == 0)
            throwVector.y = 0;

        if (throwCharge >= GreatThrowThreshhold &&
            throwCharge < PerfectThrowThreshhold)
        {
            GreatThrow();
        }
        else if (throwCharge >= PerfectThrowThreshhold)
        {
            PerfectThrow();
        }

        throwCharge = 0;

        if (onThrow != null)
            onThrow(this, EventArgs.Empty);

        cAnimator.SetTrigger("throw");
    }

    [RPC]
    private void RPC_SpecialThrow(Vector3 _throwDirection, bool _hasKnockback)
    {
        StopCoroutine(CR_THROW_AFTER_IDLE);

        throwDirection = _throwDirection;
        throwCharge = 100;

        if (throwDirection.x < Mathf.Abs(throwDirectionThreshhold))
            switch (Team)
            {
                case Team.LEFT: throwDirection.x = throwDirectionThreshhold; break;
                case Team.RIGHT: throwDirection.x = -throwDirectionThreshhold; break;
            }

        // When throwCharge = 0, throwVector.x = throwPower / 2; throwCharge = 100, throwVector.x = throwPower
        float throwPower = (ThrowPower + ThrowPowerMod) * ThrowPowerMultiplier;
        throwVector = throwDirection * ((((throwPower / 2) * throwCharge) / 100) + (throwPower / 2));

        switch (Team)
        {
            case Team.LEFT: throwVector.x = Mathf.Abs(throwVector.x); break;
            case Team.RIGHT: throwVector.x = -Mathf.Abs(throwVector.x); break;
        }

        if (throwCharge == 0)
            throwVector.y = 0;

        if (_hasKnockback)
        {
            Disc.Instance.HasKnockback = true;
            float knockback = (Knockback + KnockbackMod) * KnockbackMultiplier;
            Disc.Instance.KnockbackPower = knockback;
        }

        throwCharge = 0;

        if (onThrow != null)
            onThrow(this, EventArgs.Empty);

        cAnimator.SetTrigger("throw");
    }

    [RPC]
    private void RPC_Catch()
    {
        State = PlayerState.AIM;
        cAnimator.SetInteger("state", (int)State);
        Stop();

        StopCoroutine(CR_DASH);
        StopCoroutine(CR_THROW_RECOVERY);

        knockbackVector = Disc.Instance.Velocity;

        Disc.Instance.Catch(cTransform.position + (Vector3.right * GetDiscOffset()));

        if (Disc.Instance.HasKnockback)
            StartCoroutine(CR_KNOCKBACK);

        if (onCatch != null)
            onCatch(this, EventArgs.Empty);

        StartCoroutine(CR_THROW_AFTER_IDLE);

        if (!MatchManager.Instance.isInitialCatchComplete)
            MatchManager.Instance.isInitialCatchComplete = true;
    }

    [RPC]
    private void RPC_Lob(Vector3 _inputVector)
    {
        lobTarget = GetLobTargetPosition(_inputVector);

        cAnimator.SetTrigger("lob");

        if (onLob != null)
            onLob(this, EventArgs.Empty);
    }

    [RPC]
    private void RPC_EX(Vector3 _inputVector)
    {
        if (onEX != null)
            onEX(this, new EXEventArgs(_inputVector));

        Meter -= MeterForEX;

        StopCoroutine(CR_THROW_AFTER_IDLE);

        if (State == PlayerState.AIM)
            StartCoroutine(CR_THROW_AFTER_IDLE);
    }

    [RPC]
    private void RPC_Super(Vector3 _inputVector)
    {
        if (onSuper != null)
            onSuper(this, new SuperEventArgs(_inputVector));

        if (MeterForSuper == 99 && Meter == 100)
            Meter = 0;
        else
            Meter -= MeterForSuper;

        StopCoroutine(CR_THROW_AFTER_IDLE);

        if (State == PlayerState.AIM)
            StartCoroutine(CR_THROW_AFTER_IDLE);
    }

    [RPC]
    private void RPC_BookStun(int _id)
    {
        if (onBookStun != null)
            onBookStun(this, EventArgs.Empty);

        GameObject[] gObjList = GameObject.FindGameObjectsWithTag(BOOK_TAG);
        GameObject book = null;

        foreach (GameObject gObj in gObjList)
        {
            if (gObj.GetSafeComponent<Book>().ID == _id)
                book = gObj;
        }

        if (book != null)
            StartCoroutine(CR_BookStun(book.GetSafeComponent<Book>()));
    }

    [RPC]
    private void RPC_RemoveKnockback()
    {
        Disc.Instance.HasKnockback = false;
        Disc.Instance.KnockbackPower = 0;
    }

    #endregion
}
