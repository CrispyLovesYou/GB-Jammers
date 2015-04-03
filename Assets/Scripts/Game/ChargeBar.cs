using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/ChargeBar")]
public class ChargeBar : MonoBehaviour
{
    #region Fields

    public float OffsetX = 0.7f;

    private Transform cTransform;
    private GameObject player;
    private Controller_Player playerScript;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cTransform = GetComponent<Transform>();
        cTransform.localScale = new Vector3(cTransform.localScale.x, 0, 1);
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector3 position = player.transform.position;
        position.x -= OffsetX;
        cTransform.position = position;
        cTransform.localScale = new Vector3(cTransform.localScale.x, playerScript.ThrowCharge / 100, 1);
    }

    #endregion

    #region Methods

    public void SetPlayer(GameObject _player)
    {
        player = _player;
        playerScript = _player.GetComponent<Controller_Player>();
    }

    #endregion
}