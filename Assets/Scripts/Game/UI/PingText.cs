using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Game/UI/Ping Text")]
public class PingText : MonoBehaviour
{
    #region Fields

    private Text cText;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cText = GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine(CR_Update());
    }

    private IEnumerator CR_Update()
    {
        while (PhotonNetwork.inRoom && !PhotonNetwork.offlineMode)
        {
            cText.text = PhotonNetwork.GetPing().ToString() + "ms";

            yield return new WaitForSeconds(3.0f);
        }

        cText.text = "";
    }

    #endregion
}