using UnityEngine;
using System.Collections;

public class MainMenuMusic : MonoBehaviour
{
    #region Fields

    public static MainMenuMusic Instance;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion
}