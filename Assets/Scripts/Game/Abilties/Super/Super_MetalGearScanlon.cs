﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Abilities/Super/Metal Gear Scanlon")]
public class Super_MetalGearScanlon : Super_Base
{
    #region Fields

    public float Duration = 0.5f;

    private SpriteRenderer discRenderer;
    private Color color;

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        discRenderer = Disc.Instance.GetComponent<SpriteRenderer>();
        color = discRenderer.color;

        Controller_Player.OnSuper += Controller_Player_OnSuper;
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnSuper(object sender, SuperEventArgs e)
    {
        if ((Controller_Player)sender != player)
            return;

        player.SpecialThrow(e.InputVector, true);
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

    #endregion

    #region Coroutines

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