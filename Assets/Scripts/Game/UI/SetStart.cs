using UnityEngine;
using System.Collections;

public class SetStart : MonoBehaviour
{
    #region Fields

    public Animator SetNumber;

    #endregion

    #region Methods

    private void Update()
    {
        SetNumber.SetInteger("value", MatchManager.Instance.CurrentSet);
    }

    public void SetStartComplete()
    {
        MatchManager.Instance.SetStartComplete = true;
    }

    #endregion
}