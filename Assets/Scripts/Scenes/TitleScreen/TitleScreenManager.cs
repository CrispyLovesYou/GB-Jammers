using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Managers/Scene/Title Screen")]
public class TitleScreenManager : MonoBehaviour
{
    #region Constants

    private const string MAIN_MENU_SCENE = "main_menu";

    #endregion

    #region Fields

    public float IntroDuration = 9.0f;
    public float FadeOutDuration = 2.0f;
    public AudioSource Music;
    public CanvasGroup Fade;

    private AudioSource cAudioSource;
    private float initVolume = 1.0f;
    private bool isReadyForInput = false;
    private bool fading = false;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cAudioSource = GetComponent<AudioSource>();
        initVolume = Music.volume;
    }

    private void Start()
    {
        StartCoroutine(CR_WaitForAllowedInput());
    }

    private void Update()
    {
        if (!isReadyForInput)
            return;

        if (Input.anyKeyDown && !fading)
        {
            cAudioSource.Play();

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", Fade.alpha,
                "to", 1.0f,
                "time", FadeOutDuration,
                "onupdate",
                    (System.Action<object>)(value =>
                    {
                        Fade.alpha = (float)value;
                        Music.volume = initVolume * (1.0f - (float)value);
                    }),
                "oncomplete",
                    (System.Action<object>)(p =>
                    {
                        Application.LoadLevel(MAIN_MENU_SCENE);
                    })
                ));

            fading = true;
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator CR_WaitForAllowedInput()
    {
        yield return new WaitForSeconds(IntroDuration);
        isReadyForInput = true;
    }

    #endregion
}