using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/UI/Scoreboard/Player Names")]
public class Scoreboard_Name : MonoBehaviour
{
    #region Fields

    public int Player = 1;

    private Animator cAnimator;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        cAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        switch (Globals.SelectedCharacters[Player - 1])
        {
            case CharacterID.DR_TRACKSUIT: cAnimator.SetInteger("character", 0); break;
            case CharacterID.V_BOMB: cAnimator.SetInteger("character", 1); break;
            case CharacterID.DIRTY_DAN_RYCKERT: cAnimator.SetInteger("character", 2); break;
            case CharacterID.METAL_GEAR_SCANLON: cAnimator.SetInteger("character", 3); break;
        }
    }

    #endregion
}