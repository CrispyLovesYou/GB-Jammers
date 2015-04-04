using UnityEngine;
using System.Collections;

public class FenceShake : MonoBehaviour
{
    #region Fields

    private Animator cAnimator;
    private AudioSource cAudioSource;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cAnimator = GetComponent<Animator>();
        cAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (_collider2D.tag == Disc.Instance.tag)
        {
            cAnimator.SetTrigger("shake");
            cAudioSource.Play();
        }
    }

    #endregion
}