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
    public Transform ParentTransform;
    public Vector3 RelativePosition = Vector3.zero;
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

        RelativePosition = cTransform.position;

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
                            }),
                        "oncomplete",
                            (System.Action<object>)(param2 =>
                            {
                                Destroy(this.gameObject);
                            })
                    ));
                })
        ));
    }

    private void Update()
    {
        switch (Direction)
        {
            case PopupDirection.UP: RelativePosition += Vector3.up * Distance * Time.deltaTime; break;
            case PopupDirection.DOWN: RelativePosition += Vector3.down * Distance * Time.deltaTime; break;
        }

        cTransform.position = ParentTransform.position + RelativePosition;
    }

    #endregion
}