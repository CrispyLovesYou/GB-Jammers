using UnityEngine;
using System.Collections;

public class CharacterSelectManager : MonoBehaviour
{
    #region Unity Callbacks

    private void Awake()
    {
        LoadCharacterData();
    }

    #endregion

    #region Methods

    public void SetCharacter_P1(int _id)
    {
        Globals.SelectedCharacters[0] = (CharacterID)_id;
    }

    public void SetCharacter_P2(int _id)
    {
        Globals.SelectedCharacters[1] = (CharacterID)_id;
    }

    public void LoadScene(string _id)
    {
        Application.LoadLevel(_id);
    }

    private void LoadCharacterData()
    {
        TextAsset ta;

        ta = Resources.Load("data_dr-tracksuit") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.DR_TRACKSUIT, Character.Deserialize(ta.text));

        ta = Resources.Load("data_v-bomb") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.V_BOMB, Character.Deserialize(ta.text));

        ta = Resources.Load("data_dirty-dan-ryckert") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.DIRTY_DAN_RYCKERT, Character.Deserialize(ta.text));

        ta = Resources.Load("data_metal-gear-scanlon") as TextAsset;
        Globals.CharacterDict.Add(CharacterID.METAL_GEAR_SCANLON, Character.Deserialize(ta.text));
    }

    #endregion
}