using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Meter : MonoBehaviour
{
    #region Fields

    public Team Team = Team.LEFT;
    public float Duration = 0.8f;

    private RectTransform cRectTransform;
    private float initWidth;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cRectTransform = GetComponent<RectTransform>();
        Controller_Player.OnMeterChange += Controller_Player_OnMeterChange;

        initWidth = cRectTransform.rect.width;

        switch (Team)
        {
            case global::Team.LEFT: cRectTransform.offsetMax = new Vector2(-initWidth, 0); break;
            case global::Team.RIGHT: cRectTransform.offsetMin = new Vector2(initWidth, 0); break;
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator HandleAnimation(int _meter)
    {
        float nFrames = Duration * (1.0f / Time.deltaTime);
        float targetOffset = ((float)_meter / 100) * initWidth;

        targetOffset = initWidth - targetOffset;

        for (int i = 0; i < nFrames; i++)
        {
            float offset = 0;
            float newWidth = 0;

            switch (Team)
            {
                case global::Team.LEFT:
                    offset = cRectTransform.offsetMax.x;
                    newWidth = Mathf.Lerp(offset, -targetOffset, 1.0f / nFrames);
                    cRectTransform.offsetMax = new Vector2(newWidth, 0);
                    break;
                case global::Team.RIGHT:
                    offset = cRectTransform.offsetMin.x;
                    newWidth = Mathf.Lerp(offset, targetOffset, 1.0f / nFrames);
                    cRectTransform.offsetMin = new Vector2(newWidth, 0);
                    break;
            }

            yield return 0;
        }
    }

    #endregion

    #region Callbacks

    private void Controller_Player_OnMeterChange(object sender, MeterChangeEventArgs e)
    {
        if (e.Team != this.Team)
            return;

        StartCoroutine(HandleAnimation(e.Total));
    }

    #endregion
}