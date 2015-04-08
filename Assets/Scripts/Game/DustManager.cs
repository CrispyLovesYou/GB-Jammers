using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Dust Manager")]
[RequireComponent(typeof(Controller_Player))]
public class DustManager : MonoBehaviour
{
    #region Fields

    public GameObject Dust;
    public float DustSpawnDelay = 0.12f;
    public float DustVerticalOffset = -0.45f;

    private Controller_Player player;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        player = GetComponent<Controller_Player>();

        Controller_Player.OnDash += Controller_Player_OnDash;
        Controller_Player.OnKnockback += Controller_Player_OnKnockback;
    }

    private void OnDestroy()
    {
        Controller_Player.OnDash -= Controller_Player_OnDash;
        Controller_Player.OnKnockback -= Controller_Player_OnKnockback;
    }

    #endregion

    #region Coroutines

    public IEnumerator CR_Dash()
    {
        while (player.State == PlayerState.DASH)
        {
            Vector3 position = player.transform.position;
            position.y += DustVerticalOffset;
            Instantiate(Dust, position, Quaternion.identity);
            yield return new WaitForSeconds(DustSpawnDelay);
        }
    }

    public IEnumerator CR_Knockback()
    {
        while (player.State == PlayerState.KNOCKBACK)
        {
            Vector3 position = player.transform.position;
            position.y += DustVerticalOffset;
            Instantiate(Dust, position, Quaternion.identity);
            yield return new WaitForSeconds(DustSpawnDelay);
        }
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnDash(object sender, System.EventArgs e)
    {
        StartCoroutine(CR_Dash());
    }

    private void Controller_Player_OnKnockback(object sender, System.EventArgs e)
    {
        StartCoroutine(CR_Knockback());
    }

    #endregion
}