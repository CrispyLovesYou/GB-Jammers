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

    private const string CR_DASH = "CR_Dash";
    private const string CR_CHARGE = "CR_Charge";
    private const string CR_THROW_RECOVERY = "CR_ThrowRecovery";
    private const string CR_KNOCKBACK = "CR_Knockback";

    private const string COURT_AREA_TAG = "Court_Area";
    private const string GOAL_WALL_LAYER = "Goal Walls";

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
    public PlayerState State = PlayerState.NORMAL;
    public Direction CurrentDirection = Direction.DOWN;
    public float GreatThrowThreshhold = 80;
    public float PerfectThrowThreshhold = 90;
    public float MeterForEX = 33;

    public AudioClip SFXError;

    public static bool isPingCompensating = false;

    private Transform cTransform;
    private Rigidbody2D cRigidbody2D;
    private PhotonView cPhotonView;

    private GameObject courtArea;

    private float throwCharge = 0;
    public float ThrowCharge { get { return throwCharge; } }

    private float maxChargeDuration = 1.0f;
    private Vector2 throwDirection = Vector2.zero;
    private float throwDirectionThreshhold = 0.7f;
    private Vector2 knockbackVector = Vector2.zero;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cTransform = GetComponent<Transform>();
        cRigidbody2D = GetComponent<Rigidbody2D>();
        cPhotonView = GetComponent<PhotonView>();

        courtArea = GameObject.FindGameObjectWithTag(COURT_AREA_TAG);

        MatchManager.OnBeginResetAfterScore += MatchManager_OnBeginResetAfterScore;
        MatchManager.OnCompleteResetAfterScore += MatchManager_OnCompleteResetAfterScore;
    }

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (Disc.Instance == null ||
            State == PlayerState.AIM ||
            State == PlayerState.RESET)
            return;

        if (_collider2D.tag == Disc.Instance.tag)
        {
            if (cPhotonView.isMine)
                Catch();
            else
            {
                isPingCompensating = true;
                StartCoroutine(CR_PingCompensation());
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
    }

    public void Move(Vector2 _inputVector)
    {
        if (State != PlayerState.NORMAL)
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

        float moveSpeed = (MoveSpeed + MoveSpeedMod) * MoveSpeedMultiplier;

        if (moveSpeed < MIN_MOVE_SPEED)
            moveSpeed = MIN_MOVE_SPEED;

        cRigidbody2D.velocity = _inputVector * moveSpeed;
    }

    public void Action(Vector2 _inputVector)
    {
        switch (State)
        {
            case PlayerState.NORMAL:
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
        Throw();
    }

    public void Lob(Vector2 _inputVector)
    {
        if (State != PlayerState.AIM)
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

        float lobDuration = (LobDuration + LobDurationMod) * LobDurationMultiplier;
        Disc.Instance.Lob(Team, targetPosition, lobDuration);
        StartCoroutine(CR_THROW_RECOVERY);

        if (onLob != null)
            onLob(this, EventArgs.Empty);
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

    private void Catch()
    {
        State = PlayerState.AIM;
        Stop();

        StopCoroutine(CR_DASH);
        StopCoroutine(CR_THROW_RECOVERY);

        knockbackVector = Disc.Instance.Velocity;

        Disc.Instance.Catch(cTransform.position + (Vector3.right * GetDiscOffset()));

        if (Disc.Instance.HasKnockback)
            StartCoroutine(CR_KNOCKBACK);

        cPhotonView.RPC("RPC_Catch", PhotonTargets.AllViaServer);
    }

    private void Throw()
    {
        if (throwDirection.x < Mathf.Abs(throwDirectionThreshhold))
            switch (Team)
            {
                case Team.LEFT: throwDirection.x = throwDirectionThreshhold; break;
                case Team.RIGHT: throwDirection.x = -throwDirectionThreshhold; break;
            }

        // When throwCharge = 0, throwVector.x = throwPower / 2; throwCharge = 100, throwVector.x = throwPower
        float throwPower = (ThrowPower + ThrowPowerMod) * ThrowPowerMultiplier;
        Vector2 throwVector = throwDirection * ((((throwPower / 2) * throwCharge) / 100) + (throwPower / 2));

        switch (Team)
        {
            case Team.LEFT: throwVector.x = Mathf.Abs(throwVector.x); break;
            case Team.RIGHT: throwVector.x = -Mathf.Abs(throwVector.x); break;
        }

        if (throwCharge == 0)
            throwVector.y = 0;

        Disc.Instance.Throw(cTransform.position + (Vector3.right * GetDiscOffset()), (Vector3)throwVector);

        if (throwCharge >= GreatThrowThreshhold &&
            throwCharge < PerfectThrowThreshhold)
        {
            if (onGreatThrow != null)
                onGreatThrow(this, EventArgs.Empty);

            Meter += MeterGainOnGreat;
            cPhotonView.RPC("RPC_OnGreatThrowOthers", PhotonTargets.Others);
        }
        else if (throwCharge >= PerfectThrowThreshhold)
        {
            if (onPerfectThrow != null)
                onPerfectThrow(this, EventArgs.Empty);

            Meter += MeterGainOnPerfect;
            Disc.Instance.HasKnockback = true;
            float knockback = (Knockback + KnockbackMod) * KnockbackMultiplier;
            Disc.Instance.KnockbackPower = knockback;
            cPhotonView.RPC("RPC_OnPerfectThrowOthers", PhotonTargets.Others);
        }

        throwCharge = 0;
        State = PlayerState.RECOVERY;
        StartCoroutine(CR_THROW_RECOVERY);

        if (onThrow != null)
            onThrow(this, EventArgs.Empty);
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

    #endregion

    #region Coroutines

    private IEnumerator CR_Charge()
    {
        State = PlayerState.CHARGE;
        throwCharge = 0;

        float nFrames = maxChargeDuration * 60;

        for (float i = 0; i < nFrames; i++)
        {
            throwCharge = (i / nFrames) * 100;
            yield return 0;
        }

        for (float i = nFrames; i > 0; i--)
        {
            throwCharge = (i / nFrames) * 100;
            yield return 0;
        }

        Throw();
    }

    private IEnumerator CR_Dash(Vector2 _directionVector)
    {
        State = PlayerState.DASH;
        float dashSpeed = (DashSpeed + DashSpeedMod) * DashSpeedMultiplier;

        if (dashSpeed < MIN_DASH_SPEED)
            dashSpeed = MIN_DASH_SPEED;

        cRigidbody2D.velocity = _directionVector * dashSpeed;
        yield return new WaitForSeconds(DashDuration);
        Stop();
        State = PlayerState.NORMAL;
    }

    private IEnumerator CR_ThrowRecovery()
    {
        State = PlayerState.RECOVERY;
        yield return new WaitForSeconds(ThrowRecovery);
        State = PlayerState.NORMAL;
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

        State = PlayerState.RESET;

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

    void MatchManager_OnCompleteResetAfterScore(object sender, EventArgs e)
    {
        State = PlayerState.NORMAL;
    }

    #endregion

    #region RPC

    [RPC]
    private void RPC_SetData(int _team, int _id)
    {
        Team = (Team)_team;

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
    private void RPC_Catch()
    {
        if (onCatch != null)
            onCatch(this, EventArgs.Empty);
    }

    [RPC]
    private void RPC_EX(Vector3 _inputVector)
    {

    }

    [RPC]
    private void RPC_OnGreatThrowOthers()
    {
        if (!cPhotonView.isMine)
        {
            if (onGreatThrow != null)
                onGreatThrow(this, EventArgs.Empty);

            Meter += MeterGainOnGreat;
        }
    }

    [RPC]
    private void RPC_OnPerfectThrowOthers()
    {
        if (!cPhotonView.isMine)
        {
            if (onPerfectThrow != null)
                onPerfectThrow(this, EventArgs.Empty);

            Meter += MeterGainOnPerfect;
            Disc.Instance.HasKnockback = true;
            Disc.Instance.KnockbackPower = Knockback;
        }
    }

    [RPC]
    private void RPC_RemoveKnockback()
    {
        Disc.Instance.HasKnockback = false;
        Disc.Instance.KnockbackPower = 0;
    }

    #endregion
}
