using UnityEngine;
using System.Collections;

public class StatusPopup : MonoBehaviour
{
    #region Fields

    public float Distance = 0.5f;
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
        StartCoroutine(HandleFade());
    }

    private void Update()
    {
        cTransform.position += Vector3.up * Distance * Time.deltaTime;
    }

    #endregion

    #region Coroutines

    private IEnumerator HandleFade()
    {
        float nFrames = (Duration / 2) * (1.0f / Time.deltaTime);

        for (int i = 0; i < nFrames; i++)
        {
            color.a = i / nFrames;
            cSpriteRenderer.color = color;

            yield return 0;
        }

        for (float i = nFrames; i >= 0; i--)
        {
            color.a = i / nFrames;
            cSpriteRenderer.color = color;

            yield return 0;
        }

        Destroy(this.gameObject);
    }

    #endregion
}