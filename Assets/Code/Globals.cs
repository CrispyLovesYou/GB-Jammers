using System;
using System.Collections.Generic;

public static class Globals
{
    #region Constants

    public const string GAME_VERSION = "0.1";
    public const int MAX_CONNECTED_PLAYERS = 2;

    #endregion

    #region Fields

    public static GameModes GameMode = GameModes.MAIN_MENU;
    public static Dictionary<CharacterID, Character> CharacterDict = new Dictionary<CharacterID, Character>();
    public static CharacterID[] SelectedCharacters = new CharacterID[MAX_CONNECTED_PLAYERS];

    #endregion
}