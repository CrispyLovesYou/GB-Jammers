using System;
using System.Collections.Generic;

public static class Globals
{
    #region Constants

    public const string GAME_VERSION = "0.1";
    public const int MAX_CONNECTED_PLAYERS = 2;

    public const int DEFAULT_SETS_TO_WIN_MATCH = 2;
    public const int DEFAULT_POINTS_TO_WIN_SET = 11;

    #endregion

    #region Fields

    public static GameModes GameMode = GameModes.MAIN_MENU;
    public static Dictionary<CharacterID, Character> CharacterDict = new Dictionary<CharacterID, Character>();
    public static CharacterID[] SelectedCharacters = new CharacterID[MAX_CONNECTED_PLAYERS];
    public static string Username = "";
	public static InputType[] PlayerInputs = new InputType[2]{InputType.KEYBOARD, InputType.KEYBOARD};
    public static bool HasGameStarted = false;

    #endregion
}