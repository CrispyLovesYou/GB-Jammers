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

    #endregion

    #region Callbacks

    private void Controller_Player_OnMeterChange(object sender, MeterChangeEventArgs e)
    {
        if (e.Team != this.Team)
            return;

        float targetOffset = ((float)e.Total / 100) * initWidth;
        targetOffset = initWidth - targetOffset;

        switch (Team)
        {
            case global::Team.LEFT:
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", cRectTransform.offsetMax.x,
                    "to", -targetOffset,
                    "time", Duration,
                    "onupdate",
                        (System.Action<object>)(value => {
                            switch (Team)
                            {
                                case global::Team.LEFT: cRectTransform.offsetMax = new Vector2((float)value, 0); break;
                                case global::Team.RIGHT: cRectTransform.offsetMin = new Vector2((float)value, 0); break;
                            }
                        })
                    ));
                break;

            case global::Team.RIGHT:
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", cRectTransform.offsetMin.x,
                    "to", targetOffset,
                    "time", Duration,
                    "onupdate",
                        (System.Action<object>)(value =>
                        {
                            switch (Team)
                            {
                                case global::Team.LEFT: cRectTransform.offsetMax = new Vector2((float)value, 0); break;
                                case global::Team.RIGHT: cRectTransform.offsetMin = new Vector2((float)value, 0); break;
                            }
                        })
                    ));
                break;
        }
    }

    #endregion
}