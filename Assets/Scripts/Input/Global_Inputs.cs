using UnityEngine;
using System.Collections;

public class Global_Inputs : MonoBehaviour
{
    #region Fields

    private static Global_Inputs instance;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M))
        {
            if (AudioListener.volume == 0)
                AudioListener.volume = 1.0f;
            else
                AudioListener.volume = 0;
        }
    }

    #endregion
}