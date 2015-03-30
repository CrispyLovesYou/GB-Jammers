using System;
using System.Collections.Generic;

public static class Globals
{
    private const int MAX_ALLOWED_PLAYERS = 2;

    public static Dictionary<CharacterID, Character> CharacterDict = new Dictionary<CharacterID, Character>();
    public static CharacterID[] SelectedCharacters = new CharacterID[MAX_ALLOWED_PLAYERS];
}