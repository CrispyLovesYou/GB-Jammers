using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioClip))]
public class AudioManager : MonoBehaviour
{
    #region Fields

    private AudioSource cAudioSource;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cAudioSource = GetComponent<AudioSource>();
        MatchManager.OnMatchStart += MatchManager_OnMatchStart;
    }

    #endregion

    #region Callbacks

    private void MatchManager_OnMatchStart(object sender, System.EventArgs e)
    {
        cAudioSource.Play();
    }

    #endregion
}