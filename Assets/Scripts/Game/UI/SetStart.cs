using UnityEngine;
using System.Collections;

public class SetStart : MonoBehaviour
{
    #region Fields
    #endregion

    #region Methods

    public void SetStartComplete()
    {
        MatchManager.Instance.SetStartComplete = true;
    }

    #endregion
}