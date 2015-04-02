using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/UI/Status Popup")]
public class StatusPopup : MonoBehaviour
{
    #region Enums

    public enum PopupDirection
    {
        UP,
        DOWN
    }

    #endregion

    #region Fields

    public PopupDirection Direction = PopupDirection.UP;
    public float Distance = 0.25f;
    public float Duration = 2.0f;

    private Transform cTransform;
    private SpriteRenderer cSpriteRenderer;
    private Color color;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cTransform = GetComponent<Transform>();
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
            "time", Duration / 2,
            "onupdate",
                (System.Action<object>)(value =>
                {
                    color.a = (float)value;
                    cSpriteRenderer.color = color;
                }),
            "oncomplete",
                (System.Action<object>)(param =>
                {
                    iTween.ValueTo(gameObject, iTween.Hash(
                        "from", color.a,
                        "to", 0,
                        "time", Duration / 2,
                        "onupdate",
                            (System.Action<object>)(value =>
                            {
                                color.a = (float)value;
                                cSpriteRenderer.color = color;
                            })
                    ));
                })
        ));
    }

    private void Update()
    {
        switch (Direction)
        {
            case PopupDirection.UP: cTransform.position += Vector3.up * Distance * Time.deltaTime; break;
            case PopupDirection.DOWN: cTransform.position += Vector3.down * Distance * Time.deltaTime; break;
        }
    }

    #endregion
}