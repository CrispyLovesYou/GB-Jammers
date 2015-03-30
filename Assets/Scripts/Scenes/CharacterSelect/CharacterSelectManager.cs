using UnityEngine;
using System.Collections;

[AddComponentMenu("Managers/Scene/Character Select")]
public class CharacterSelectManager : Singleton<CharacterSelectManager>
{
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

    #endregion
}