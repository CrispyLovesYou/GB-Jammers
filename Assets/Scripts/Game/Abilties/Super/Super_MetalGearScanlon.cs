using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/Metal Gear Scanlon")]
public class Super_MetalGearScanlon : Super_Base
{
    #region Fields

    public int ThrowCharge = 70;
    public float Duration = 0.5f;

    private SpriteRenderer discRenderer;
    private Color color;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();

        Controller_Player.OnSuper += Controller_Player_OnSuper;
    }

	private void OnDestroy(){
		Controller_Player.OnSuper -= Controller_Player_OnSuper;
	}

    #endregion

    #region Callbacks

    private void Controller_Player_OnSuper(object sender, SuperEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        player.SpecialThrow(ThrowCharge, e.InputVector, true);

        if (discRenderer == null)
            discRenderer = Disc.Instance.GetComponent<SpriteRenderer>();

        color = discRenderer.color;

        StartCoroutine(CR_WaitForAnimationDelay());
    }

    #endregion

    #region Coroutines

    private IEnumerator CR_WaitForAnimationDelay()
    {
        yield return new WaitForSeconds(0.2f);

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", color.a,
            "to", 0,
            "time", 0.2f,
            "onupdate",
                (System.Action<object>)(value =>
                {
                    color.a = (float)value;
                    discRenderer.color = color;
                })
            ));

        StartCoroutine(CR_FadeIn());
    }

    private IEnumerator CR_FadeIn()
    {
        yield return new WaitForSeconds(Duration + 0.2f);

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", color.a,
            "to", 1.0f,
            "time", 0.2f,
            "onupdate",
                (System.Action<object>)(value =>
                {
                    color.a = (float)value;
                    discRenderer.color = color;
                })
            ));
    }

    #endregion
}