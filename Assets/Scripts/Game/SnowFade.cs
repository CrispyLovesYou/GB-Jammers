using UnityEngine;
using System.Collections;

public class SnowFade : MonoBehaviour
{
    #region Fields

    public float FadeDuration = 2.0f;

    private SpriteRenderer cSpriteRenderer;
    private Color color;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cSpriteRenderer = GetComponent<SpriteRenderer>();
        color = cSpriteRenderer.color;
        color.a = 0;
        cSpriteRenderer.color = color;
    }

    private void Start()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", color.a,
            "to", 1.0f,
            "time", FadeDuration,
            "onupdate",
                (System.Action<object>)(value =>
                {
                    color.a = (float)value;
                    cSpriteRenderer.color = color;
                })
            ));
    }

    #endregion

    #region Methods

    public void FadeOut()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", color.a,
            "to", 0,
            "time", FadeDuration,
            "onupdate",
                (System.Action<object>)(value =>
                {
                    color.a = (float)value;
                    cSpriteRenderer.color = color;
                }),
            "oncomplete",
                (System.Action<object>)(p =>
                {
                    Destroy(this.gameObject);
                })
            ));
    }

    #endregion
}