using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Animate_Super_Mask : MonoBehaviour
{
    #region Fields

    public Image cImage;
    public Text cText;

    public Sprite DrTracksuit;
    public Sprite VBomb;
    public Sprite DirtyDanRyckert;
    public Sprite MetalGearScanlon;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        Time.timeScale = 0;
        MatchManager.Instance.IsPauseAllowed = false;
    }

    #endregion

    #region Methods

    public void SetData(CharacterID _character)
    {
        switch (_character)
        {
            case CharacterID.DR_TRACKSUIT: cImage.sprite = DrTracksuit; break;
            case CharacterID.V_BOMB: cImage.sprite = VBomb; break;
            case CharacterID.DIRTY_DAN_RYCKERT: cImage.sprite = DirtyDanRyckert; break;
            case CharacterID.METAL_GEAR_SCANLON: cImage.sprite = MetalGearScanlon; break;
        }

        cText.text = Globals.CharacterDict[_character].AbilitySuper;
    }

    public void DestroyAfterAnimation()
    {
        Time.timeScale = 1.0f;
        MatchManager.Instance.IsPauseAllowed = true;
        Destroy(this.gameObject);
    }

    #endregion
}