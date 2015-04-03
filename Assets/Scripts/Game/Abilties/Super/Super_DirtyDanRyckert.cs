using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/Dirty Dan Ryckert")]
public class Super_DirtyDanRyckert : Super_Base
{
    #region Constants

    private const float FLAG_OFFSET = 14.0f;

    #endregion

    #region Fields

    public GameObject Flag;
    public GameObject Fireworks;
    public float Duration = 10.0f;

    private Vector3 spawnPosition = Vector3.zero;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        Controller_Player.OnSuper += Controller_Player_OnSuper;
    }

    #endregion

    #region Coroutines

    private IEnumerator CR_Animate()
    {
        GameObject flag = Instantiate(Flag, spawnPosition, Quaternion.identity) as GameObject;

        iTween.MoveTo(flag, Vector3.zero, Duration / 8);
        yield return new WaitForSeconds(Duration / 8);

        GameObject fireworks = Instantiate(Fireworks, Vector3.zero, Quaternion.identity) as GameObject;

        yield return new WaitForSeconds(Duration - (Duration / 8));

        iTween.MoveTo(flag, iTween.Hash(
            "position", -spawnPosition,
            "time", Duration / 8,
            "oncomplete",
                (System.Action<object>)(param =>
                {
                    Destroy(flag);
                    Destroy(fireworks);
                })
            ));
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnSuper(object sender, SuperEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        switch (player.Team)
        {
            case Team.LEFT: spawnPosition = new Vector3(FLAG_OFFSET, 0, 0); break;
            case Team.RIGHT: spawnPosition = new Vector3(-FLAG_OFFSET, 0, 0); break;
        }

        StartCoroutine(CR_Animate());
    }

    #endregion
}