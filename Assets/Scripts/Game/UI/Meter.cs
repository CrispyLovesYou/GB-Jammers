using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Game/UI/Meter")]
public class Meter : MonoBehaviour
{
    #region Fields

    public Team Team = Team.LEFT;
    public Image MeterNormal;
    public Image MeterSpecial;
    public Image EXFill;
    public Image SpecialFill;
    public int MeterToEX = 33;
    public int MeterToSpecial = 100;
    public float AnimationSpeed = 0.1f;

    private int meter;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        Controller_Player.OnMeterChange += Controller_Player_OnMeterChange;
    }

    private void Update()
    {
        if (meter >= 99)
            MeterSpecial.fillAmount = Mathf.Lerp(MeterSpecial.fillAmount, 1, AnimationSpeed);
        else
            MeterSpecial.fillAmount = 0.0f;

        MeterNormal.fillAmount = Mathf.Lerp(MeterNormal.fillAmount, (meter / 100.0f), AnimationSpeed);
    }

    #endregion

    #region Coroutines

    #endregion

    #region Callbacks

    private void Controller_Player_OnMeterChange(object sender, MeterChangeEventArgs e)
    {
        if (e.Team != this.Team)
            return;

        meter = e.Total;

        if (meter >= MeterToEX)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", EXFill.color.a,
                "to", 1.0f,
                "onupdate",
                    (System.Action<object>)(value =>
                    {
                        Color color = EXFill.color;
                        color.a = (float)value;
                        EXFill.color = color;
                    })
                ));
        }
        else
        {
            Color color = EXFill.color;
            color.a = 0;
            EXFill.color = color;
        }

        if (meter >= MeterToSpecial)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", SpecialFill.color.a,
                "to", 1.0f,
                "onupdate",
                    (System.Action<object>)(value =>
                    {
                        Color color = SpecialFill.color;
                        color.a = (float)value;
                        SpecialFill.color = color;
                    })
                ));
        }
        else
        {
            Color color = SpecialFill.color;
            color.a = 0;
            SpecialFill.color = color;
        }
    }

    #endregion
}