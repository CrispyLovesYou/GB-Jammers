using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/Book")]
public class Book : MonoBehaviour
{
    #region Fields

    public static float StunDuration = 1.5f;
    public float Duration = 10.0f;
    public float LobDuration = 1.2f;
    public float LobScale = 5.0f;
    public float AngularVelocity = 180.0f;
    public Vector2 TargetPosition = Vector2.zero;

    public int ID { get; private set; }
    private static int bookCount = 0;

    private Transform cTransform;
    private Rigidbody2D cRigidbody2D;
    private Collider2D cCollider2D;
    private SpriteRenderer cSpriteRenderer;
    private Color color;

    private float fadeOutDuration = 1.0f;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        ID = bookCount;
        bookCount++;

        cTransform = GetComponent<Transform>();
        cRigidbody2D = GetComponent<Rigidbody2D>();
        cCollider2D = GetComponent<Collider2D>();
        cSpriteRenderer = GetComponent<SpriteRenderer>();

        cCollider2D.enabled = false;
        color = cSpriteRenderer.color;
        cTransform.rotation = Quaternion.Euler(0, 0, -AngularVelocity);
    }

    private void Start()
    {
        cRigidbody2D.angularVelocity = AngularVelocity;
        cSpriteRenderer.sortingOrder = 10;

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", (Vector3)TargetPosition,
            "time", LobDuration,
            "easetype", iTween.EaseType.linear));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", cTransform.localScale * LobScale,
            "time", LobDuration / 2,
            "easetype", iTween.EaseType.easeOutQuad,
            "oncomplete",
                (System.Action<object>)(param =>
                {
                    iTween.ScaleTo(gameObject, iTween.Hash(
                        "scale", cTransform.localScale / LobScale,
                        "time", LobDuration / 2,
                        "easetype", iTween.EaseType.easeInQuad,
                        "oncomplete", "Active"));
                })
            ));
    }

    #endregion

    #region Methods

    public void DestroySelf()
    {
        iTween.Stop(this.gameObject);
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", color.a,
            "to", 0,
            "time", fadeOutDuration,
            "onupdate",
                (System.Action<object>)(value =>
                {
                    color.a = (float)value;
                    cSpriteRenderer.color = color;
                }),
            "oncomplete",
                (System.Action<object>)(param =>
                {
                    Destroy(this.gameObject);
                })
            ));
    }

    private void Active()
    {
        cRigidbody2D.angularVelocity = 0;
        cSpriteRenderer.sortingOrder = 0;
        cCollider2D.enabled = true;
        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        
        yield return new WaitForSeconds(Duration - fadeOutDuration);

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", color.a,
            "to", 0,
            "time", fadeOutDuration,
            "onupdate",
                (System.Action<object>)(value =>
                {
                    color.a = (float)value;
                    cSpriteRenderer.color = color;
                }),
            "oncomplete",
                (System.Action<object>)(param =>
                {
                    Destroy(this.gameObject);
                })
            ));
    }

    #endregion
}