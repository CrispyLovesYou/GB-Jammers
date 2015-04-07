using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusic : MonoBehaviour
{
    #region Fields

    public static MainMenuMusic Instance;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            GetComponent<AudioSource>().Play();
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion
}