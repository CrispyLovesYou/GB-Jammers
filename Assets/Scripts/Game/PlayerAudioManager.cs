using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Player Audio")]
public class PlayerAudioManager : MonoBehaviour
{
    #region Fields
    
    public AudioClip[] General;
    public AudioClip[] VsTracksuit;
    public AudioClip[] VsVBomb;
    public AudioClip[] VsDirtyDan;
    public AudioClip[] VsScanlon;
    public AudioClip[] EX;
    public AudioClip[] Special;
    public AudioClip[] Score;
    public AudioClip[] Win;
    public AudioClip[] Throw;
    public AudioClip[] Knockback;

    private Controller_Player player;
    private AudioSource cAudioSource;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        player = GetComponent<Controller_Player>();
        cAudioSource = GetComponent<AudioSource>();

        Controller_Player.OnEX += Controller_Player_OnEX;
        Controller_Player.OnSuper += Controller_Player_OnSuper;
        Controller_Player.OnThrow += Controller_Player_OnThrow;
        Controller_Player.OnKnockback += Controller_Player_OnKnockback;
        MatchManager.OnScored += MatchManager_OnScored;
        MatchManager.OnMatchEnd += MatchManager_OnMatchEnd;

        MatchManager.OnMatchStartVoice += MatchManager_OnMatchStartVoice;
    }

    private void OnDestroy()
    {
        Controller_Player.OnEX -= Controller_Player_OnEX;
        Controller_Player.OnSuper -= Controller_Player_OnSuper;
        Controller_Player.OnThrow -= Controller_Player_OnThrow;
        Controller_Player.OnKnockback -= Controller_Player_OnKnockback;
        MatchManager.OnScored -= MatchManager_OnScored;
        MatchManager.OnMatchEnd -= MatchManager_OnMatchEnd;

        MatchManager.OnMatchStartVoice -= MatchManager_OnMatchStartVoice;
    }

    #endregion

    #region Callbacks

    void Controller_Player_OnEX(object sender, EXEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        if (!cAudioSource.isPlaying && EX.Length != 0)
        {
            cAudioSource.clip = EX[UnityEngine.Random.Range(0, EX.Length)];
            cAudioSource.Play();
        }
    }

    void Controller_Player_OnSuper(object sender, SuperEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        if (!cAudioSource.isPlaying && Special.Length != 0)
        {
            cAudioSource.clip = Special[UnityEngine.Random.Range(0, Special.Length)];
            cAudioSource.Play();
        }
    }

    void Controller_Player_OnThrow(object sender, System.EventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        if (!cAudioSource.isPlaying && Throw.Length != 0)
        {
            cAudioSource.clip = Throw[UnityEngine.Random.Range(0, Throw.Length)];
            cAudioSource.Play();
        }
    }

    void Controller_Player_OnKnockback(object sender, System.EventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        if (!cAudioSource.isPlaying && Knockback.Length != 0)
        {
            cAudioSource.clip = Knockback[UnityEngine.Random.Range(0, Knockback.Length)];
            cAudioSource.Play();
        }
    }

    void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        if (e.Team != player.Team)
            return;

        if (!cAudioSource.isPlaying && Score.Length != 0)
        {
            cAudioSource.clip = Score[UnityEngine.Random.Range(0, Score.Length)];
            cAudioSource.Play();
        }
    }

    void MatchManager_OnMatchEnd(object sender, MatchEndEventArgs e)
    {
        if (e.Winner != player.Team)
            return;

        if (Win.Length != 0)
            cAudioSource.clip = Win[UnityEngine.Random.Range(0, Win.Length)];

        StartCoroutine(CR_WaitForMatchEndClip());
    }

    void MatchManager_OnMatchStartVoice(object sender, MatchStartVoiceEventArgs e)
    {
        if (e.Team != player.Team)
            return;

        CharacterID enemy = CharacterID.DR_TRACKSUIT;

        switch (e.Team)
        {
            case Team.LEFT: enemy = Globals.SelectedCharacters[1]; break;
            case Team.RIGHT: enemy = Globals.SelectedCharacters[0]; break;
        }

        switch (enemy)
        {
            default:
                if (General.Length != 0)
                    cAudioSource.clip = General[UnityEngine.Random.Range(0, General.Length)];
                else
                    cAudioSource.clip = null;
                    break;
            case CharacterID.DR_TRACKSUIT:
                if (VsTracksuit.Length != 0)
                    cAudioSource.clip = VsTracksuit[UnityEngine.Random.Range(0, VsTracksuit.Length)];
                else
                    cAudioSource.clip = null;
                    break;
            case CharacterID.V_BOMB:
                if (VsVBomb.Length != 0)
                    cAudioSource.clip = VsVBomb[UnityEngine.Random.Range(0, VsVBomb.Length)];
                else
                    cAudioSource.clip = null;
                    break;
            case CharacterID.DIRTY_DAN_RYCKERT:
                if (VsDirtyDan.Length != 0)
                    cAudioSource.clip = VsDirtyDan[UnityEngine.Random.Range(0, VsDirtyDan.Length)];
                else
                    cAudioSource.clip = null;
                    break;
            case CharacterID.METAL_GEAR_SCANLON:
                if (VsScanlon.Length != 0)
                    cAudioSource.clip = VsScanlon[UnityEngine.Random.Range(0, VsScanlon.Length)];
                else
                    cAudioSource.clip = null;
                    break;
        }

        StartCoroutine(CR_WaitForMatchStartClip(e.Team));
    }

    #endregion

    #region Coroutines

    private IEnumerator CR_WaitForMatchStartClip(Team _team)
    {
        if (cAudioSource.clip != null &&
            Globals.SelectedCharacters[0] != Globals.SelectedCharacters[1])
        {
            cAudioSource.Play();
            yield return new WaitForSeconds(cAudioSource.clip.length);
        }

        int team = 0;

        if (_team == Team.RIGHT)
            team = 1;

        MatchManager.Instance.MatchStartVoiceComplete[team] = true;
    }

    private IEnumerator CR_WaitForMatchEndClip()
    {
        if (cAudioSource.clip != null)
        {
            cAudioSource.Play();
            yield return new WaitForSeconds(cAudioSource.clip.length);
        }

        MatchManager.Instance.MatchEndVoiceComplete = true;
    }

    #endregion
}