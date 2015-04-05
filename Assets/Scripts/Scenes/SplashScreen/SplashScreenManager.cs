using UnityEngine;
using System.Collections;

[AddComponentMenu("Managers/Scene/Splash Screen")]
public class SplashScreenManager : Singleton<SplashScreenManager>
{
    #region Constants

    private const string TITLE_SCREEN = "title_screen";

    #endregion

    #region Unity Callbacks

    protected override void Awake()
    {
        base.Awake();
        LoadCharacterData();
        Application.LoadLevel(TITLE_SCREEN);
    }

    #endregion

    #region Methods

    public static void LoadCharacterData()
    {
        TextAsset ta;

        ta = Resources.Load("data_dr_tracksuit") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.DR_TRACKSUIT, Character.Deserialize(ta.text));

        ta = Resources.Load("data_v_bomb") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.V_BOMB, Character.Deserialize(ta.text));

        ta = Resources.Load("data_dirty_dan_ryckert") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.DIRTY_DAN_RYCKERT, Character.Deserialize(ta.text));

        ta = Resources.Load("data_metal_gear_scanlon") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.METAL_GEAR_SCANLON, Character.Deserialize(ta.text));
    }

    #endregion
}