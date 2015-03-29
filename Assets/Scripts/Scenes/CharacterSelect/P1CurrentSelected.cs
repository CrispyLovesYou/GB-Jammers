using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class P1CurrentSelected : MonoBehaviour
{
    #region Fields

    private Text cText;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cText = GetComponent<Text>();
    }

    private void Update()
    {
        switch (Globals.SelectedCharacters[0])
        {
            case CharacterID.DR_TRACKSUIT: cText.text = "Dr. Tracksuit"; break;
            case CharacterID.V_BOMB: cText.text = "V-Bomb"; break;
            case CharacterID.DIRTY_DAN_RYCKERT: cText.text = "Dirty Dan Ryckert"; break;
            case CharacterID.METAL_GEAR_SCANLON: cText.text = "Metal Gear Scanlon"; break;
        }
    }

    #endregion
}