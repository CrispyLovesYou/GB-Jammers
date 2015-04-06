using UnityEngine;
using System.Collections;

public class FenceShake : MonoBehaviour
{
    #region Fields

    private Animator cAnimator;
    private AudioSource cAudioSource;
    private bool disableFromMagnet = false;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cAnimator = GetComponent<Animator>();
        cAudioSource = GetComponent<AudioSource>();

        MatchManager.OnScored += MatchManager_OnScored;
        Controller_Player.OnCatch += Controller_Player_OnCatch;
    }

	private void OnDestroy(){
		MatchManager.OnScored -= MatchManager_OnScored;
		Controller_Player.OnCatch -= Controller_Player_OnCatch;
	}

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (_collider2D.tag == Disc.Instance.tag && !disableFromMagnet)
        {
            cAnimator.SetTrigger("shake");
            cAudioSource.Play();

            if (Disc.Instance.IsMagnet)
                disableFromMagnet = true;
        }
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnScored(object sender, ScoredEventArgs e)
    {
        disableFromMagnet = false;
    }

    private void Controller_Player_OnCatch(object sender, System.EventArgs e)
    {
        disableFromMagnet = false;
    }

    #endregion
}